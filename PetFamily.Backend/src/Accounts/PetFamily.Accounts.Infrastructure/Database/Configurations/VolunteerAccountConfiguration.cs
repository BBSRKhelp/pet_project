using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Accounts.Contracts.DTOs;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Accounts.Domain.ValueObjects;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel;

namespace PetFamily.Accounts.Infrastructure.Database.Configurations;

public class VolunteerAccountConfiguration : IEntityTypeConfiguration<VolunteerAccount>
{
    public void Configure(EntityTypeBuilder<VolunteerAccount> builder)
    {
        builder.ToTable("volunteer_accounts");
        
        builder.HasKey(va => va.Id);
        
        builder.ComplexProperty(v => v.Description, db =>
        {
            db.Property(d => d.Value)
                .IsRequired(false)
                .HasColumnName("description")
                .HasMaxLength(Constants.MAX_MEDIUM_TEXT_LENGTH)
                .HasColumnOrder(2);
        });
        
        builder.ComplexProperty(v => v.WorkExperience, web =>
        {
            web.Property(we => we.Value)
                .IsRequired()
                .HasColumnName("work_experience")
                .HasMaxLength(WorkExperience.MAX_NUMBER)
                .HasColumnOrder(3);;
        });

        builder.Property(v => v.Requisites)
            .IsRequired()
            .ValueObjectsCollectionJsonConversion(
                r => new RequisiteDto(r.Title, r.Description),
                dto => Requisite.Create(dto.Title, dto.Description).Value)
            .HasColumnOrder(4);
        
        builder.Property(v => v.Certificates)
            .IsRequired()
            .ValueObjectsCollectionJsonConversion(
                c => new CertificateDto(c.Name, c.Url, c.IssueDate),
                dto => Certificate.Create(dto.Name, dto.Url, dto.IssueDate).Value)
            .HasColumnOrder(5);
        
        builder.HasOne(va => va.User)
            .WithOne(u => u.VolunteerAccount)
            .HasForeignKey<VolunteerAccount>(va => va.UserId);
    }
}