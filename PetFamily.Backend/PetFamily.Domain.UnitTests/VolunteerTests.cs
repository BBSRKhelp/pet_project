using FluentAssertions;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.SpeciesAggregate.ValueObjects.Ids;
using PetFamily.Domain.VolunteerAggregate;
using PetFamily.Domain.VolunteerAggregate.Entities;
using PetFamily.Domain.VolunteerAggregate.Enums;
using PetFamily.Domain.VolunteerAggregate.ValueObjects;

namespace PetFamily.Domain.UnitTests;

public class VolunteerTests
{
    [Fact]
    public void AddPet_WithEmptyPets_ShouldSuccessResult()
    {
        //Arrange
        var sut = CreateVolunteer();
        var pet = CreatePet();

        //Act
        var result = sut.AddPet(pet);

        //Assert
        var addedPetResult = sut.GetPetById(pet.Id);

        result.IsSuccess.Should().BeTrue();
        addedPetResult.IsSuccess.Should().BeTrue();
        addedPetResult.Value.Id.Should().Be(pet.Id);
        addedPetResult.Value.Position.Value.Should().Be(1);
    }

    [Fact]
    public void AddPet_WithOtherPets_ShouldSuccessResult()
    {
        //Arrange
        const int PETS_COUNT = 5;

        var sut = CreateVolunteerWithPets(PETS_COUNT);
        var petToAdd = CreatePet();

        //Act
        var result = sut.AddPet(petToAdd);

        //Assert
        var addedPetResult = sut.GetPetById(petToAdd.Id);

        var serialNumber = Position.Create(PETS_COUNT + 1).Value;

        result.IsSuccess.Should().BeTrue();
        addedPetResult.IsSuccess.Should().BeTrue();
        addedPetResult.Value.Id.Should().Be(petToAdd.Id);
        addedPetResult.Value.Position.Should().Be(serialNumber);
    }

    [Fact]
    public void MovePet_WhenPetAlreadyAtNewPosition_ShouldNotMove()
    {
        //Arrange
        const int PETS_COUNT = 6;

        var sut = CreateVolunteerWithPets(PETS_COUNT);

        var secondPosition = Position.Create(2).Value;

        var firstPet = sut.Pets[0];
        var secondPet = sut.Pets[1];
        var thirdPet = sut.Pets[2];
        var fourthPet = sut.Pets[3];
        var fifthPet = sut.Pets[4];
        var sixthPet = sut.Pets[5];

        //Act
        var result = sut.MovePet(secondPet, secondPosition);

        //Assert
        result.IsSuccess.Should().BeTrue();
        firstPet.Position.Value.Should().Be(1);
        secondPet.Position.Value.Should().Be(2);
        thirdPet.Position.Value.Should().Be(3);
        fourthPet.Position.Value.Should().Be(4);
        fifthPet.Position.Value.Should().Be(5);
        sixthPet.Position.Value.Should().Be(6);
    }

    [Fact]
    public void MovePet_WhenNewPositionIsLower_ShouldMoveOtherPetsForward()
    {
        //Arrange
        const int PETS_COUNT = 6;

        var sut = CreateVolunteerWithPets(PETS_COUNT);

        var secondPosition = Position.Create(2).Value;

        var firstPet = sut.Pets[0];
        var secondPet = sut.Pets[1];
        var thirdPet = sut.Pets[2];
        var fourthPet = sut.Pets[3];
        var fifthPet = sut.Pets[4];
        var sixthPet = sut.Pets[5];

        //Act
        var result = sut.MovePet(fifthPet, secondPosition);

        //Assert
        result.IsSuccess.Should().BeTrue();
        firstPet.Position.Value.Should().Be(1);
        secondPet.Position.Value.Should().Be(3);
        thirdPet.Position.Value.Should().Be(4);
        fourthPet.Position.Value.Should().Be(5);
        fifthPet.Position.Value.Should().Be(2);
        sixthPet.Position.Value.Should().Be(6);
    }

    [Fact]
    public void MovePet_WhenNewPositionIsFirst_ShouldMoveOtherPetsForward()
    {
        //Arrange
        const int PETS_COUNT = 6;

        var sut = CreateVolunteerWithPets(PETS_COUNT);

        var firstPosition = Position.Create(1).Value;

        var firstPet = sut.Pets[0];
        var secondPet = sut.Pets[1];
        var thirdPet = sut.Pets[2];
        var fourthPet = sut.Pets[3];
        var fifthPet = sut.Pets[4];
        var sixthPet = sut.Pets[5];

        //Act
        var result = sut.MovePet(sixthPet, firstPosition);

        //Assert
        result.IsSuccess.Should().BeTrue();
        firstPet.Position.Value.Should().Be(2);
        secondPet.Position.Value.Should().Be(3);
        thirdPet.Position.Value.Should().Be(4);
        fourthPet.Position.Value.Should().Be(5);
        fifthPet.Position.Value.Should().Be(6);
        sixthPet.Position.Value.Should().Be(1);
    }

