using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendIP
{
    class Message
    {
        DateTime timeStamp;
        string msg;
        int pos;
        string type;

        public DateTime TimeStamp
        {
            get
            {
                return timeStamp;
            }
        }

        public string Msg
        {
            get
            {
                return msg;
            }
            set
            {
                msg = value;
            }
        }

        public int Pos
        {
            get
            {
                return pos;
            }
        }

        public string Type
        {
            get
            {
                return type;
            }
        }

        public Message(int Pos,string Type, string Msg)
        {
            this.timeStamp = DateTime.Now;
            this.pos = Pos;
            this.type = Type;
            this.msg = Msg;
        }

        public string ToString()
        {
            string vrat = "["+this.pos+"]["+timeStamp.Hour+":"+timeStamp.Minute+":"+timeStamp.Second+ "][" + type + "] " + msg;
            
            return vrat;
        }
    }
}
