using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMDT.Areas.Admin.Controllers
{
    public class CategorySingleton
    {
        public static CategorySingleton Instance { get; } = new CategorySingleton();
        public CategorySingleton() { }
    }
}