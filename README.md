# PetFamily API

## Usage

The PetFamily API provides endpoints for managing species, breeds, volunteers, and their associated pets. Here's a summary of the available endpoints:

### Species

- `POST /species`: Create a new species
- `POST /species/{id}/breeds`: Add a new breed to a species
- `DELETE /species/{id}`: Delete a species
- `DELETE /species/{speciesId}/breeds/{breedId}`: Delete a breed from a species
- `GET /species`: Get a paginated list of filtered species
- `GET /species/{speciesId}/breeds`: Get a paginated list of breeds for a specific species

### Volunteers

- `POST /volunteers`: Create a new volunteer
- `PUT /volunteers/{id}/main-info`: Update a volunteer's main information
- `PUT /volunteers/{id}/requisites`: Update a volunteer's requisites
- `PUT /volunteers/{id}/social-networks`: Update a volunteer's social networks
- `DELETE /volunteers/{id}`: Delete a volunteer
- `POST /volunteers/{id}/pets`: Add a new pet to a volunteer
- `POST /volunteers/{volunteerId}/pets/{petId}/photos`: Upload files to a pet
- `PUT /volunteers/{volunteerId}/pets/{petId}/position`: Change the position of a pet
- `GET /volunteers`: Get a paginated list of filtered volunteers
- `GET /volunteers/{id}`: Get a volunteer by ID
- `PUT /volunteers/{volunteerId}/pets/{petId}/main-info`: Update a pet's main information
- `PUT /volunteers/{volunteerId}/pets/{petId}/status`: Update a pet's status
- `DELETE /volunteers/{volunteerId}/pets/{petId}/soft`: Soft delete a pet
- `DELETE /volunteers/{volunteerId}/pets/{petId}/hard`: Hard delete a pet
- `PUT /volunteers/{volunteerId}/pets/{petId}/main-photo`: Set the main photo for a pet

## API

### Species Endpoints

#### Create Species
- **Endpoint**: `POST /species`
- **Request Body**:
  ```
  {
    Name : string
  }
  ```
- **Response**: `Guid` (ID of the created species)

#### Add Breed
- **Endpoint**: `POST /species/{id}/breeds`
- **Request Body**:
  ```
  {
    Name: string
  }
  ```
- **Response**: `Guid` (ID of the created breed)


#### Delete Species
- **Endpoint**: `DELETE /species/{id}`
- **Response**: `Boolean` (Indicates whether the species was deleted successfully)

#### Delete Breed
- **Endpoint**: `DELETE /species/{speciesId}/breeds/{breedId}`
- **Response**: `Boolean` (Indicates whether the breed was deleted successfully)

#### Get Filtered Species
- **Endpoint**: `GET /species`
- **Query Parameters**:
  ```
  {
    PageNumber : integer
    PageSize : integer
    Name : Nullable<string>
    SortBy : Nullable<string>
    SortDirection : Nullable<string>
  }
  ```
- **Response**: `PagedList<SpeciesDto>`

#### Get Breeds by Species ID
- **Endpoint**: `GET /species/{speciesId}/breeds`
- **Query Parameters**:
  ```
  {
    PageNumber : integer
    PageSize : integer
    Name : Nullable<string>
    SortBy : Nullable<string>
    SortDirection : Nullable<string>
  }
  ```
- **Response**: `PagedList<BreedDto>`

### Volunteers Endpoints

#### Create Volunteer
- **Endpoint**: `POST /volunteers`
- **Request Body**:
  ```
  {
    FirstName : string
    LastName : string
    Patronymic : Nullable<string>
    Email : string
    Description : Nullable<string>
    WorkExperience : byte
    PhoneNumber : string
    SocialNetworks : Nullable<SocialNetworkDto[]>
    Requisites : Nullable<RequisiteDto[]>
  }
  ```
- **Response**: `Guid` (ID of the created volunteer)

#### Update Volunteer Main Info
- **Endpoint**: `PUT /volunteers/{id}/main-info`
- **Request Body**:
  ```
    {
      FirstName : string
      LastName : string
      Patronymic : Nullable<string>
      Email : string
      Description : Nullable<string>
      WorkExperience : byte
      PhoneNumber : string
    }
  ```
