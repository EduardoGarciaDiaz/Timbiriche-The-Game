using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService.Email.Templates
{
    public class PasswordResetTemplate : IEmailTemplate
    {
        public string ComposeEmail(string message)
        {
            string emailContent = message;
            return emailContent;
        }
    }
}
