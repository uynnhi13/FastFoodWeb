using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMDT.MongoDB
{
    public class LikeProduct
    {
        public int idPro {  get; set; }
        public string namePro { get; set; }
        public string idUser { get; set; }
        public bool status { get; set; }
    }
}