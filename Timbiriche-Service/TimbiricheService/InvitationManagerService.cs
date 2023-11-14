using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimbiricheService.Email;
using TimbiricheService.Email.Templates;

namespace TimbiricheService
{
    public partial class UserManagerService : IInvitationManager
    {
        public bool SendInvitationToEmail(string lobbyCode, string email)
        {
            EmailSender emailSender = new EmailSender(new InvitationToLobbyTemplate());
            bool isEmailSent = emailSender.SendEmail(email, lobbyCode);
            return isEmailSent;
        }
    }
}
