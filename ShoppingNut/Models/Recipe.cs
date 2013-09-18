﻿using System;
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
		public double Servings { get; set; }

		[ForeignKey("User")]
		public int UserId { get; set; }
		public virtual UserProfile User { get; set; }
		
		public virtual List<Instruction> Instructions { get; set; }
		public virtual List<Ingredient> Ingredients { get; set; }

		public object ToJson()
		{
			var ing = this.Ingredients.Select(x => new
			{
				x.Id,
				x.FoodId,
				x.QuantityTypeId, 
				Food = new { Name = x.Food.Name }, 
				x.Quantity,
				QuantityType = new { Name = x.QuantityType.Name, Id = x.QuantityType.Id },
				Calories = ((x.QuantityType.Grams * x.Food.Calories) / 100),
				x.RecipeId
			}).ToList();
			var Instructions = this.Instructions.Select(x => new {x.Id, Order = x.Order, Text = x.Text, x.RecipeId});
			return new { this.Id, this.Name, this.Servings, this.Description, Ingredients = ing, Instructions };
		}
	}
}