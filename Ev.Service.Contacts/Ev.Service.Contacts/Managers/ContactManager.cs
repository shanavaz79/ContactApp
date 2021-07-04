using Ev.Service.Contacts.Dto;
using Ev.Service.Contacts.Logs;
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
        private readonly IGenericRepository<Models.Contacts> contactsRepository;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactsManager"/> class.
        /// </summary>
        /// <param name="contactsRepository">Contacts Repository.</param>
        /// <param name="logger">Logger.</param>
        public ContactsManager(IGenericRepository<Models.Contacts> contactsRepository, IInfoLog logger)
        {
            this.contactsRepository = contactsRepository;
            this.logger = logger?.GetInstance();
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

            IEnumerable<Models.Contacts> contacts = this.contactsRepository.GetAll();

            var filteredData = string.IsNullOrEmpty(status) ? contacts : contacts.Where(x => x.Status == status);
            var orderedData = filteredData.Select(this.AssembleContactsDto).OrderBy(x => x.FirstName);
            var skippedData = offset.HasValue ? orderedData.Skip(offset.Value) : orderedData;
            var finalData = limit.HasValue ? skippedData.Take(limit.Value) : skippedData;
            responseDto.Code = ApiResponseCode.Success;
            responseDto.Data = finalData;
            return responseDto;
        }

        private ContactsDto AssembleContactsDto(Models.Contacts contact)
        {
            return new ContactsDto { Status = contact.Status, Email = contact.Email, FirstName = contact.FirstName, Id = contact.Id, LastName = contact.LastName, PhoneNumber = contact.PhoneNumber };
        }
    }
}
