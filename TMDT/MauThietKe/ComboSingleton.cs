using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMDT.Models;

namespace TMDT.MauThietKe
{
    public class ComboSingleton
    {
        public static ComboSingleton instance { get; } = new ComboSingleton();
        public List<Combo> lsCombo { get; } = new List<Combo>();
        // list đơn hàng còn sản phẩm còn có thể sử dụng không    
        public List<bool> SoldOut { get; set; } = new List<bool>();

        public void Init(TMDTThucAnNhanhEntities context)
        {
            if (lsCombo.Count == 0) {
                var combo = context.Combo.AsEnumerable()
                    .ToList();

                lsCombo.AddRange(combo);

                SoldOut.Clear();

                for (int i = 0; i < combo.Count; i++) {
                    //SoldOut[i] = true;
                    SoldOut.Add(true);
                }

                UpdateIngredientData(context, lsCombo);
            }
        }

        public void UpdateIngredientData(TMDTThucAnNhanhEntities context, List<Combo> _lsCombo)
        {
            UpdateStatus(context, _lsCombo);
        }

        public void HideOrShowCombo(int comboID)
        {
            var item = lsCombo.FirstOrDefault(f => f.comboID == comboID);
            int index = lsCombo.IndexOf(item);
            if (index != -1) {
                SoldOut[index] = false;
            }
        }

        private void UpdateStatus(TMDTThucAnNhanhEntities context, List<Combo> _lsCombo)
        {
            var lsIngredient = context.Ingredient.Where(i => i.quantity <= i.quantityMin).ToList();
            var lsRecipe = context.Recipe.AsEnumerable().ToList();
            var ComboDetail = context.ComboDetail.ToList();

            // list id sản phẩm hết nguyên liệu làm
            var intersectingIDs = lsRecipe
                .Where(r => lsIngredient.Select(i => i.ingID).Contains(r.ingID))
                .Select(r => r.cateID)
                .Intersect(ComboDetail.Select(f => f.cateID));

            // Find combos associated with those IDs
            var soldOutCombos = _lsCombo
                .Where(c => intersectingIDs.Contains(c.comboID))
                .Select(c => c.comboID);

            foreach (var id in soldOutCombos) {
                int index = lsCombo.FindIndex(f => f.comboID == id);
                SoldOut[index] = false; 
            }
        }

        public void Update(TMDTThucAnNhanhEntities context)
        {
            lsCombo.Clear();
            Init(context);
        }
    }
}