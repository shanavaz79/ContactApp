

namespace Ev.Service.Contacts.Repository
{
    using Ev.Service.Contacts.Logs;
    using Microsoft.EntityFrameworkCore;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ContactDBContext : DbContext
    {
        public ContactDBContext()
        {
        }

        public ContactDBContext(DbContextOptions<ContactDBContext> options) : base(options)
        {
        }

        public virtual DbSet<Models.Contacts> Contacts { get; set; }
    }
}
