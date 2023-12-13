using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheViews.Components
{
    public class ButtonClickEventArgs : EventArgs
    {
        public string ButtonName { get; private set; }
        public string Username { get; private set; }

        public ButtonClickEventArgs(string buttonName, string username)
        {
            ButtonName = buttonName;
            Username = username;
        }
    }
}
