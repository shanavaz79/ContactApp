

namespace Ev.Service.Contacts.Logs
{
    using System.Diagnostics.CodeAnalysis;
    using Serilog;

    /// <summary>
    /// AuditLog.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AuditLog : IAuditLog
    {
        /// <summary>
        /// Gets Instance.
        /// </summary>
        /// <returns>ILogger.</returns>
        public Serilog.ILogger GetInstance()
        {
            return Log.ForContext("Category", "Audit");
        }
    }
}
