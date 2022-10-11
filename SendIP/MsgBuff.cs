using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace SendIP
{
    class MsgBuff
    {
        List<Message> messages;
        int numPrintMsg;
        int numMaxMsg;
        string logFile;

        public int NumPrintMsg
        {
            get
            {
                return numPrintMsg;
            }
            set
            {
                numPrintMsg = value;
            }
        }

        public string LogFile
        {
            get
            {
                return logFile;
            }
            set
            {
                logFile = value;
            }
        }

        public int NumMaxMsg
        {
            get
            {
                return numMaxMsg;
            }
            set
            {
                numMaxMsg = value;
            }
        }

        public List<Message> Messages
        {
            get
            {
                return messages;
            }
        }

        public int Count
        {
            get
            {
                return this.messages.Count;
            }
        }
       
        public MsgBuff(int NumPrintMsg, int NumMaxMsg, string LogFilePath)
        { 
            this.messages = new List<Message>();
            this.numPrintMsg = NumPrintMsg;
            this.numMaxMsg = NumMaxMsg;
            this.logFile = LogFilePath;
        }
       
        public void Add(string Type, string Msg)
        {
            int pos = this.messages.Count;

            if (pos == numMaxMsg)
            {
                this.messages.RemoveAt(0);
                pos = this.messages.Count;
            }

            Message pom = new Message(pos, Type, Msg);
            this.messages.Add(pom);
        }

        public void Clear()
        {
            this.messages.Clear();
        }

        public void Print(int left, int top)
        { 
            int i;
            int poc;

            if (numMaxMsg < numPrintMsg) poc = numMaxMsg;
            else poc = numPrintMsg;

            if (poc > this.Count) poc = this.Count;

            for (i = 0; i < poc; i++)
            {
                Program.ClearLine(left, top+i);
                Console.SetCursorPosition(left, top+i);
                Console.Write(messages[this.Count-i-1].ToString());
            }
        }

        public int WriteLog()
        {
            int vrat = -1;
            bool res;
            int i;

            try
            {
                DateTime TimeDate = DateTime.Now;

                ArrayList cont = new ArrayList();
                cont.Add("Send IP Log File");
                cont.Add("Date and time: " + TimeDate.ToString());
                cont.Add("");

                for (i = 0; i < this.messages.Count; i++)
                {
                    cont.Add(messages[i].ToString());
                }

                res = TextFile.writeFile(logFile, cont);

                if (res)
                {
                    vrat = 1;
                }
            }
            catch (Exception e) { }

            return vrat;
        }

    }
}
