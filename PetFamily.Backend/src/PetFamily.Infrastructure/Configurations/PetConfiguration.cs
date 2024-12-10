using PetFamily.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.SpeciesAggregate.ValueObjects.Ids;
using PetFamily.Domain.VolunteerAggregate.Entities;
using PetFamily.Domain.VolunteerAggregate.Enums;
using PetFamily.Domain.VolunteerAggregate.ValueObjects;
using PetFamily.Domain.VolunteerAggregate.ValueObjects.Ids;

namespace PetFamily.Infrastructure.Configurations;

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

        builder.OwnsOne(p => p.Name, nb =>
        {
            nb.Property(n => n.Value)
                .IsRequired(false)
                .HasColumnName("name")
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH)
                .HasColumnOrder(2);
        });

        builder.OwnsOne(p => p.Description, db =>
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

        builder.ComplexProperty(p => p.HealthDetails, hdb =>
        {
            hdb.Property(hd => hd.HealthInformation)
                .IsRequired()
                .HasColumnName("health_information")
                .HasMaxLength(Constants.MAX_MEDIUM_LOW_TEXT_LENGTH)
                .HasColumnOrder(14);

            hdb.Property(hd => hd.IsCastrated)
                .IsRequired()
                .HasColumnName("is_castrated")
                .HasDefaultValue(false)
                .HasColumnOrder(15);

            hdb.Property(hd => hd.IsVaccinated)
                .IsRequired()
                .HasColumnName("is_vaccinated")
                .HasDefaultValue(false)
                .HasColumnOrder(16);
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

            ab.Property(p => p.Postalcode)
                .IsRequired(false)
                .HasColumnName("postalcode")
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH)
                .HasColumnOrder(10);
        });

        builder.OwnsOne(p => p.PhoneNumber, pb =>
        {
            pb.Property(pn => pn.Value)
                .IsRequired()
                .HasColumnName("phone_number")
                .HasMaxLength(PhoneNumber.MAX_LENGTH)
                .HasColumnOrder(11);
        });

        builder.Property(p => p.Birthday)
            .IsRequired(false)
            .HasColumnOrder(12);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasDefaultValue(StatusForHelp.Unknown)
            .HasColumnOrder(13);

        builder.ComplexProperty(p => p.BreedAndSpeciesId, bsb =>
        {
            bsb.Property(ad => ad.SpeciesId)
                .IsRequired()
                .HasColumnName("species_id")
                .HasConversion(
                    id => id.Value,
                    value => SpeciesId.Create(value));

            bsb.Property(ad => ad.BreedId)
                .IsRequired()
                .HasColumnName("breed_id");
        });

        builder.OwnsOne(p => p.Requisites, requisiteBuilder =>
        {
            requisiteBuilder.ToJson("requisite");

            requisiteBuilder.OwnsMany(r => r.Requisites, rb =>
            {
                rb.Property(r => r.Title)
                    .IsRequired()
                    .HasMaxLength(Constants.MAX_VERY_LOW_TEXT_LENGTH);

                rb.Property(r => r.Description)
                    .IsRequired()
                    .HasMaxLength(Constants.MAX_MEDIUM_LOW_TEXT_LENGTH);
            });
        });

        builder.OwnsOne(p => p.PetPhotos, petPhotoBuilder =>
        {
            petPhotoBuilder.ToJson("pet_photo");

            petPhotoBuilder.OwnsMany(pp => pp.PetPhotos, ppb =>
            {
                ppb.Property(pp => pp.IsMainPhoto)
                    .IsRequired();
                
                ppb.Property(pp => pp.Path)
                    .IsRequired()
                    .HasConversion(
                        p => p.Path,
                        value => PhotoPath.Create(value).Value)
                    .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH);
            });
        });

        builder.OwnsOne(p => p.Position, snb =>
        {
            snb.Property(sn => sn.Value)
                .IsRequired()
                .HasColumnName("serial_number");
        });

        builder.Property<bool>("_isDeleted")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("is_deleted");
    }
}