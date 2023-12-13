using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
    [DataContract]
    public class LobbyInformation
    {
        [DataMember]
        public float MatchDurationInMinutes { get; set; }

        [DataMember]
        public float TurnDurationInMinutes { get; set; }
    }
}
