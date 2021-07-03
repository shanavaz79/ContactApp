

namespace Ev.Service.Contacts.Logs
{
    using Serilog;

    /// <summary>
    /// AuditLog.
    /// </summary>
    public class InfoLog : IInfoLog
    {
        /// <summary>
        /// Gets Instance.
        /// </summary>
        /// <returns>ILogger.</returns>
        public Serilog.ILogger GetInstance()
        {
            return Log.ForContext("Category", "Info");
        }
    }
}
