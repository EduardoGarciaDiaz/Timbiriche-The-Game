using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
    [DataContract]
    public class Account
    {
        [DataMember]
        public int IdAccount { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Surname { get; set; }

        [DataMember]
        public DateTime Birthdate { get; set; }
    }
}
