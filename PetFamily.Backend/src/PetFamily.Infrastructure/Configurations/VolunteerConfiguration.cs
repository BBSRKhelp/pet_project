using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregate;
using PetFamily.Domain.VolunteerAggregate.ValueObjects.Ids;

namespace PetFamily.Infrastructure.Configurations;

public class VolunteerConfiguration : IEntityTypeConfiguration<Volunteer>
{
    public void Configure(EntityTypeBuilder<Volunteer> builder)
    {
        builder.ToTable("volunteer");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => VolunteerId.Create(value));

        builder.OwnsOne(v => v.Fullname, fb =>
        {
            fb.Property(f => f.FirstName)
                .IsRequired()
                .HasColumnName("first_name")
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH);
            
            fb.Property(f => f.LastName)
                .IsRequired()
                .HasColumnName("last_name")
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH);
            
            fb.Property(f => f.Patronymic)
                .IsRequired()
                .HasColumnName("patronymic")
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH);
        });

        builder.ComplexProperty(v => v.Email, eb =>
        {
            eb.Property(e => e.Value)
                .IsRequired()
                .HasColumnName("email")
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH);
        });
        
        builder.Property(v => v.Description)
            .IsRequired(false)
            .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH);

        builder.Property(v => v.WorkExperience)
            .IsRequired()
            .HasDefaultValue(0);
        
        builder.ComplexProperty(p => p.PhoneNumber, pb =>
        {
            pb.Property(pn => pn.Value)
                .IsRequired()
                .HasColumnName("phone_number")
                .HasMaxLength(PhoneNumber.MAX_LENGTH);
        });

        builder.OwnsOne(v => v.Details, db =>
        {
            db.ToJson("details");

            db.OwnsMany(r => r.Requisites, rb =>
            {
                rb.Property(r => r.Title)
                    .IsRequired()
                    .HasMaxLength(Constants.MAX_VERY_LOW_TEXT_LENGTH);
                
                rb.Property(r => r.Description)
                    .IsRequired()
                    .HasMaxLength(Constants.MAX_MEDIUM_LOW_TEXT_LENGTH);
            });

            db.OwnsMany(sn => sn.SocialNetworks, snb =>
            {
                snb.Property(sn => sn.Title)
                    .IsRequired()
                    .HasMaxLength(Constants.MAX_VERY_LOW_TEXT_LENGTH);
                
                snb.Property(sn => sn.Url)
                    .IsRequired()
                    .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH);
            });
        });
        
        builder.HasMany(v => v.Pets)
            .WithOne()
            .HasForeignKey("volunteer_id")
            .IsRequired()
            .OnDelete(DeleteBehavior.SetNull);
    }
}