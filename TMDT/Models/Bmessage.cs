using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using static log4net.Appender.RollingFileAppender;

namespace TMDT.Models
{
    public class Bmessage
    {
        public Bmessage() { }
        public Bmessage(string user_id, string message, DateTime timestamp, int decentral)
        {
            this.user_id = user_id;
            this.message = message;
            this.timestamp = timestamp;
            this.decentral = decentral;
        }

        public string user_id {  get; set; }
        public string message { get; set; }
        public DateTime timestamp { get; set; }
        public int decentral { get; set; }
    }
}