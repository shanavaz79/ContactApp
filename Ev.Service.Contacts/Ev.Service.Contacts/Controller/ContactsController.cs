using Ev.Service.Contacts.Dto;
using Ev.Service.Contacts.Logs;
using Ev.Service.Contacts.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ev.Service.Contacts
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactsManager contactsManager;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactsController"/> class.
        /// </summary>
        /// <param name="contactsManager">Contacts Manager.</param>
        /// <param name="logger">Logger.</param>
        public ContactsController(IContactsManager contactsManager, IInfoLog logger)
        {
            this.contactsManager = contactsManager;
            this.logger = logger?.GetInstance();
        }

        [HttpGet]
        public IActionResult Get(string status, int? limit, int? offset)
        {
            this.logger.Information($"Get Contacts Api is called with parameters {status}, {limit} and {offset}");

            var response = this.contactsManager.GetContacts(status, limit, offset);

            if (response.Code == ApiResponseCode.Success)
            {
                this.logger.Information($"Get Contacts with parameters {status}, {limit} and {offset} responded with {response}");
                return this.Ok(response);
            }

            this.logger.Error($"Get Contacts with parameters {status}, {limit} and {offset} responded with {response}");
            return this.BadRequest(response);
        }
    }
}
