

namespace Ev.Service.Contacts.Repository
{
    using Microsoft.EntityFrameworkCore;

    public class ContactDBContext : DbContext
    {
        public ContactDBContext()
        {
        }

        public ContactDBContext(DbContextOptions<ContactDBContext> options) : base(options)
        {
        }

        public virtual DbSet<Models.Contact> Contacts { get; set; }
    }
}
