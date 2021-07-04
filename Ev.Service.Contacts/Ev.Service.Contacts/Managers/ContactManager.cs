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

            Models.Contact contactEntity;

            if (contact != null)
            {
                contactEntity = new Models.Contact
                {
                    FirstName = contact.FirstName,
                    Email = contact.Email,
                    LastName = contact.LastName,
                    PhoneNumber = contact.PhoneNumber,
                    Status = (ContactStatus)Enum.Parse(typeof(ContactStatus), contact.Status.Trim(), true)
                };
            }
            else
            {
                return CreateErrorResponse(ApiResponseCode.MissingMandatoryInformation, "Contact cannot be null.", response);
            }

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
    }
}
