using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Accounts.Contracts.DTOs;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Accounts.Domain.ValueObjects;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.Accounts.Infrastructure.Database.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.ComplexProperty(u => u.FullName, fnb =>
        {
            fnb.Property(fn => fn.FirstName)
                .IsRequired()
                .HasColumnName("first_name")
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH);

            fnb.Property(fn => fn.LastName)
                .IsRequired()
                .HasColumnName("last_name")
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH);

            fnb.Property(fn => fn.Patronymic)
                .IsRequired(false)
                .HasColumnName("patronymic")
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH);
        });

        builder.Property(u => u.PhotoPath)
            .IsRequired(false)
            .HasConversion(
                pp => pp != null ? pp.Path : null,
                s => s != null ? PhotoPath.Create(s).Value : null
            )
            .HasColumnName("photo");

        builder.Property(v => v.SocialNetworks)
            .IsRequired()
            .ValueObjectsCollectionJsonConversion(
                sn => new SocialNetworkDto(sn.Title, sn.Url),
                dto => SocialNetwork.Create(dto.Title, dto.Url).Value);

        builder.HasMany(u => u.Roles)
            .WithMany()
            .UsingEntity<IdentityUserRole<Guid>>();

        builder.Navigation(u => u.Roles).AutoInclude();
    }
}