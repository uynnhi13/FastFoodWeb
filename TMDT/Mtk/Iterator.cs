using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMDT.Models;

namespace TMDT.Mtk
{
    public interface Iteratorr
    {
        Address First();
        Address Next();
        bool IsDone { get; }
        Address CurrentItem { get; }
    }
    public class Iterator : Iteratorr
    {
        readonly List<Address> listAddress;
        int current = 0;
        int step = 1;

        public Iterator(IQueryable<Address> lsAad)
        {
            listAddress = lsAad.ToList();
        }

        public bool IsDone {
            get { return current >= listAddress.Count; }
        }

        public Address CurrentItem =>listAddress[current];

        public IQueryable<Address> LsAad { get; }

        public Address First()
        {
            current = 0;
            if (listAddress.Count>0) { return listAddress[current]; }
            return null;
        }

        public Address Next()
        {
            current += step;
            if (!IsDone)
                return listAddress[current];
            else
                return null;
        }

   
    }
}