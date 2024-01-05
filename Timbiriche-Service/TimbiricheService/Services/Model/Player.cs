using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
    [DataContract]
    public class Player
    {
        [DataMember]
        public int IdPlayer { get; set; }

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public int Coins { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string Salt { get; set; }

        [DataMember]
        public int IdColorSelected { get; set; }

        [DataMember]
        public int IdStyleSelected { get; set; }

        [DataMember]
        public Account AccountFK { get; set; }
    }
}
