using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoppingNut.Models;
using System.IO;

namespace ShoppingNut.Controllers
{
	/// <summary>
	/// $entityTypes = "Food", "Ingredient", "Instruction", "InstructionItem", "QuantityType", "Recipe", "RecipeImage", "UserSubmittedImage"
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
			var recipe = recipes.AllIncluding(x => x.Images).ToList();
			List<object> v = new List<object>();
			recipe.ForEach(x => v.Add(x.ToJsonLite()));
			return Json(v, JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetAllRecipesWithIngredients()
		{
			RecipeRepository recipes = new RecipeRepository();
			var recipe = recipes.AllIncluding(x => x.Ingredients).ToList();
			List<object> v = new List<object>();
			recipe.ForEach(x => v.Add(x.ToJson()));
			return Json(v, JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetShoppingLists()
		{
			var users = new UserProfileRepository();
			var id = users.All.Single(x => x.UserName == User.Identity.Name).UserId;
			var lists = new ShoppingListRepository();
			var userLists = lists.AllIncluding(x => x.Items).Where(x => x.UserId == id).ToList();
			List<object> v = new List<object>();
			userLists.ForEach(x => v.Add(x.ToJson()));
			return Json(v, JsonRequestBehavior.AllowGet);
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

		public ActionResult InsertOrUpdateRecipe(Recipe recipe)
		{
			if (!this.User.Identity.IsAuthenticated)
				throw new Exception("User not logged in");
			recipe.Instructions = null;
			recipe.Ingredients = null;
			try
			{
				UserProfileRepository users = new UserProfileRepository();
				recipe.UserId = users.All.Single(x => x.UserName == this.User.Identity.Name).UserId;
				RecipeRepository recipes = new RecipeRepository();
				recipes.InsertOrUpdate(recipe);
				recipes.Save();
				return Json(new { Success = true, RecipeId = recipe.Id }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception)
			{
				throw;
			}
		}

		public ActionResult InsertOrUpdateIngredient(Ingredient ingredient)
		{
			if (!this.User.Identity.IsAuthenticated)
				throw new Exception("User not logged in");
			try
			{
				if (ingredient.RecipeId == default(int))
				{
					Recipe recipe = new Recipe();
					UserProfileRepository users = new UserProfileRepository();
					recipe.UserId = users.All.Single(x => x.UserName == this.User.Identity.Name).UserId;
					RecipeRepository recipes = new RecipeRepository();
					recipes.InsertOrUpdate(recipe);
					recipes.Save();
					ingredient.RecipeId = recipe.Id;
				}
				IngredientRepository ingredients = new IngredientRepository();
				Ingredient newIng = new Ingredient
				{
					RecipeId = ingredient.RecipeId,
					Id = ingredient.Id,
					FoodId = ingredient.FoodId,
					Quantity = ingredient.Quantity,
					QuantityTypeId = ingredient.QuantityTypeId,
					Notes = ingredient.Notes
				};
				ingredients.InsertOrUpdate(newIng);
				ingredients.Save();
				return Json(new { Success = true, ingredient.RecipeId, IngredientId = newIng.Id }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception)
			{
				throw;
			}
		}

		[HttpPost]
		public ActionResult DeleteIngredient(int id)
		{
			if (!this.User.Identity.IsAuthenticated)
				throw new Exception("User not logged in");
			try
			{
				IngredientRepository ingredients = new IngredientRepository();
				ingredients.Delete(id);
				ingredients.Save();
				return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception)
			{
				throw;
			}
		}

		[HttpPost]
		public ActionResult DeleteInstruction(int id)
		{
			if (!this.User.Identity.IsAuthenticated)
				throw new Exception("User not logged in");
			try
			{
				InstructionRepository repo = new InstructionRepository();
				repo.Delete(id);
				repo.Save();
				return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception)
			{
				throw;
			}
		}

		public ActionResult InsertOrUpdateInstruction(Instruction instruction)
		{
			if (!this.User.Identity.IsAuthenticated)
				throw new Exception("User not logged in");
			try
			{
				if (instruction.RecipeId == default(int))
				{
					Recipe recipe = new Recipe();
					UserProfileRepository users = new UserProfileRepository();
					recipe.UserId = users.All.Single(x => x.UserName == this.User.Identity.Name).UserId;
					RecipeRepository recipes = new RecipeRepository();
					recipes.InsertOrUpdate(recipe);
					recipes.Save();
					instruction.RecipeId = recipe.Id;
				}
				InstructionRepository repo = new InstructionRepository();
				repo.InsertOrUpdate(instruction);
				repo.Save();
				return Json(new { Success = true, RecipeId = instruction.RecipeId, InstructionId = instruction.Id }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception)
			{
				throw;
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

		public ActionResult ItemPickedUp(int id, bool pickedUp)
		{
			if (!this.User.Identity.IsAuthenticated)
				throw new Exception("User not logged in");
			ShoppingListItemRepository repo = new ShoppingListItemRepository();
			var item = repo.Find(id);
			item.PickedUp = pickedUp;
			repo.InsertOrUpdate(item);
			repo.Save();
			return null;
		}

		[HttpPost]
		public ActionResult UploadImages(HttpPostedFileBase[] file, RecipeIdWrapper id)
		{
			if (!this.User.Identity.IsAuthenticated)
				throw new Exception("User not logged in");
			if (file == null || !file.Any())
				return Json("no files");
			UserSubmittedImageRepository imageRepo = new UserSubmittedImageRepository();
			RecipeImageRepository recipeImageRepo = new RecipeImageRepository();
			foreach (var f in file)
			{
				UserSubmittedImage image = new UserSubmittedImage { Image = ModifyImageForDatabase(f) };
				imageRepo.InsertOrUpdate(image);
				imageRepo.Save();
				RecipeImage recipeImage = new RecipeImage { UserSubmittedImageId = image.Id, RecipeId = Convert.ToInt32(id.recipeId) };
				recipeImageRepo.InsertOrUpdate(recipeImage);
				recipeImageRepo.Save();
			}

			return Json("Got it!", JsonRequestBehavior.DenyGet);
		}

		public ActionResult GetRecipeImages(int recipeId)
		{
			RecipeRepository repo = new RecipeRepository();
			Recipe recipe = repo.Find(recipeId);
			if (recipe != null && recipe.Images.Any())
			{
				int[] imageIds = recipe.Images.Select(x => x.UserSubmittedImageId).ToArray();
				return Json(imageIds, JsonRequestBehavior.AllowGet);
			}
			return Json(null, JsonRequestBehavior.AllowGet);
		}

		private byte[] ModifyImageForDatabase(HttpPostedFileBase file)
		{
			Image image = new Bitmap(file.InputStream);
			Size newSize = default(Size);
			newSize.Width = 320;
			newSize.Height = (int)((decimal)image.Size.Height / (decimal)((decimal)image.Size.Width / (decimal)newSize.Width));
			Image newImage = new Bitmap(image, newSize);
			MemoryStream ms = new MemoryStream();
			newImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
			return ms.ToArray();
		}

		public ActionResult Image(int id)
		{
			UserSubmittedImageRepository repo = new UserSubmittedImageRepository();
			var image = repo.Find(id);
			if (image != null)
				return File(image.Image, "image/jpeg");
			return Json(null, JsonRequestBehavior.AllowGet);
		}

		public ActionResult Boot()
		{
			return View();
		}

		public ActionResult Boot2()
		{
			return View();
		}

		#region NestedTypes
		public class RecipeIdWrapper
		{
			public string recipeId { get; set; }
		}
		#endregion
	}
}
