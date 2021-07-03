

namespace Ev.Service.Contacts.Logs
{
    /// <summary>
    /// IAuditLog.
    /// </summary>
    public interface IAuditLog
    {
        /// <summary>
        /// Gets Instance.
        /// </summary>
        /// <returns>ILogger.</returns>
        Serilog.ILogger GetInstance();
    }
}
