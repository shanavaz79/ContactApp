using Ev.Service.Contacts.Dto;
using Ev.Service.Contacts.Logs;
using Ev.Service.Contacts.Models;
using Ev.Service.Contacts.Repository;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ev.Service.Contacts.Managers
{
    public class ContactsManager : IContactsManager
    {
        private readonly IGenericRepository<Models.Contact> contactsRepository;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactsManager"/> class.
        /// </summary>
        /// <param name="contactsRepository">Contacts Repository.</param>
        /// <param name="logger">Logger.</param>
        public ContactsManager(IGenericRepository<Models.Contact> contactsRepository, IInfoLog logger)
        {
            this.contactsRepository = contactsRepository;
            this.logger = logger?.GetInstance();
        }

        public async Task<ApiResponseDto> AddContact(PostContactDto contact)
        {
            var response = new ApiResponseDto();
            await this.ParametersCheckForContact(contact, response).ConfigureAwait(false);

            if (response.Errors.Count != 0)
            {
                response.Code = response.Errors.Count > 1 ? ApiResponseCode.MultipleErrors : response.Errors[0].Code;
                return response;
            }

            var contactEntity = new Models.Contact
            {
                FirstName = contact.FirstName,
                Email = contact.Email,
                LastName = contact.LastName,
                PhoneNumber = contact.PhoneNumber,
                Status = (ContactStatus)Enum.Parse(typeof(ContactStatus), contact.Status.Trim(), true)
            };

            this.contactsRepository.Insert(contactEntity);
            await this.contactsRepository.SaveAsync().ConfigureAwait(false);
            return new ApiResponseDto
            {
                Data = this.AssembleContactsDto(contactEntity),
                Code = ApiResponseCode.Success,
            };
        }

        private static ApiResponseDto CreateErrorResponse(ApiResponseCode errorCode, string errorMessage, ApiResponseDto errorResponse)
        {
            var errorDto = new ErrorDto
            {
                Code = errorCode,
                Error = errorMessage,
            };

            errorResponse.Code = errorCode;
            errorResponse.Errors.Add(errorDto);
            return errorResponse;
        }

        private async Task ParametersCheckForContact(PostContactDto contact, ApiResponseDto responseDto)
        {
            if (contact == null)
            {
                CreateErrorResponse(ApiResponseCode.MissingMandatoryInformation, "Contact cannot be null.", responseDto);
                return;
            }
            if (!Enum.TryParse(contact.Status.Trim(), true, out ContactStatus contactStatus))
            {
                CreateErrorResponse(ApiResponseCode.InvalidData, "Contact Status should be Active or Inactive.", responseDto);
            }
            if (string.IsNullOrEmpty(contact.Email?.Trim()))
            {
                CreateErrorResponse(ApiResponseCode.MissingMandatoryInformation, "Contact Email cannot be null.", responseDto);
            }
            else
            {
                var contactExits = await this.contactsRepository.GetAsync(x => x.Email == contact.Email).ConfigureAwait(false);

                if (contactExits.Any())
                {
                    CreateErrorResponse(ApiResponseCode.DuplicateData, "Contact with same Email already exits.", responseDto);
                }
            }
        }

        private void ParametersCheckForContact(PutContactDto contact, ApiResponseDto responseDto)
        {
            if (contact == null)
            {
                CreateErrorResponse(ApiResponseCode.MissingMandatoryInformation, "Contact cannot be null.", responseDto);
                return;
            }
            if (!Enum.TryParse(contact.Status.Trim(), true, out ContactStatus contactStatus))
            {
                CreateErrorResponse(ApiResponseCode.InvalidData, "Contact Status should be Active or Inactive.", responseDto);
            }
            if (string.IsNullOrEmpty(contact.Email?.Trim()))
            {
                CreateErrorResponse(ApiResponseCode.MissingMandatoryInformation, "Contact Email cannot be null.", responseDto);
            }
        }

        public async Task<ApiResponseDto> GetByKeyAsync(int contactId)
        {
            var apiResponseDto = new ApiResponseDto();
            if (contactId <= 0)
            {
                apiResponseDto.Code = ApiResponseCode.MissingMandatoryInformation;
                apiResponseDto.Errors.Add(new ErrorDto { Error = $"Contact Id: '{contactId}' is invalid." });
                return apiResponseDto;
            }

            var contacts = await this.contactsRepository.GetAsync(x => x.Id == contactId).ConfigureAwait(false);
            if (contacts.Any())
            {
                apiResponseDto.Code = ApiResponseCode.Success;
                apiResponseDto.Data = this.AssembleContactsDto(contacts.First());
                return apiResponseDto;
            }

            apiResponseDto.Code = ApiResponseCode.NoDataFound;
            apiResponseDto.Errors.Add(new ErrorDto { Error = $"Contact with key: '{contactId}' not found." });
            return apiResponseDto;
        }

        /// <summary>
        /// Get Contacts.
        /// </summary>
        /// <param name="status">Status.</param>
        /// <param name="limit">Limit.</param>
        /// <param name="offset">Offset.</param>
        /// <returns>ApiResponseDto.</returns>
        public ApiResponseDto GetContacts(string status, int? limit, int? offset)
        {
            var responseDto = new ApiResponseDto();

            IEnumerable<Models.Contact> contacts = this.contactsRepository.GetAll();
            var filteredData = contacts;
            if (!string.IsNullOrEmpty(status))
                if (Enum.TryParse(status.Trim(), true, out ContactStatus contactStatus))
                {
                    filteredData = contacts.Where(x => x.Status == contactStatus);
                }
                else
                {
                    responseDto.Code = ApiResponseCode.InvalidData;
                    responseDto.Errors.Add(new ErrorDto { Error = $"status : '{status}' is invalid." });
                    return responseDto;
                }
            var orderedData = filteredData.Select(this.AssembleContactsDto).OrderBy(x => x.FirstName);
            var skippedData = offset.HasValue ? orderedData.Skip(offset.Value) : orderedData;
            var finalData = limit.HasValue ? skippedData.Take(limit.Value) : skippedData;
            responseDto.Code = ApiResponseCode.Success;
            responseDto.Data = finalData;
            return responseDto;
        }

        private ContactsDto AssembleContactsDto(Models.Contact contact)
        {
            return new ContactsDto { Status = contact.Status.ToString(), Email = contact.Email, FirstName = contact.FirstName, Id = contact.Id, LastName = contact.LastName, PhoneNumber = contact.PhoneNumber };
        }

        public async Task<ApiResponseDto> UpdateContactAsync(PutContactDto contact)
        {
            var response = new ApiResponseDto();
            this.ParametersCheckForContact(contact, response);

            if (response.Errors.Any())
            {
                response.Code = response.Errors.Count > 1 ? ApiResponseCode.MultipleErrors : response.Errors[0].Code;
                return response;
            }
            var dbContact = await this.contactsRepository.GetByIdAsync(contact.Id);

            dbContact.FirstName = contact.FirstName;
            dbContact.Email = contact.Email;
            dbContact.LastName = contact.LastName;
            dbContact.PhoneNumber = contact.PhoneNumber;
            dbContact.Status = (ContactStatus)Enum.Parse(typeof(ContactStatus), contact.Status.Trim(), true);

            this.contactsRepository.Update(dbContact);
            await this.contactsRepository.SaveAsync().ConfigureAwait(false);
            return new ApiResponseDto
            {
                Data = this.AssembleContactsDto(dbContact),
                Code = ApiResponseCode.Success,
            };
        }

        public async Task<ApiResponseDto> PatchContact(PatchContactDto patchContactDto)
        {
            var response = new ApiResponseDto();

            if (patchContactDto is null)
            {
                response.Code = ApiResponseCode.MissingMandatoryInformation;
                response.Errors.Add(new ErrorDto { Error = "PATCH contact info is missing.", Code = ApiResponseCode.MissingMandatoryInformation });

                return response;
            }
            if (!Enum.TryParse(patchContactDto.Status.Trim(), true, out ContactStatus contactStatus))
            {
                response.Code = ApiResponseCode.InvalidData;
                response.Errors.Add(new ErrorDto { Error = "PATCH Contact Status should be Active or Inactive.", Code = ApiResponseCode.InvalidData });

                return response;
            }
            var contactToUpdate = await this.contactsRepository.GetByIdAsync(patchContactDto.Id).ConfigureAwait(false);
            if (contactToUpdate is null)
            {
                this.logger.Error($"No records exist for contact id: {patchContactDto.Id}");
                response.Errors.Add(new ErrorDto { Error = $"No records exist for contact id: {patchContactDto.Id}", Code = ApiResponseCode.NoDataFound });
                return response;
            }

            contactToUpdate.Status = (ContactStatus)Enum.Parse(typeof(ContactStatus), patchContactDto.Status.Trim(), true);
            this.contactsRepository.Update(contactToUpdate);

            response.Data = this.AssembleContactsDto(contactToUpdate);

            await this.contactsRepository.SaveAsync().ConfigureAwait(false);

            return response;
        }
    }
}
