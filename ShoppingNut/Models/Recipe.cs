using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ShoppingNut.Models
{
	public class Recipe
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public double? Servings { get; set; }
		public string Url { get; set; }
		public string ActiveTime { get; set; }
		public string TotalTime { get; set; }
		public DateTime Created { get; set; }
		public string Notes { get; set; }

		[ForeignKey("User")]
		public int UserId { get; set; }
		public virtual UserProfile User { get; set; }

		public virtual List<RecipeImage> Images { get; set; }
		public virtual List<Instruction> Instructions { get; set; }
		public virtual List<Ingredient> Ingredients { get; set; }

		public object ToJson()
		{
			var ing = this.Ingredients.Select(x => new
			{
				x.Id,
				x.FoodId,
				x.QuantityTypeId,
				Food = new { Name = x.Food.Name, QuantityTypes = x.Food.QuantityTypes.Select(y => new { y.Name, y.Id }) },
				x.Quantity,
				QuantityType = new { Name = x.QuantityType.Name, Id = x.QuantityType.Id },
				Calories = ((x.QuantityType.Grams * x.Food.Calories) / 100),
				x.RecipeId,
				x.Notes
			}).ToList();
			var Instructions = this.Instructions.Select(x => new { x.Id, Order = x.Order, Text = x.Text, x.RecipeId });
			var Images = this.Images.Select(x => new { x.Id, x.RecipeId, x.UserSubmittedImageId });
			string name = this.Name;
			if (string.IsNullOrWhiteSpace(name))
				name = this.Created.ToShortDateString();
			return new { this.Id, Name = name, this.Servings, this.Description, this.Url, this.ActiveTime, this.TotalTime, this.Created, this.Notes, Ingredients = ing, Instructions, Images };
		}

		public object ToJsonLite()
		{
			var img = this.Images.Select(x => x.UserSubmittedImageId).ToList();
			return new { this.Id, this.Name, this.Description, Images = img };
		}
	}
}