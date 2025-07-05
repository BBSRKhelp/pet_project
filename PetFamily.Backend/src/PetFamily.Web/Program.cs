using PetFamily.Accounts.Application;
using PetFamily.Accounts.Infrastructure;
using PetFamily.Accounts.Presentation;
using PetFamily.Files.Infrastructure;
using PetFamily.Files.Presentation;
using PetFamily.Species.Application;
using PetFamily.Species.Infrastructure;
using PetFamily.Species.Presentation;
using PetFamily.Volunteers.Application;
using PetFamily.Volunteers.Infrastructure;
using PetFamily.Volunteers.Presentation;
using PetFamily.Web;
using PetFamily.Web.Extensions;
using PetFamily.Web.Middlewares;
using Serilog;
using SwaggerThemes;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWeb(builder.Configuration);

builder.Services.AddAccountsPresentation() //DI Accounts
    .AddAccountsApplication()
    .AddAccountsInfrastructure(builder.Configuration)
    .ConfigureAuthentication(builder.Configuration)
    .ConfigureAuthorization();

builder.Services.AddFilePresentation() //DI Files
    .AddFileInfrastructure(builder.Configuration);

builder.Services.AddSpeciesPresentation() //DI Species
    .AddSpeciesApplication()
    .AddSpeciesInfrastructure(builder.Configuration);

builder.Services.AddVolunteersPresentation() //DI Volunteers
    .AddVolunteerApplication()
    .AddVolunteerInfrastructure(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
    await app.UseAccountsSeederAsync();

app.UseExceptionMiddleware();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(Theme.UniversalDark);

    // await app.ApplyMigrations();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

namespace PetFamily.Web
{
    public partial class Program;
}