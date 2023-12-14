using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TMDT.MauThietKe
{
    public abstract class ControllerTemplateMethod : Controller
    {
        public abstract void PrintRouter();
        public abstract void PrintDIs();

        //template methods
        public void PrintInformation()
        {
            PrintRouter();
            PrintDIs();
        }
    }
}