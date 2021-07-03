
namespace Ev.Service.Contacts.Dto
{
    /// <summary>
    /// Api Error Codes.
    /// </summary>
    public enum ApiResponseCode
    {
        /// <summary>
        /// Default.
        /// </summary>
        None = 0,

        /// <summary>
        /// The success
        /// </summary>
        Success = 1000,

        /// <summary>
        /// The duplicate data
        /// </summary>
        DuplicateData = 1001,

        /// <summary>
        /// The missing mandatory information
        /// </summary>
        MissingMandatoryInformation = 1002,

        /// <summary>
        /// The format error
        /// </summary>
        FormatError = 1003,

        /// <summary>
        /// The invalid data
        /// </summary>
        InvalidData = 1004,

        /// <summary>
        /// The resource doesnt exist
        /// </summary>
        ResourceDoesntExist = 1005,

        /// <summary>
        /// The no data found
        /// </summary>
        NoDataFound = 1006,


        /// <summary>
        /// The multiple errors
        /// </summary>
        MultipleErrors = 1100,
    }
}
