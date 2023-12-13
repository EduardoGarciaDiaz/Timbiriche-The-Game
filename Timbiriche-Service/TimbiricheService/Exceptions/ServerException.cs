using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService.Exceptions
{
    [DataContract]
    public class TimbiricheServerException : Exception
    {
        [DataMember]
        public new string Message { get; set; }
        [DataMember]
        public new string StackTrace { get; set; }
    }
}
