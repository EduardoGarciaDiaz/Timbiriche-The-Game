using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
    [DataContract]
    public class Movement
    {
        private string _typeline;
        private int _row;
        private int _column;
        private int _earnedPoints;
        private string _hexadecimalColor;
        private string _stylePath;
        private string _username;

        [DataMember]
        public string TypeLine { get { return _typeline; } set { _typeline = value; } }

        [DataMember]
        public int Row { get { return _row; } set { _row = value; } }
        
        [DataMember]
        public int Column { get { return _column; } set { _column = value; } }
        
        [DataMember]
        public int EarnedPoints { get { return _earnedPoints; } set { _earnedPoints = value; } }
        
        [DataMember]
        public string HexadecimalColor { get { return _hexadecimalColor; } set { _hexadecimalColor = value; } }
        
        [DataMember]
        public string StylePath { get { return _stylePath; } set { _stylePath = value; } }
        
        [DataMember]
        public string Username { get { return _username; } set { _username = value; } }
    }
}
