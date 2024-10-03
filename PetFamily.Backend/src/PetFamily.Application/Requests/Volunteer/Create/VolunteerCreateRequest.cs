using PetFamily.Application.Interfaces.Repositories;
using PetFamily.Domain.VolunteerAggregate.Entities;
using PetFamily.Domain.VolunteerAggregate.ValueObjects;

namespace PetFamily.Application.Requests.Volunteer.Create;

public record VolunteerCreateRequest(
    string FirstName,
    string LastName,
    string Patronymic,
    string Email,
    string? Description,
    byte WorkExperience,
    string PhoneNumber,
    IEnumerable<SocialNetwork>? SocialNetworks,
    IEnumerable<Requisite>? Requisites,
    IEnumerable<Pet>? Pets);
    