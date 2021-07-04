

namespace Ev.Service.Contacts.Tests
{
    using Ev.Service.Contacts.Models;
    using Ev.Service.Contacts.Repository;
    using Moq;
    using Xunit;

    public class GenericRepositoryTests
    {
        [Fact(DisplayName = "Save works fine")]
        public async System.Threading.Tasks.Task Save_testsAsync()
        {
            var context = new Mock<ContactDBContext>();
            var repo = new GenericRepository<Contact>(context.Object);
            await repo.SaveAsync();
            context.Verify(x => x.SaveChangesAsync(default), Times.Once());
        }
    }
}
