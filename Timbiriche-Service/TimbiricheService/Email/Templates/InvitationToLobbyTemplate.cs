using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService.Email.Templates
{
    public class InvitationToLobbyTemplate : IEmailTemplate
    {
        public string ComposeEmail(string message)
        {
            string emailContent = "Unete al juego! El código del lobby es: " + message;
            return emailContent;
        }
    }
}
