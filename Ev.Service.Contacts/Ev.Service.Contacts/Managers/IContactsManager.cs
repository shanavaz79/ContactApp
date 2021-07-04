using Ev.Service.Contacts.Dto;

namespace Ev.Service.Contacts.Managers
{
    public interface IContactsManager
    {
        ApiResponseDto GetContacts(string status, int? limit, int? offset);
    }
}