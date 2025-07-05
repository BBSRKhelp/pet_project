using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Core.Enums;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.Volunteers.Contracts.DTOs.Pet;
using PetFamily.Volunteers.Domain.Entities;
using PetFamily.Volunteers.Domain.ValueObjects;
using PetFamily.Volunteers.Domain.ValueObjects.Ids;

namespace PetFamily.Volunteers.Infrastructure.Database.Configurations.Write;

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("pets");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => PetId.Create(value))
            .HasColumnOrder(1);

        builder.ComplexProperty(p => p.Name, nb =>
        {
            nb.Property(n => n.Value)
                .IsRequired(false)
                .HasColumnName("name")
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH)
                .HasColumnOrder(2);
        });

        builder.ComplexProperty(p => p.Description, db =>
        {
            db.Property(d => d.Value)
                .IsRequired(false)
                .HasColumnName("description")
                .HasMaxLength(Constants.MAX_MEDIUM_TEXT_LENGTH)
                .HasColumnOrder(3);
        });

        builder.ComplexProperty(p => p.AppearanceDetails, ab =>
        {
            ab.Property(ad => ad.Coloration)
                .IsRequired()
                .HasColumnName("coloration")
                .HasDefaultValue(Colour.Unknown)
                .HasColumnOrder(4);

            ab.Property(ad => ad.Weight)
                .IsRequired()
                .HasColumnName("weight")
                .HasDefaultValue(0)
                .HasColumnOrder(5);

            ab.Property(ad => ad.Height)
                .IsRequired()
                .HasColumnName("height")
                .HasDefaultValue(0)
                .HasColumnOrder(6);
        });

        builder.ComplexProperty(p => p.Address, ab =>
        {
            ab.Property(p => p.Country)
                .IsRequired()
                .HasColumnName("country")
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH)
                .HasColumnOrder(7);

            ab.Property(p => p.City)
                .IsRequired()
                .HasColumnName("city")
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH)
                .HasColumnOrder(8);

            ab.Property(p => p.Street)
                .IsRequired()
                .HasColumnName("street")
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH)
                .HasColumnOrder(9);

            ab.Property(p => p.PostalCode)
                .IsRequired(false)
                .HasColumnName("postal_code")
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH)
                .HasColumnOrder(10);
        });

        builder.Property(p => p.BirthDate)
            .IsRequired(false)
            .HasColumnOrder(11);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasColumnOrder(12);

        builder.ComplexProperty(p => p.HealthDetails, hdb =>
        {
            hdb.Property(hd => hd.HealthInformation)
                .IsRequired()
                .HasColumnName("health_information")
                .HasMaxLength(Constants.MAX_MEDIUM_LOW_TEXT_LENGTH)
                .HasColumnOrder(13);

            hdb.Property(hd => hd.IsCastrated)
                .IsRequired()
                .HasColumnName("is_castrated")
                .HasDefaultValue(false)
                .HasColumnOrder(14);

            hdb.Property(hd => hd.IsVaccinated)
                .IsRequired()
                .HasColumnName("is_vaccinated")
                .HasDefaultValue(false)
                .HasColumnOrder(15);
        });

        builder.ComplexProperty(p => p.Position, snb =>
        {
            snb.Property(sn => sn.Value)
                .IsRequired()
                .HasColumnName("position")
                .HasColumnOrder(16);
        });

        builder.Property(p => p.PetPhotos)
            .IsRequired()
            .ValueObjectsCollectionJsonConversion(
                pp => new PetPhotoDto(pp.PhotoPath.Path, pp.IsMainPhoto),
                dto => new PetPhoto(PhotoPath.Create(dto.PhotoPath).Value, dto.IsMainPhoto))
            .HasColumnOrder(17);
        
        builder.ComplexProperty(p => p.BreedAndSpeciesId, bsb =>
        {
            bsb.Property(ad => ad.SpeciesId)
                .IsRequired()
                .HasColumnName("species_id")
                .HasConversion(
                    id => id.Value,
                    value => SpeciesId.Create(value))
                .HasColumnOrder(18);

            bsb.Property(ad => ad.BreedId)
                .IsRequired()
                .HasColumnName("breed_id")
                .HasColumnOrder(19);
        });

        builder.Property(p => p.IsDeleted)
            .IsRequired()
            .HasColumnName("is_deleted")
            .HasColumnOrder(21);
        
        builder.Property(p => p.DeletionDate)
            .IsRequired(false)
            .HasColumnName("deletion_date")
            .HasColumnOrder(22);
    }
}