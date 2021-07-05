

namespace Ev.Service.Contacts.Tests
{
    using Moq;
    using Serilog;
    using Xunit;
    using Ev.Service.Contacts.Dto;
    using Microsoft.AspNetCore.Mvc;
    using Ev.Service.Contacts.Managers;
    using Ev.Service.Contacts.Logs;
    using Ev.Service.Contacts.Models;
    using System.Collections.Generic;

    public class ContactsControllerTests
    {
        [Fact]
        public void Get_Contact_Returns_Success()
        {
            var mockedContactManager = new Mock<IContactsManager>();
            var mockedLogger = new Mock<ILogger>();
            var mockInfoLog = new Mock<IInfoLog>();
            var mockAuditLog = new Mock<IAuditLog>();
            mockInfoLog.Setup(x => x.GetInstance()).Returns(mockedLogger.Object);
            mockAuditLog.Setup(x => x.GetInstance()).Returns(mockedLogger.Object);

            var contacts = new List<Contact>
            {
                new Contact(),
            };
            mockedContactManager.Setup(x => x.GetContacts(null, null, null)).Returns(new ApiResponseDto { Code = ApiResponseCode.Success, Data = contacts });

            var capabilitiesController = new ContactsController(mockedContactManager.Object, mockInfoLog.Object, mockAuditLog.Object);
            var response = capabilitiesController.Get(null, null, null) as OkObjectResult;
            var responseDto = response.Value as ApiResponseDto;

            mockedContactManager.Verify(x => x.GetContacts(null, null, null), Times.Once);
            Assert.Equal(ApiResponseCode.Success, responseDto.Code);
            Assert.Equal(contacts, responseDto.Data);
            Assert.Empty(responseDto.Errors);
        }
    }
}
