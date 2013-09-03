using System.Web.Security;
using ShoppingNut.Models;
using WebMatrix.WebData;

namespace ShoppingNut.Migrations
{
	using System;
	using System.Data.Entity;
	using System.Data.Entity.Migrations;
	using System.Linq;

	internal sealed class Configuration : DbMigrationsConfiguration<ShoppingNut.Models.ShoppingNutContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = true;
		}

		protected override void Seed(ShoppingNut.Models.ShoppingNutContext context)
		{
			WebSecurity.InitializeDatabaseConnection("ShoppingNutContext", "UserProfile", "UserId", "UserName", autoCreateTables: true);

			if (!Roles.RoleExists("Administrator"))
				Roles.CreateRole("Administrator");

			if (!WebSecurity.UserExists("aboes"))
				WebSecurity.CreateUserAndAccount(
						"aboes",
						"abcdefghi");

			if (!Roles.GetRolesForUser("aboes").Contains("Administrator"))
				Roles.AddUsersToRoles(new[] { "aboes" }, new[] { "Administrator" });
			//Food brownSugar = new Food {Calories = 380, Name = "Sugars,Brown"};
			//Food avocado = new Food {Calories = 167, Name = "AVOCADOS,RAW,CALIFORNIA"};
			//context.Foods.Add(brownSugar);
			//context.Foods.Add(avocado);
			//context.SaveChanges();

			//QuantityType brownSugarCupPacked = new QuantityType { Grams = 220, Name = "cup packed", FoodId = brownSugar.Id };
			//QuantityType brownSugarCupUnpacked = new QuantityType { Grams = 145, Name = "cup unpacked", FoodId = brownSugar.Id };
			//QuantityType brownSugarTspBrownulated = new QuantityType { Grams = 3.2, Name = "tsp brownulated", FoodId = brownSugar.Id };
			//QuantityType brownSugarTspPacked = new QuantityType { Grams = 4.6, Name = "tsp packed", FoodId = brownSugar.Id };
			//QuantityType brownSugarTspUnpacked = new QuantityType { Grams = 3, Name = "tsp unpacked", FoodId = brownSugar.Id };
			//context.QuantityTypes.Add(brownSugarCupPacked);
			//context.QuantityTypes.Add(brownSugarCupUnpacked);
			//context.QuantityTypes.Add(brownSugarTspBrownulated);
			//context.QuantityTypes.Add(brownSugarTspPacked);
			//context.QuantityTypes.Add(brownSugarTspUnpacked);

			//QuantityType a1 = new QuantityType { FoodId = avocado.Id, Grams = 230, Name = "cup, pureed" };
			//QuantityType a2 = new QuantityType { FoodId = avocado.Id, Grams = 136, Name = "fruit, without skin and seed" };
			//QuantityType a3 = new QuantityType { FoodId = avocado.Id, Grams = 30, Name = "NLEA serving" };
			//context.QuantityTypes.Add(a1);
			//context.QuantityTypes.Add(a2);
			//context.QuantityTypes.Add(a3);
			//context.SaveChanges();
			//Food avacado = new Food { Calories = 100, Name = "Avacado" };
			//Food blackBeans = new Food { Calories = 40, Name = "Black Beans" };
			//Food cilantro = new Food { Calories = 5, Name = "Cilantro" };
			//Food corn = new Food { Calories = 80, Name = "Corn" };
			//Food oliveOil = new Food { Calories = 50, Name = "Olive Oil" };
			//context.Foods.Add(avacado);
			//context.Foods.Add(blackBeans);
			//context.Foods.Add(cilantro);
			//context.Foods.Add(corn);
			//context.Foods.Add(oliveOil);
			//context.SaveChanges();


			//Recipe recipe = new Recipe {Name = "Black Bean Salad", Description = "It's good :o)", CaloriesPerServing = 100};
			//context.Recipes.Add(recipe);
			//context.SaveChanges();

			//Ingredient iAvacado = new Ingredient { FoodId = avacado.Id, Quantity = 1, RecipeId = recipe.Id };
			//Ingredient iBlackBeans = new Ingredient { FoodId = blackBeans.Id, Quantity = 2, RecipeId = recipe.Id };
			//Ingredient iCilantro = new Ingredient { FoodId = cilantro.Id, Quantity = .25, RecipeId = recipe.Id };
			//Ingredient iCorn = new Ingredient { FoodId = corn.Id, Quantity = 2, RecipeId = recipe.Id };
			//Ingredient iOliveOil = new Ingredient { FoodId = oliveOil.Id, Quantity = 3, RecipeId = recipe.Id };
			//context.Ingredients.Add(iAvacado);
			//context.Ingredients.Add(iBlackBeans);
			//context.Ingredients.Add(iCilantro);
			//context.Ingredients.Add(iCorn);
			//context.Ingredients.Add(iOliveOil);
			//context.SaveChanges();

			//Instruction stepOne = new Instruction { Order = 1, Text = "Combine all ingredients except avacado in a medium mixing bowl", RecipeId = recipe.Id};
			//Instruction stepTwo = new Instruction { Order = 2, Text = "Add avacado and mix gently.", RecipeId = recipe.Id};
			//context.Instructions.Add(stepOne);
			//context.Instructions.Add(stepTwo);
			//context.SaveChanges();

			//InstructionItem itemOne = new InstructionItem { InstructionId = stepOne.Id, RecipeId = recipe.Id };
			//InstructionItem itemTwo = new InstructionItem { InstructionId = stepTwo.Id, RecipeId = recipe.Id };
			//context.InstructionItems.Add(itemOne);
			//context.InstructionItems.Add(itemTwo);
			//context.SaveChanges();

			//  This method will be called after migrating to the latest version.

			//  You can use the DbSet<T>.AddOrUpdate() helper extension method 
			//  to avoid creating duplicate seed data. E.g.
			//
			//    context.People.AddOrUpdate(
			//      p => p.FullName,
			//      new Person { FullName = "Andrew Peters" },
			//      new Person { FullName = "Brice Lambson" },
			//      new Person { FullName = "Rowan Miller" }
			//    );
			//
		}
	}
}
