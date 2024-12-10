using System.Data;
using System.Data.Common;
using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using PetFamily.Application.Commands.Volunteer.UploadFilesToPet;
using PetFamily.Application.Database;
using PetFamily.Application.Dtos;
using PetFamily.Application.Interfaces.Repositories;
using PetFamily.Application.Providers;
using PetFamily.Domain.Shared.Models;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.SpeciesAggregate.ValueObjects.Ids;
using PetFamily.Domain.VolunteerAggregate;
using PetFamily.Domain.VolunteerAggregate.Entities;
using PetFamily.Domain.VolunteerAggregate.Enums;
using PetFamily.Domain.VolunteerAggregate.ValueObjects;
using PetFamily.Domain.VolunteerAggregate.ValueObjects.Shell;
using IFileProvider = PetFamily.Application.Interfaces.Providers.IFileProvider;

namespace PetFamily.Application.UnitTests;

public class UploadFilesToPetTests
{
    [Fact]
    public async Task Handle_ShouldUploadFilesToPet()
    {
        //Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var volunteer = CreateVolunteer();
        var pet = CreatePet();
        volunteer.AddPet(pet);

        const string FILENAME = "test.jpg";
        var stream = new MemoryStream();
        var uploadFileDto = new UploadFileDto(stream, FILENAME);
        List<UploadFileDto> files = [uploadFileDto, uploadFileDto];

        var command = new UploadFilesToPetCommand(volunteer.Id.Value, pet.Id.Value, files);

        //volunteersRepository
        var volunteersRepositoryMock = new Mock<IVolunteersRepository>();
        volunteersRepositoryMock
            .Setup(r => r.GetByIdAsync(volunteer.Id, cancellationToken))
            .ReturnsAsync(Result.Success<Volunteer, Error>(volunteer));

        //fileProvider
        List<PhotoPath> photoPaths =
        [
            PhotoPath.Create("jpg").Value,
            PhotoPath.Create("jpg").Value
        ];

        var fileProviderMock = new Mock<IFileProvider>();
        fileProviderMock
            .Setup(p => p.UploadFilesAsync(It.IsAny<IEnumerable<FileData>>(), cancellationToken))
            .ReturnsAsync(Result.Success<IReadOnlyList<PhotoPath>, Error>(photoPaths));

        //validator
        var validatorMock = new Mock<IValidator<UploadFilesToPetCommand>>();
        validatorMock
            .Setup(v => v.ValidateAsync(command, cancellationToken))
            .ReturnsAsync(new ValidationResult());
        
        //unitOfWork
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        
        unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        unitOfWorkMock
            .Setup(u => u.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(new Mock<IDbTransaction>().Object);
        
        //logger
        var logger = LoggerFactory
            .Create(builder => builder.AddConsole())
            .CreateLogger<UploadFilesToPetHandler>();
        
        //createHandler
        var handler = new UploadFilesToPetHandler(
            volunteersRepositoryMock.Object,
            fileProviderMock.Object,
            validatorMock.Object,
            unitOfWorkMock.Object,
            logger);

        //Act
        var result = await handler.HandleAsync(command, cancellationToken);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(pet.Id.Value);
    }

    private Volunteer CreateVolunteer()
    {
        var fullName = FullName.Create("firstName", "lastName").Value;
        var email = Email.Create("email@email.com").Value;
        var description = Description.Create("description").Value;
        var workExperience = WorkExperience.Create(2).Value;
        var phoneNumber = PhoneNumber.Create("89166988888").Value;
        var socialNetworkShell = new SocialNetworksShell([SocialNetwork.Create("title", "url").Value]);
        var requisiteShell = new RequisitesShell([Requisite.Create("title", "url").Value]);

        return new Volunteer(
            fullName,
            email,
            description,
            workExperience,
            phoneNumber,
            socialNetworkShell,
            requisiteShell);
    }

    private Pet CreatePet()
    {
        var name = Name.Create("TestPet").Value;
        var description = Description.Create("TestDescription").Value;
        var appearanceDetails = AppearanceDetails.Create(Colour.Orange, 10, 100).Value;
        var healthDetails = HealthDetails.Create("test", true, true).Value;
        var address = Address.Create("test", "test", "test", "test").Value;
        var phoneNumber = PhoneNumber.Create("88888888888").Value;
        var birthday = DateOnly.Parse("2015-01-01");
        var status = StatusForHelp.NeedsHelp;
        var petPhotos = new PetPhotosShell([new PetPhoto(PhotoPath.Create(".png").Value)]);
        var requisites = new RequisitesShell([Requisite.Create("TestRequisite", "TestRequisiteUrl").Value]);
        var breedAndSpeciesId = BreedAndSpeciesId.Create(SpeciesId.NewId(), Guid.NewGuid()).Value;

        return new Pet(
            name,
            description,
            appearanceDetails,
            healthDetails,
            address,
            phoneNumber,
            birthday,
            status,
            petPhotos,
            requisites,
            breedAndSpeciesId);
    }
}