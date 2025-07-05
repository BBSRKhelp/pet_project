using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.Core.Messaging;
using PetFamily.Core.Providers;
using PetFamily.Files.Contracts;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.Volunteers.Domain.ValueObjects;
using PetFamily.Volunteers.Application.Interfaces;

namespace PetFamily.Volunteers.Application.Features.Commands.Pet.UploadFilesToPet;

public class UploadFilesToPetHandler : ICommandHandler<Guid, UploadFilesToPetCommand>
{
    private const string BUCKET_NAME = "pet_photos";
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly IFileContract _fileContract;
    private readonly IValidator<UploadFilesToPetCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageQueue<IEnumerable<FileIdentifier>> _messageQueue;
    private readonly ILogger<UploadFilesToPetHandler> _logger;

    public UploadFilesToPetHandler(
        IVolunteersRepository volunteersRepository,
        IFileContract fileContract,
        IValidator<UploadFilesToPetCommand> validator,
        [FromKeyedServices(UnitOfWorkContext.Volunteers)]IUnitOfWork unitOfWork,
        IMessageQueue<IEnumerable<FileIdentifier>> messageQueue,
        ILogger<UploadFilesToPetHandler> logger)
    {
        _volunteersRepository = volunteersRepository;
        _fileContract = fileContract;
        _validator = validator;
        _unitOfWork = unitOfWork;
        _messageQueue = messageQueue;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        UploadFilesToPetCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Uploading files to pet");

        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Upload files to pet failed");
                return validationResult.ToErrorList();
            }

            var volunteerResult = await _volunteersRepository.GetByIdAsync(command.VolunteerId, cancellationToken);
            if (volunteerResult.IsFailure)
            {
                _logger.LogWarning("Upload files to pet failed");
                return (ErrorList)volunteerResult.Error;
            }

            List<FileData> filesData = [];
            foreach (var file in command.Files)
            {
                var photoPath = PhotoPath.Create(file.FileName).Value;

                var fileData = new FileData(file.Stream, new FileIdentifier(photoPath, BUCKET_NAME));

                filesData.Add(fileData);
            }

            var filePathsResult = await _fileContract.UploadFilesAsync(filesData, cancellationToken);
            if (filePathsResult.IsFailure)
            {
                _logger.LogError("Upload files to pet failed");
                _logger.LogInformation("Writing files for cleaning");
                await _messageQueue.WriteAsync(filesData.Select(f => f.FileIdentifier), cancellationToken);
            
                return (ErrorList)filePathsResult.Error;
            }

            var petPhotos = filePathsResult.Value
                    .Select(p => p)
                    .Select(p => new PetPhoto(p)).ToArray();

            var petResult = volunteerResult.Value.GetPetById(command.PetId);
            if (petResult.IsFailure)
            {
                _logger.LogError("Upload files to pet failed");
                return (ErrorList)petResult.Error;
            }

            volunteerResult.Value.AddPetPhotos(petResult.Value, petPhotos);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Success uploaded files to pet with Id = {PetId}", petResult.Value.Id.Value);

            transaction.Commit();

            return petResult.Value.Id.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Upload files to pet failed");

            transaction.Rollback();

            return (ErrorList)Error.Failure("upload.files.error",
                $"upload files to pet with Id = {command.PetId} failed");
        }
    }
}