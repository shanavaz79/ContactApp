using Ev.Service.Contacts.Dto;
using System.Threading.Tasks;

namespace Ev.Service.Contacts.Managers
{
    public interface IContactsManager
    {
        ApiResponseDto GetContacts(string status, int? limit, int? offset);
        Task<ApiResponseDto> GetByKeyAsync(int contactId);
        Task<ApiResponseDto> AddContact(PostContactDto contact);
    }
}