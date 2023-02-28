

namespace Ev.Service.Contacts.Repository
{
    using Azure.Identity;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;

    public class ContactDBContext : DbContext
    {
        public IConfiguration Configuration { get; }
        public ContactDBContext(IConfiguration configuration,DbContextOptions<ContactDBContext> options) : base(options)
        {
            Configuration = configuration;
        }

        //public ContactDBContext(DbContextOptions<ContactDBContext> options) : base(options)
        //{
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string contactDbConnectionString = this.Configuration.GetValue<string>("ContactAdmin:ContactDB:ConnectionString");
            string contactDbClientId = this.Configuration.GetValue<string>("ContactAdmin:ContactDB:ClientId");
            if (string.IsNullOrWhiteSpace(contactDbConnectionString))
            {
                throw new System.Configuration.ConfigurationErrorsException($"{nameof(contactDbConnectionString)}, this parameter is null");
            }
            //string userAssignedClientId = ""; //Give Client ID of User Managed Identity
            var conn = new SqlConnection(contactDbConnectionString);
            var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = contactDbClientId });
            var token = credential.GetToken(new Azure.Core.TokenRequestContext(new[] { "https://database.windows.net/.default" }));
            conn.AccessToken = token.Token;

            optionsBuilder.UseSqlServer(conn);
            base.OnConfiguring(optionsBuilder);
        }

        public virtual DbSet<Models.Contact> Contacts { get; set; }
    }
}
