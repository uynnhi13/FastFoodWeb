﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TMDT.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    using System.Data;
    using System.Data.SqlClient;
    using System.Collections.Generic;

    public partial class TMDTThucAnNhanhEntities : DbContext
    {
        public TMDTThucAnNhanhEntities()
            : base("name=TMDTThucAnNhanhEntities")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        public virtual DbSet<C__EFMigrationsHistory> C__EFMigrationsHistory { get; set; }
        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Combo> Combo { get; set; }
        public virtual DbSet<ComboDetail> ComboDetail { get; set; }
        public virtual DbSet<Condition> Condition { get; set; }
        public virtual DbSet<Employees> Employees { get; set; }
        public virtual DbSet<Ingredient> Ingredient { get; set; }
        public virtual DbSet<Invoice> Invoice { get; set; }
        public virtual DbSet<InvoiceDetails> InvoiceDetails { get; set; }
        public virtual DbSet<location> location { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderDetail> OrderDetail { get; set; }
        public virtual DbSet<Position> Position { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Recipe> Recipe { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Unit> Unit { get; set; }
        public virtual DbSet<User> User { get; set; }

        public virtual int AddProductAndCombo(string name, Nullable<decimal> price, string image, Nullable<int> typeID, Nullable<decimal> priceUp)
        {
            var nameParameter = name != null ?
                new ObjectParameter("name", name) :
                new ObjectParameter("name", typeof(string));

            var priceParameter = price.HasValue ?
                new ObjectParameter("price", price) :
                new ObjectParameter("price", typeof(decimal));

            var imageParameter = image != null ?
                new ObjectParameter("image", image) :
                new ObjectParameter("image", typeof(string));

            var typeIDParameter = typeID.HasValue ?
                new ObjectParameter("typeID", typeID) :
                new ObjectParameter("typeID", typeof(int));

            var priceUpParameter = priceUp.HasValue ?
                new ObjectParameter("priceUp", priceUp) :
                new ObjectParameter("priceUp", typeof(decimal));

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("AddProductAndCombo", nameParameter, priceParameter, imageParameter, typeIDParameter, priceUpParameter);
        }

        public virtual void createRecipe(string productName, Nullable<decimal> productPrice, Nullable<decimal> productPriceUp, string productImage, Nullable<int> productTypeID, List<ingre> IngredientsList)
        {
            // table type
            var contextAdapter = (IObjectContextAdapter)this;
            var objectContext = contextAdapter.ObjectContext;

            var Ingredients = new DataTable("IngredientsList");
            Ingredients.Columns.Add("id", typeof(int));
            Ingredients.Columns.Add("quantity", typeof(decimal));

            /*foreach (var user in userList)
            {
                userTable.Rows.Add(user.id, user.name, user.password);
            }*/

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
        }
    }
}
