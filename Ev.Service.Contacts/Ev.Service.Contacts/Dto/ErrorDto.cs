
namespace Ev.Service.Contacts.Dto
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Error Dto.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ErrorDto
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        public ApiResponseCode Code { get; set; }

        /// <summary>
        /// Gets or sets Error.
        /// </summary>
        public string Error { get; set; }
    }
}
