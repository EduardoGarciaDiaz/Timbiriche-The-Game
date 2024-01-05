using TimbiricheService.Email;
using TimbiricheService.Email.Templates;

namespace TimbiricheService
{
    public partial class UserManagerService : IInvitationManager
    {
        public bool SendInvitationToEmail(string lobbyCode, string email)
        {
            EmailSender emailSender = new EmailSender(new InvitationToLobbyTemplate());
            return emailSender.SendEmail(email, lobbyCode);
        }
    }
}
