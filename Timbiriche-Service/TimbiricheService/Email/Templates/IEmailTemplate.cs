using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService.Email.Templates
{
    public interface IEmailTemplate
    {
        string ComposeEmail(string message);
    }
}
