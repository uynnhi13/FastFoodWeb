using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace TMDT.Models
{
    public static class Procedure
    {
/*

        public virtual static void createRecipeDB(string productName, Nullable<decimal> productPrice, Nullable<decimal> productPriceUp, string productImage, Nullable<int> productTypeID, List<ingre> IngredientsList)
        {
            // table type
            var contextAdapter = (IObjectContextAdapter)this;
            var objectContext = contextAdapter.ObjectContext;

            var Ingredients = new DataTable("IngredientsList");
            Ingredients.Columns.Add("id", typeof(int));
            Ingredients.Columns.Add("quantity", typeof(decimal));

            foreach (var item in IngredientsList) {
                DataRow row = Ingredients.NewRow();
                row["id"] = item.id;
                row["quantity"] = item.quantity;
                Ingredients.Rows.Add(row);
            }

            using (var context = new TMDTThucAnNhanhEntities()) // Thay YourDbContext bằng tên DbContext thực tế của bạn
            {
                SqlParameter productNamePara = new SqlParameter("@ProductName", productName);
                SqlParameter productPricePara = new SqlParameter("@ProductPrice", productPrice);
                SqlParameter productPriceUpPara = new SqlParameter("@ProductPriceUp", productPriceUp);
                SqlParameter productImagePara = new SqlParameter("@ProductImage", productImage);
                SqlParameter productTypeIDPara = new SqlParameter("@ProductTypeID", productTypeID);

                SqlParameter IngredientsListPara = new SqlParameter("@IngredientsList", SqlDbType.Structured) {
                    TypeName = "dbo.IngredientsList",
                    Value = Ingredients
                };

                context.Database.ExecuteSqlCommand("EXEC createRecipe @ProductName, @ProductPrice, @ProductPriceUp, @ProductImage, @ProductTypeID, @IngredientsList",
                productNamePara, productPricePara, productPriceUpPara, productImagePara, productTypeIDPara, IngredientsListPara);
            }
        }*/
    }
}