- **Response**: `Guid` (ID of the updated volunteer)

#### Update Volunteer Requisites
- **Endpoint**: `PUT /volunteers/{id}/requisites`
- **Request Body**:
  ```
  {
    Requisites : Nullable<RequisiteDto[]>
  }
  ```
- **Response**: `Guid` (ID of the updated volunteer)

#### Update Volunteer Social Networks
- **Endpoint**: `PUT /volunteers/{id}/social-networks`
- **Request Body**: 
  ```
  {
    SocialNetworks : Nullable<SocialNetworkDto[]>
  }
  ```
- **Response**: `Guid` (ID of the updated volunteer)

#### Delete Volunteer
- **Endpoint**: `DELETE /volunteers/{id}`
- **Response**: `Boolean` (Indicates whether the volunteer was deleted successfully)

#### Add Pet
- **Endpoint**: `POST /volunteers/{id}/pets`
- **Request Body**:
  ```
  {
    Name : Nullable<string>
    Description : Nullable<string>
    Coloration : string
    Weight : float
    Height : float
    HealthInformation : string
    IsCastrated : bool
    IsVaccinated : bool
    Country : string
    City : string
    Street : string
    Postalcode : string
    PhoneNumber : string
    Birthday : Nullable<DateTime>
    Status : string
    Requisites : Nullable<RequisiteDto[]>
    SpeciesId : Guid
    BreedId : Guid
  }
  ```
- **Response**: `Guid` (ID of the created pet)

#### Upload Files to Pet
- **Endpoint**: `POST /volunteers/{volunteerId}/pets/{petId}/photos`
- **Request Body**: `IFormFileCollection`
- **Response**: `Guid` (ID of the uploaded files)

#### Change Pet Position
- **Endpoint**: `PUT /volunteers/{volunteerId}/pets/{petId}/position`
- **Request Body**:
  ```
  {
    NewPosition : integer
  }
  ```
- **Response**: `void`

#### Get Filtered Volunteers With Pagination
- **Endpoint**: `GET /volunteers`
- **Query Parameters**:
  ```
  {
    PageNumber : integer
    PageSize : integer
    FirstName : Nullable<string>
    LastName : Nullable<string>
    Patronymic : Nullable<string>
    WorkExperience : Nullable<byte>
    SortBy : Nullable<string>
    SortDirection : Nullable<string>
  }
  ```
- **Response**: `PagedList<VolunteerDto>`

#### Get Volunteer by ID
- **Endpoint**: `GET /volunteers/{id}`
- **Response**: `VolunteerDto`

#### Update Pet Main Info
- **Endpoint**: `PUT /volunteers/{volunteerId}/pets/{petId}/main-info`
- **Request Body**: 
  ```
  {
    Name : Nullable<string>
    Description : Nullable<string>
    Coloration : string
    Weight : float
    Height : float
    Country : string
    City : string
    Street : string
    Postalcode : string
    PhoneNumber : string
    Birthday : Nullable<DateTime>
    HealthInformation : string
    IsCastrated : bool
    IsVaccinated : bool
    Requisites : Nullable<RequisiteDto[]>
    SpeciesId : Guid
    BreedId : Guid
  }
  ```
- **Response**: `Guid` (ID of the updated pet)

#### Update Pet Status
- **Endpoint**: `PUT /volunteers/{volunteerId}/pets/{petId}/status`
- **Request Body**:
  ```
  {
    Status : string
  }
  ```
- **Response**: `Guid` (ID of the updated pet)

#### Soft Delete Pet
- **Endpoint**: `DELETE /volunteers/{volunteerId}/pets/{petId}/soft`
- **Response**: `Boolean` (Indicates whether the pet was soft deleted successfully)

#### Hard Delete Pet
- **Endpoint**: `DELETE /volunteers/{volunteerId}/pets/{petId}/hard`
- **Response**: `Boolean` (Indicates whether the pet was hard deleted successfully)

#### Set Main Pet Photo
- **Endpoint**: `PUT /volunteers/{volunteerId}/pets/{petId}/main-photo`
- **Request Body**:
  ```
  {
    PhotoPath : string
  }
  ```
- **Response**: `Boolean` (Indicates whether the main photo was set successfully)
