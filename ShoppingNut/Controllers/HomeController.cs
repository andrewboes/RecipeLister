using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Authentication;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using CsQuery;
using HtmlAgilityPack;
using ShoppingNut.Models;
using System.IO;

namespace ShoppingNut.Controllers
{
	/// <summary>
	/// $entityTypes = "Food", "Ingredient", "Instruction", "InstructionItem", "QuantityType", "Recipe", "RecipeImage", "UserSubmittedImage", "FoodGroup"
	/// foreach($type in $entityTypes){Scaffold Repository -ModelType $type -Force}
	/// To update database when you get a message "Model backing the blah blah has changed since the database was created. blah"
	/// 
	/// </summary>
	[OutputCache(Location = OutputCacheLocation.None)]
	public class HomeController : Controller
	{

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult GetAllRecipes()
		{
			RecipeRepository recipes = new RecipeRepository();
			int id = this.GetUserId();
			var recipe = recipes.All.ToList();
			List<object> v = new List<object>();
			recipe.ForEach(x => v.Add(x.ToJsonLite()));
			return Json(v, JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetUserRecipes()
		{
			RecipeRepository recipes = new RecipeRepository();
			int id = this.GetUserId();
			var recipe = recipes.AllIncluding(x => x.Images).Where(x => x.UserId == id).ToList();
			List<object> v = new List<object>();
			recipe.ForEach(x => v.Add(x.ToJsonLite()));
			return Json(v, JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetAllRecipesWithIngredients()
		{
			RecipeRepository recipes = new RecipeRepository();
			int id = this.GetUserId();
			var recipe = recipes.AllIncluding(x => x.Ingredients).Where(x => x.UserId == id).ToList();
			List<object> v = new List<object>();
			recipe.ForEach(x => v.Add(x.ToJson()));
			return Json(v, JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetShoppingLists()
		{
			var lists = new ShoppingListRepository();
			int id = this.GetUserId();
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
			foodsJson.AddRange(this.FoodsSearch(filters, foods));
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

		private List<Food> FoodsSearch(string[] filters, FoodRepository foods)
		{
			var foodSearchOne = foods.All;
			foreach (var s in filters)
			{
				foodSearchOne = foodSearchOne.Where(x => x.Name.Contains(s) || x.CommonName.Contains(s));
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

		[HttpPost]
		public ActionResult DeleteList(int id)
		{
			if (!this.User.Identity.IsAuthenticated)
				throw new Exception("User not logged in");
			try
			{
				ShoppingListRepository repository = new ShoppingListRepository();
				repository.Delete(id);
				repository.Save();
				return Json(new { Success = true }, JsonRequestBehavior.DenyGet);
			}
			catch (Exception ex)
			{
				return Json(new { Success = false, ex.Message }, JsonRequestBehavior.DenyGet);
			}
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
			UserProfileRepository users = new UserProfileRepository();
			recipe.UserId = users.All.Single(x => x.UserName == this.User.Identity.Name).UserId;
			RecipeRepository recipes = new RecipeRepository();
			recipes.InsertOrUpdate(recipe);
			recipes.Save();
			return Json(new { Success = true, RecipeId = recipe.Id }, JsonRequestBehavior.AllowGet);
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
			catch (Exception ex)
			{
				return Json(new { Success = false, ex.Message }, JsonRequestBehavior.AllowGet);
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
				list.UserId = this.GetUserId();
				ShoppingListRepository lists = new ShoppingListRepository();
				lists.InsertOrUpdate(list);
				lists.Save();
				return Json(new { Success = true, ShoppingListId = list.Id }, JsonRequestBehavior.AllowGet); 
			}
			catch (Exception ex)
			{
				return Json(new { Success = false, ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}

		public ActionResult InsertOrUpdateShoppingListItem(ShoppingListItem item)
		{
			if (!this.User.Identity.IsAuthenticated)
				throw new Exception("User not logged in");
			try
			{
				if (item.ShoppingListId == default(int))
				{
					ShoppingList list = new ShoppingList();
					list.UserId = this.GetUserId();
					ShoppingListRepository lists = new ShoppingListRepository();
					lists.InsertOrUpdate(list);
					lists.Save();
					item.ShoppingListId = list.Id;
				}
				item.Food = null;
				item.QuantityType = null;
				item.DatabaseQuantityType = null;
				ShoppingListItemRepository repo = new ShoppingListItemRepository();
				repo.InsertOrUpdate(item);
				repo.Save();
				return Json(new { Success = true, item.ShoppingListId, ShoppingListItemId = item.Id }, JsonRequestBehavior.AllowGet); 

			}
			catch (Exception ex)
			{
				return Json(new { Success = false, ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}

		public ActionResult DeleteShoppingListItem(int id)
		{
			if (!this.User.Identity.IsAuthenticated)
				throw new Exception("User not logged in");
			try
			{
				ShoppingListItemRepository repository = new ShoppingListItemRepository();
				repository.Delete(id);
				repository.Save();
				return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { Success = false, ex.Message }, JsonRequestBehavior.AllowGet);
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

		public ActionResult GetFoodAllGroups()
		{
			FoodGroupRepository repo = new FoodGroupRepository();
			var groups = repo.All.OrderBy(x => x.Description).Select(x => new { x.Id, x.Description }).ToList();
			return Json(groups, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult InsertFood(Food food)
		{
			food.FoodGroupId = food.FoodGroup.Id;
			food.FoodGroup = null;
			food.SourceId = this.GetUserEnteredFoodSourceId();
			FoodRepository repo = new FoodRepository();
			if (repo.All.Any(x => x.Name.Contains(food.Name)))
				return Json(new {Success = false, Message = string.Format("Food with name '{0}' already exists", food.Name)}, JsonRequestBehavior.AllowGet);
			repo.InsertOrUpdate(food);
			repo.Save();
			return Json(new {Success = true, @object = food.ToJson()}, JsonRequestBehavior.DenyGet);
		}

		public ActionResult Scrape(string url)
		{
			if(string.IsNullOrEmpty(url))
			 url = "http://thepioneerwoman.com/cooking/2013/10/carb-buster-breakfast/";
			HtmlWeb web = new HtmlWeb();
			HtmlDocument doc = web.Load(url);
			CQ cqDoc = doc.DocumentNode.OuterHtml;
			var Name = this.GetName(cqDoc);
			var Servings = this.GetServings(cqDoc);
			var ingredients = this.GetIngredients(cqDoc);
			List<Tuple<string, string>> Ingredients = new List<Tuple<string, string>>();
			foreach (var ingredient in ingredients)
			{
				CQ ing = CQ.Create(ingredient);
				var amount = this.GetIngredientAmount(ing);
				var name = this.GetIngredientName(ing);
				Ingredients.Add(new Tuple<string, string>(amount, name));
			}
			var instructionDiv = this.GetInstuctionBlock(cqDoc);
			var instructionPs = this.GetInstructions(instructionDiv);
			List<string> Instructions = instructionPs.Select(instruction => instruction.InnerText).ToList();
			return Json(new { Name, Servings, Url = url, Ingredients, Instructions }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult TouchTest()
		{
			return View();
		}

		public ActionResult TouchSort()
		{
			return View();
		}

		public ActionResult SortTest()
		{
			return View();
		}

		#region Private Methods
		private int GetUserId()
		{
			var users = new UserProfileRepository();
			var id = users.All.Single(x => x.UserName == User.Identity.Name).UserId;
			return id;
		}

		private List<IDomObject> GetInstuctionBlock(CQ cqDoc)
		{
			var instructions = cqDoc["div [itemprop='instructions']"].ToList();
			if (!instructions.Any())
				instructions = cqDoc[".directions"].ToList();
			return instructions;
		}

		private CQ GetInstructions(List<IDomObject> instructionDiv)
		{
			var instructions = CQ.Create(instructionDiv)["p"];
			if (!instructions.Any())
				instructions = CQ.Create(instructionDiv)["li span"];
			return instructions;
		}

		private string GetIngredientName(CQ ing)
		{
			string name = ing["[itemprop='name']"].Text();
			if (string.IsNullOrEmpty(name))
				name = ing[".ingredient-name"].Text();
			return name;
		}

		private string GetIngredientAmount(CQ ing)
		{
			string amount = ing["[itemprop='amount']"].Text();
			if (string.IsNullOrEmpty(amount))
				amount = ing[".ingredient-amount"].Text();
			return amount;
		}

		private List<IDomObject> GetIngredients(CQ cqDoc)
		{
			var ingredients = cqDoc["[itemprop='ingredient']"].ToList();
			if (!ingredients.Any())
				ingredients = cqDoc["[itemprop='ingredients']"].ToList();
			return ingredients;
		}

		private string GetServings(CQ cqDoc)
		{
			string servings = cqDoc["[itemprop='yield']"].Text();
			if (string.IsNullOrEmpty(servings))
				servings = cqDoc["[itemprop='recipeYield']"].Text();
			return servings;
		}

		private string GetName(CQ cqDoc)
		{
			string name = cqDoc[".recipe-title"].Text();
			if (string.IsNullOrEmpty(name))
				name = cqDoc["[itemprop='name']"].Text();
			return name;
		}

		private int GetUserEnteredFoodSourceId()
		{
			return 2;
		}
		#endregion

		#region NestedTypes
		public class RecipeIdWrapper
		{
			public string recipeId { get; set; }
		}
		#endregion
	}
}
