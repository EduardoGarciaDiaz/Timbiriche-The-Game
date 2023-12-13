using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
    [DataContract]
    public class LobbyPlayer
    {
        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public int IdStylePath { get; set; }
        
        [DataMember]
        public int IdHexadecimalColor { get; set; }
        
        [DataMember]
        public string StylePath { get; set; }
        
        [DataMember]
        public string HexadecimalColor { get; set; }

        public ILobbyManagerCallback CallbackChannel { get; set; }
        public IMatchManagerCallback MatchCallbackChannel { get; set; }
        public IPlayerColorsManagerCallback ColorCallbackChannel { get; set; }
        public IPlayerStylesManagerCallback StyleCallbackChannel { get; set; }
        public IBanManagerCallback BanManagerChannel { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                LobbyPlayer otherPlayer = (LobbyPlayer)obj;
                return Username == otherPlayer.Username && IdStylePath == otherPlayer.IdStylePath &&
                    IdHexadecimalColor == otherPlayer.IdHexadecimalColor && StylePath == otherPlayer.StylePath &&
                    HexadecimalColor == otherPlayer.HexadecimalColor;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
