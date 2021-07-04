

namespace Ev.Service.Contacts.Tests
{
    using Moq;
    using Serilog;
    using Xunit;
    using System;
    using Ev.Service.Contacts.Dto;
    using Microsoft.AspNetCore.Mvc;
    using Ev.Service.Contacts.Managers;
    using Ev.Service.Contacts.Logs;
    using Ev.Service.Contacts.Models;
    using System.Collections.Generic;
    using Ev.Service.Contacts.Repository;

    public class ContactManagerTests
    {
        [Fact]
        public void Get_Contact_Returns_Success()
        {
            var mockedContactRepository = new Mock<IGenericRepository<Contact>>();
            var mockedLogger = new Mock<ILogger>();
            var mockInfoLog = new Mock<IInfoLog>();
            mockInfoLog.Setup(x => x.GetInstance()).Returns(mockedLogger.Object);

            var contact = new List<Contact>
            {
                new Contact(),
            };
            mockedContactRepository.Setup(x => x.GetAll()).Returns(contact);

            var contactManager = new ContactsManager(mockedContactRepository.Object, mockInfoLog.Object);
            var response = contactManager.GetContacts(null, null, null);

            mockedContactRepository.Verify(x => x.GetAll(), Times.Once);
            Assert.Equal(ApiResponseCode.Success, response.Code);
        }
    }
}
