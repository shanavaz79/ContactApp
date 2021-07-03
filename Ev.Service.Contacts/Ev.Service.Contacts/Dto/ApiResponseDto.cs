
namespace Ev.Service.Contacts.Dto
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// ApiResponseDto class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ApiResponseDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResponseDto" /> class.
        /// </summary>
        public ApiResponseDto()
        {
            this.Errors = new List<ErrorDto>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Data.
        /// </summary>
        public dynamic Data { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        public ApiResponseCode Code { get; set; }

        /// <summary>
        /// Gets Message.
        /// </summary>
        public IList<ErrorDto> Errors { get; }
    }
}
