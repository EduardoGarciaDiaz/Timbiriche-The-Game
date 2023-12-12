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
        private DateTime _endDate;
        private string _banStatus;

        [DataMember]
        public DateTime EndDate { get { return _endDate; } set { _endDate = value; } }

        [DataMember]
        public string BanStatus { get { return _banStatus; } set { _banStatus = value; } }
    }
}
