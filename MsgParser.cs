using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace partymode
{
/*    abstract class TCPMessage
    {
        public TCPMessage(string actionName, JsonElement data) {
            
        }
        protected abstract bool isValid(JsonElement data);
    }*/
    internal class TCPMsgHandler
    {
        Dictionary<string, TCPMsg> requests = new Dictionary<string, TCPMsg>();
        private TCPMsgHandler() {}
        static private TCPMsgHandler _instance { get; set; } = null;
        static public TCPMsgHandler instance { get { 
                if (_instance == null) _instance = new TCPMsgHandler();
                return _instance;
            }
            private set { _instance = value; }
        }
        public string handle(string cmd, JsonElement data)
        {
            try
            {
                TCPMsg msg = requests[cmd];
                if (msg == null) return "\"status\": \"fail\"";
                return msg.invokeCmd(data);
            }
            catch {return "\"status\": \"fail\""; }
        }
        public void subscribe(string cmd, TCPMsg msg) {
            requests.Add(cmd, msg);
        }
        public void unsubscribe(string cmd)
        {
            requests.Remove(cmd);
        }
    }
    class TCPMsg
    {
        string cmd;
        Func<JsonElement, string> handler;
        public TCPMsg(string cmd, Func<JsonElement, string> handler) {
            this.cmd = cmd;
            this.handler = handler;
            TCPMsgHandler.instance.subscribe(cmd, this);
        }
        ~TCPMsg() {
            TCPMsgHandler.instance.unsubscribe(this.cmd);
        }
        public string invokeCmd(JsonElement element)
        {
            return this.handler(element);
        }
    }
}
