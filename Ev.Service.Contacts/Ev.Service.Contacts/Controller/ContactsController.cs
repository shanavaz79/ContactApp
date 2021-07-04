using Ev.Service.Contacts.Dto;
using Ev.Service.Contacts.Logs;
using Ev.Service.Contacts.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ev.Service.Contacts
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactsManager contactsManager;
        private readonly ILogger auditLog;
        private readonly ILogger infoLog;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactsController"/> class.
        /// </summary>
        /// <param name="contactsManager">Contacts Manager.</param>
        /// <param name="logger">Logger.</param>
        public ContactsController(IContactsManager contactsManager, IInfoLog logger, IAuditLog auditLog)
        {
            this.contactsManager = contactsManager;
            this.auditLog = auditLog.GetInstance();
            this.infoLog = logger.GetInstance();
        }

        [HttpGet]
        public IActionResult Get(string status, int? limit, int? offset)
        {
            this.infoLog.Information($"Get Contacts Api is called with parameters {status}, {limit} and {offset}");

            var response = this.contactsManager.GetContacts(status, limit, offset);

            if (response.Code == ApiResponseCode.Success)
            {
                this.infoLog.Information($"Get Contacts with parameters {status}, {limit} and {offset} responded with {JsonSerializer.Serialize(response)}");
                return this.Ok(response);
            }

            this.infoLog.Error($"Get Contacts with parameters {status}, {limit} and {offset} responded with {JsonSerializer.Serialize(response)}");
            return this.BadRequest(response);
        }

        [HttpGet]
        [Route("{contactId}")]
        public async Task<IActionResult> GetAsync(int contactId)
        {
            this.infoLog.Information($"Contact GET Api is called with parameter {contactId}");
            var response = await this.contactsManager.GetByKeyAsync(contactId).ConfigureAwait(false);
            if (response.Code == ApiResponseCode.Success)
            {
                this.infoLog.Information($"Contact Get successful for parameter = {contactId} with response = {JsonSerializer.Serialize(response)} ");
                return this.Ok(response);
            }

            this.infoLog.Error($"Contact Get failed for parameter = {contactId} with response = {JsonSerializer.Serialize(response)} ");
            return this.BadRequest(response);
        }

        [HttpPost]
        public async Task<IActionResult> Post(PostContactDto contact)
        {
            this.infoLog.Information($"Contact POST Api called with parameters {JsonSerializer.Serialize(contact)}");
            var response = await this.contactsManager.AddContact(contact).ConfigureAwait(false);
            if (response.Code == ApiResponseCode.Success)
            {
                this.infoLog.Information($"Contact POST successful for parameter = {JsonSerializer.Serialize(contact)} with response = {JsonSerializer.Serialize(response)} ");
                this.auditLog.Information($"Contact POST successful for parameter = {JsonSerializer.Serialize(contact)} with response = {JsonSerializer.Serialize(response)} ");
                return this.Ok(response);
            }

            this.infoLog.Error($"Contact POST failed for parameter = {JsonSerializer.Serialize(contact)} with response = {JsonSerializer.Serialize(response)} ");
            return this.BadRequest(response);
        }

        [HttpPut]
        public async Task<IActionResult> PutAsync(PutContactDto contact)
        {
            this.infoLog.Information("Contact PUT Api called with parameter {0}", JsonSerializer.Serialize(contact));
            var response = await this.contactsManager.UpdateContactAsync(contact).ConfigureAwait(false);
            if (response.Code == ApiResponseCode.Success)
            {
                this.infoLog.Information($"Contact PUT Api successful for parameter = {JsonSerializer.Serialize(contact)} with response = {JsonSerializer.Serialize(response)} ");
                this.auditLog.Information($"Contact PUT Api successful for parameter = {JsonSerializer.Serialize(contact)} with response = {JsonSerializer.Serialize(response)} ");
                return this.Ok(response);
            }

            this.infoLog.Error($"Contact PUT Api failed for parameter = {JsonSerializer.Serialize(contact)} with response = {JsonSerializer.Serialize(response)} ");
            return this.BadRequest(response);

        }

        [HttpPatch]
        public async Task<IActionResult> PatchAsync(PatchContactDto patchContactDto)
        {
            this.infoLog.Information("Contact PATCH Api called with parameter {0}", JsonSerializer.Serialize(patchContactDto));
            var response = await this.contactsManager.PatchContact(patchContactDto).ConfigureAwait(false);
            if (response.Code == ApiResponseCode.Success)
            {
                this.infoLog.Information($"Contact PATCH Api successful for parameter = {JsonSerializer.Serialize(patchContactDto)} with response = {JsonSerializer.Serialize(response)} ");
                this.auditLog.Information($"Contact PATCH Api successful for parameter = {JsonSerializer.Serialize(patchContactDto)} with response = {JsonSerializer.Serialize(response)} ");
                return this.Ok(response);
            }

            this.infoLog.Error($"Contact PATCH Api failed for parameter = {JsonSerializer.Serialize(patchContactDto)} with response = {JsonSerializer.Serialize(response)} ");
            return this.BadRequest(response);
        }
    }
}
