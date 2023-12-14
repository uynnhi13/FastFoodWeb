using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMDT.Models;

namespace TMDT.MauThietKe
{
    public class updateCombo
    {
        private TMDTThucAnNhanhEntities db { get; set; }

        public updateCombo(TMDTThucAnNhanhEntities db) {
            this.db = db;
        }

        public List<int> getlistComboID(Product product)
        {
            List<int> resum = new List<int>();
            var lsComboDetail = db.ComboDetail.Where(f=>f.cateID == product.cateID).Select(s=>s.comboID);
            resum.AddRange(lsComboDetail);
            return resum;
        }
    }
}