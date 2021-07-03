

namespace Ev.Service.Contacts.Logs
{
    /// <summary>
    /// IInfoLog.
    /// </summary>
    public interface IInfoLog
    {
        /// <summary>
        /// Gets Instance.
        /// </summary>
        /// <returns>ILogger.</returns>
        Serilog.ILogger GetInstance();
    }
}
