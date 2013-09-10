using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoppingNut.Models;

namespace ShoppingNut.Controllers
{
	/// <summary>
	/// $entityTypes = "Food", "Ingredient", "Instruction", "InstructionItem", "QuantityType", "Recipe"
	/// foreach($type in $entityTypes){Scaffold Repository -ModelType $type -Force}
	/// To update database when you get a message "Model backing the blah blah has changed since the database was created. blah"
	/// 
	/// </summary>
	public class HomeController : Controller
	{
		//private List<Recipe> Recipes = new List<Recipe>
		//{
		//	new Recipe{Id = 1, Name = "Black bean salad", Description = "Black beans, corn, avacado, cilantro, etc", CaloriesPerServing = 100},
		//	new Recipe{Id = 2, Name = "Quinoa salad", Description = "Quinoa, avacado, garbonzo beans, etc", CaloriesPerServing = 90},
		//	new Recipe{Id = 3, Name = "Snickers pie", Description = "Creamy pie with snickers candy bars in them.", CaloriesPerServing = 300},
		//	new Recipe{Id = 4, Name = "Fried halibut", Description = "Halibut fried with saltine crackers", CaloriesPerServing = 250},
		//	new Recipe{Id = 5, Name = "Sweet potato burgers", Description = "Mashed up sweet potatoes fried on the stove top", CaloriesPerServing = 190},
		//	new Recipe{Id = 6, Name = "Waffles", Description = "Delicious!", CaloriesPerServing = 150},
		//	new Recipe{Id = 7, Name = "Coconut cream pie", Description = "Coconut cream pie with merage and toasted coconuts", CaloriesPerServing = 250},
		//};

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult GetAllRecipes()
		{
			RecipeRepository recipes = new RecipeRepository();
			//List<Recipe> recipe = recipes.AllIncluding(x => x.Ingredients, x => x.Instructions).Select(x=>new {x.Id, x.Name, x.Description})
			var recipe = recipes.All.Select(x => new { x.Id, x.Name, x.Description }).ToList();
			return Json(recipe, JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetShoppingLists()
		{
			var users = new UserProfileRepository();
			var id = users.All.Single(x => x.UserName == User.Identity.Name).UserId;
			var lists = new ShoppingListRepository();
			var userLists = lists.All.Where(x => x.UserId == id).Select(x => new { x.Id, x.Name }).ToList();
			return Json(userLists, JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetAllIngredients()
		{
			IngredientRepository ingredients = new IngredientRepository();
			var ingredientJson = ingredients.AllIncluding(x => x.Food).Select(x => new { x.Id, x.Food.Name, x.Food.Calories }).ToList();
			return Json(ingredientJson, JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetFoods(string filter)
		{
			//TODO: Fix issue where if a user is typing we go get values
			FoodRepository foods = new FoodRepository();
			List<Food> foodsJson = new List<Food>();
			string[] filters = filter.Split(' ');
			if (filters.Count() == 1)
				foodsJson.AddRange(this.FoodsSearchStartsWith(filters, foods));
			foodsJson.AddRange(this.FoodsSearchOne(filters, foods));
			return Json(foodsJson.Distinct().Select(x => new { x.Id, x.Name, x.Calories }).ToList(), JsonRequestBehavior.AllowGet);
		}

		private List<Food> FoodsSearchStartsWith(string[] filters, FoodRepository foods)
		{
			List<Food> foodSearchResults = new List<Food>();
			foreach (var s in filters)
			{
				var foodSearch = foods.All;
				var result = foodSearch.Where(x => x.Name.StartsWith(s));
				foodSearchResults.AddRange(result.ToList());
			}
			return foodSearchResults.Take(20).ToList();
		}

		private List<Food> FoodsSearchOne(string[] filters, FoodRepository foods)
		{
			var foodSearchOne = foods.All;
			foreach (var s in filters)
			{
				foodSearchOne = foodSearchOne.Where(x => x.Name.Contains(s));
			}
			return foodSearchOne.Take(20).ToList();
		}

		public ActionResult GetFoodQuantityTypes(int id)
		{
			var q = new QuantityTypeRepository().All.Where(x => x.FoodId == id).Select(x => new { x.Id, x.Grams, x.Name, x.FoodId });
			return Json(q, JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetRecipeById(int id)
		{
			RecipeRepository recipes = new RecipeRepository();
			var recipe = recipes.Find(id);
			return Json(recipe.ToJson(), JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetListById(int id)
		{
			ShoppingListRepository lists = new ShoppingListRepository();
			var list = lists.Find(id);
			return Json(list.ToJson(), JsonRequestBehavior.AllowGet);
		}

		public ActionResult AddRecipe(Recipe recipe)
		{
			if (!this.User.Identity.IsAuthenticated)
				throw new Exception("User not logged in");
			try
			{
				UserProfileRepository users = new UserProfileRepository();

				recipe.UserId = users.All.Single(x => x.UserName == this.User.Identity.Name).UserId;
				Ingredient[] ings = null;
				if (recipe.Ingredients != null && recipe.Ingredients.Any())
				{
					foreach (var v in recipe.Ingredients)
					{
						v.Food = null;
						v.QuantityType = null;
					}
					ings = new Ingredient[recipe.Ingredients.Count];
					recipe.Ingredients.CopyTo(ings);
					recipe.Ingredients = null;
				}
				RecipeRepository recipes = new RecipeRepository();
				recipes.InsertOrUpdate(recipe);
				recipes.Save();

				IngredientRepository ingredients = new IngredientRepository();

				if (ings != null && ings.Any())
				{
					foreach (var v in ings)
					{
						Ingredient newIng = new Ingredient
							{
								RecipeId = recipe.Id,
								Id = v.Id,
								FoodId = v.FoodId,
								Quantity = v.Quantity,
								QuantityTypeId = v.QuantityTypeId
							};
						ingredients.InsertOrUpdate(newIng);
					}
					ingredients.Save();
				}

				return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				throw;
				return Json(ex.Message, JsonRequestBehavior.AllowGet);
			}
		}

		public ActionResult InsertOrUpdateShoppingList(ShoppingList list)
		{
			if (!this.User.Identity.IsAuthenticated)
				throw new Exception("User not logged in");
			try
			{
				UserProfileRepository users = new UserProfileRepository();

				list.UserId = users.All.Single(x => x.UserName == this.User.Identity.Name).UserId;
				foreach (var v in list.Items)
				{
					v.Food = null;
					v.QuantityType = null;
				}
				ShoppingListRepository lists = new ShoppingListRepository();
				lists.InsertOrUpdate(list);
				lists.Save();

				ShoppingListItemRepository items = new ShoppingListItemRepository();

				foreach (var v in list.Items)
				{
					ShoppingListItem item = new ShoppingListItem
					{
						FoodId = v.FoodId,
						ShoppingListId = v.ShoppingListId,
						UserItemName = v.Name,
						Id = v.Id,
						Quantity = v.Quantity,
						QuantityTypeId = v.QuantityTypeId
					};
					items.InsertOrUpdate(item);
				}
				items.Save();

				return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}

		public ActionResult Boot()
		{
			return View();
		}
	}
}
