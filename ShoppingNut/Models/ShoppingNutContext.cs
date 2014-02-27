using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ShoppingNut.Models
{
    public class ShoppingNutContext : DbContext
    {
	    public ShoppingNutContext()
	    {
		    
	    }

	    public ShoppingNutContext(string connectionString):base(connectionString)
	    {
		    
	    }
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, add the following
        // code to the Application_Start method in your Global.asax file.
        // Note: this will destroy and re-create your database with every model change.
        // 
        // System.Data.Entity.Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<ShoppingNut.Models.ShoppingNutContext>());

			public DbSet<ShoppingNut.Models.Food> Foods { get; set; }

			public DbSet<ShoppingNut.Models.Ingredient> Ingredients { get; set; }

			public DbSet<ShoppingNut.Models.Instruction> Instructions { get; set; }

			public DbSet<ShoppingNut.Models.QuantityType> QuantityTypes { get; set; }

			public DbSet<ShoppingNut.Models.Recipe> Recipes { get; set; }

			public DbSet<ShoppingNut.Models.UserProfile> UserProfiles { get; set; }

			public DbSet<ShoppingNut.Models.ShoppingList> ShoppingLists { get; set; }

			public DbSet<ShoppingNut.Models.ShoppingListItem> ShoppingListItems { get; set; }

			public DbSet<ShoppingNut.Models.UserSubmittedImage> UserSubmittedImages { get; set; }

			public DbSet<ShoppingNut.Models.RecipeImage> RecipeImages { get; set; }

			public DbSet<ShoppingNut.Models.FoodGroup> FoodGroups { get; set; }

			public DbSet<ShoppingNut.Models.Source> Sources { get; set; }

			public DbSet<ShoppingNut.Models.ShoppingListShoppingListItem> ShoppingListShoppingListItems { get; set; }
		}
}