    [Fact]
    public void MovePet_WhenNewPositionIsGreater_ShouldMoveOtherPetsBack()
    {
        //Arrange
        const int PETS_COUNT = 6;

        var sut = CreateVolunteerWithPets(PETS_COUNT);

        var fifthPosition = Position.Create(5).Value;

        var firstPet = sut.Pets[0];
        var secondPet = sut.Pets[1];
        var thirdPet = sut.Pets[2];
        var fourthPet = sut.Pets[3];
        var fifthPet = sut.Pets[4];
        var sixthPet = sut.Pets[5];

        //Act
        var result = sut.MovePet(secondPet, fifthPosition);

        //Assert
        result.IsSuccess.Should().BeTrue();
        firstPet.Position.Value.Should().Be(1);
        secondPet.Position.Value.Should().Be(5);
        thirdPet.Position.Value.Should().Be(2);
        fourthPet.Position.Value.Should().Be(3);
        fifthPet.Position.Value.Should().Be(4);
        sixthPet.Position.Value.Should().Be(6);
    }

    [Fact]
    public void MovePet_WhenNewPositionIsLast_ShouldMoveOtherPetsBack()
    {
        //Arrange
        const int PETS_COUNT = 6;

        var sut = CreateVolunteerWithPets(PETS_COUNT);

        var sixthPosition = Position.Create(6).Value;

        var firstPet = sut.Pets[0];
        var secondPet = sut.Pets[1];
        var thirdPet = sut.Pets[2];
        var fourthPet = sut.Pets[3];
        var fifthPet = sut.Pets[4];
        var sixthPet = sut.Pets[5];

        //Act
        var result = sut.MovePet(firstPet, sixthPosition);

        //Assert
        result.IsSuccess.Should().BeTrue();
        firstPet.Position.Value.Should().Be(6);
        secondPet.Position.Value.Should().Be(1);
        thirdPet.Position.Value.Should().Be(2);
        fourthPet.Position.Value.Should().Be(3);
        fifthPet.Position.Value.Should().Be(4);
        sixthPet.Position.Value.Should().Be(5);
    }

    [Fact]
    public void MovePet_WhenNewPositionIsOutOfRange_ShouldMoveOtherPetBack()
    {
        //Arrange
        const int PETS_COUNT = 6;

        var sut = CreateVolunteerWithPets(PETS_COUNT);

        var outOfRangePosition = Position.Create(8).Value;

        var firstPet = sut.Pets[0];
        var secondPet = sut.Pets[1];
        var thirdPet = sut.Pets[2];
        var fourthPet = sut.Pets[3];
        var fifthPet = sut.Pets[4];
        var sixthPet = sut.Pets[5];

        //Act
        var result = sut.MovePet(thirdPet, outOfRangePosition);

        //Assert
        result.IsSuccess.Should().BeTrue();
        firstPet.Position.Value.Should().Be(1);
        secondPet.Position.Value.Should().Be(2);
        thirdPet.Position.Value.Should().Be(6);
        fourthPet.Position.Value.Should().Be(3);
        fifthPet.Position.Value.Should().Be(4);
        sixthPet.Position.Value.Should().Be(5);
    }

    private Volunteer CreateVolunteer()
    {
        var fullname = FullName.Create("TestJohn", "TestSmith", "TestPetrov").Value;
        var email = Email.Create("TestJohn@TestSmith.com").Value;
        var description = Description.Create("TestDescription").Value;
        var workExperience = WorkExperience.Create(8).Value;
        var phoneNumber = PhoneNumber.Create("88888888888").Value;
        IReadOnlyList<SocialNetwork> socialNetworks = [SocialNetwork.Create("TestSocialNetwork", "url").Value];
        IReadOnlyList<Requisite> requisites = [Requisite.Create("TestRequisite", "TestRequisiteUrl").Value];

        return new Volunteer(
            fullname,
            email,
            description,
            workExperience,
            phoneNumber,
            socialNetworks,
            requisites);
    }

    private Pet CreatePet()
    {
        var name = Name.Create("TestPet").Value;
        var description = Description.Create("TestDescription").Value;
        var appearanceDetails = AppearanceDetails.Create(Colour.Orange, 10, 100).Value;
        var healthDetails = HealthDetails.Create("test", true, true).Value;
        var address = Address.Create("test", "test", "test", "test").Value;
        var phoneNumber = PhoneNumber.Create("88888888888").Value;
        var birthday = DateTime.Now;
        var status = Status.LookingForHome;
        IReadOnlyList<Requisite> requisites = [Requisite.Create("TestRequisite", "TestRequisiteUrl").Value];
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
            requisites,
            breedAndSpeciesId);
    }

    private Volunteer CreateVolunteerWithPets(int petsCount)
    {
        var volunteer = CreateVolunteer();
        var pets = Enumerable.Range(1, petsCount).Select(_ => CreatePet());

        foreach (var pet in pets)
            volunteer.AddPet(pet);

        return volunteer;
    }
}