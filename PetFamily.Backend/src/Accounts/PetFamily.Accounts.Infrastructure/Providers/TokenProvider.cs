using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PetFamily.Accounts.Application.Interfaces;
using PetFamily.Accounts.Application.Models;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Accounts.Infrastructure.Database;
using PetFamily.Accounts.Infrastructure.Extensions;
using PetFamily.Accounts.Infrastructure.Options;
using PetFamily.Core.Models;
using PetFamily.SharedKernel;

namespace PetFamily.Accounts.Infrastructure.Providers;

public class TokenProvider : ITokenProvider
{
    private readonly JwtOptions _jwtOptions;
    private readonly RefreshOptions _refreshOptions;
    private readonly AccountsDbContext _accountsContext;
    private readonly ILogger<TokenProvider> _logger;

    public TokenProvider(
        IOptions<JwtOptions> jwtOptions,
        IOptions<RefreshOptions> refreshOptions,
        AccountsDbContext accountsContext,
        ILogger<TokenProvider> logger)
    {
        _jwtOptions = jwtOptions.Value;
        _refreshOptions = refreshOptions.Value;
        _accountsContext = accountsContext;
        _logger = logger;
    }

    public AccessTokenResult GenerateAccessToken(User user)
    {
        _logger.LogInformation("Generating access token");
        
        var jti = Guid.NewGuid();
        var roleClaims = user.Roles.Select(r => new Claim(CustomClaims.ROLE, r.Name!));

        var claims = new List<Claim>
        {
            new(CustomClaims.NAME_IDENTIFIER, user.Id.ToString()),
            new(CustomClaims.EMAIL, user.Email!),
            new(CustomClaims.JTI, jti.ToString())
        }.Concat(roleClaims);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var jwtToken = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenLifeTimeMinutes),
            signingCredentials: signingCredentials,
            claims: claims);

        var stringToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        _logger.LogInformation("Successfully generated access token");

        return new AccessTokenResult(stringToken, jti);
    }

    public async Task<Guid> GenerateRefreshTokenAsync(User user, Guid jti,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating refresh token");

        var token = Guid.NewGuid();

        var refreshSession = new RefreshSession(
            token,
            jti,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(_refreshOptions.RefreshTokenLifeTimeDays),
            user);

        await _accountsContext.RefreshSessions.AddAsync(refreshSession, cancellationToken);
        await _accountsContext.SaveChangesAsync(cancellationToken);

        return refreshSession.Token;
    }

    public async Task<Result<IReadOnlyList<Claim>, Error>> GetUserClaimsFromTokenAsync(string accessToken)
    {
        var tokenValidationParameters = TokenValidationParametersFactory.CreateWithOutLifeTime(_jwtOptions);

        var validationResult = await new JwtSecurityTokenHandler()
            .ValidateTokenAsync(accessToken, tokenValidationParameters);

        if (validationResult.IsValid)
            return validationResult.ClaimsIdentity.Claims.ToList();

        _logger.LogError("Failed to validate token {AccessToken}", accessToken);
        return Errors.Authorization.InvalidAccessToken();
    }
}