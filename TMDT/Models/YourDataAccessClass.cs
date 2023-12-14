using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using TMDT.Models;

public class YourDataAccessClass
{
    private string connectionString = "Data Source=PHONG_MSI_15\\SQLEXPRESS;Initial Catalog=TMDTThucAnNhanh;Integrated Security=True;Multiple Active Result Sets=True;Trust Server Certificate=True;Application Name=EntityFramework";

    public void CreateRecipeDB(string productName, decimal? productPrice, decimal? productPriceUp, string productImage, int? productTypeID, List<ingre> ingredientsList)
    {
        DataTable ingredientsTable = new DataTable("IngredientsList");
        ingredientsTable.Columns.Add("id", typeof(int));
        ingredientsTable.Columns.Add("quantity", typeof(decimal));

        foreach (var item in ingredientsList) {
            DataRow row = ingredientsTable.NewRow();
            row["id"] = item.id;
            row["quantity"] = item.quantity;
            ingredientsTable.Rows.Add(row);
        }

        using (SqlConnection connection = new SqlConnection(connectionString)) {
            SqlCommand command = new SqlCommand("createRecipe", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@ProductName", productName);
            command.Parameters.AddWithValue("@ProductPrice", productPrice ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ProductPriceUp", productPriceUp ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ProductImage", productImage);
            command.Parameters.AddWithValue("@ProductTypeID", productTypeID ?? (object)DBNull.Value);

            SqlParameter ingredientsListParam = new SqlParameter("@IngredientsList", SqlDbType.Structured) {
                TypeName = "dbo.IngredientsList", // Thay thế bằng tên thực tế của type
                Value = ingredientsTable
            };

            command.Parameters.Add(ingredientsListParam);

            try {
                connection.Open();
                command.ExecuteNonQuery();
                Console.WriteLine("Stored procedure executed successfully.");
            }
            catch (Exception ex) {
                Console.WriteLine("Error: " + ex.Message);
                // Xử lý ngoại lệ
            }
        }
    }
}
