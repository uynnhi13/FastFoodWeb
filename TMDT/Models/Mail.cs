using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMDT.Models
{
    public class Mail
    {
        public string Fullname { get; set; }
        public string FullName { get; internal set; }
        public string Email {
            get;
            set;
        }
        public string To {
            get;
            set;
        }
        public string Subject {
            get;
            set;
        }
        public string Content {
            get;
            set;
        }
    }
}