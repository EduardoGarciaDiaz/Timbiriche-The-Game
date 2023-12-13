using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
    [DataContract]
    public class BanInformation
    {
        [DataMember]
        public DateTime EndDate { get; set; }

        [DataMember]
        public string BanStatus { get; set; }
    }
}
