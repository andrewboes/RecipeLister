using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ShoppingNut.Models
{
	public class Ingredient
	{
		public int Id { get; set; }
		public double Quantity { get; set; }
		public string Notes { get; set; }

		[ForeignKey("QuantityType")]
		public int? QuantityTypeId { get; set; }
		public virtual QuantityType QuantityType { get; set; }

		[ForeignKey("Recipe")]
		public int RecipeId { get; set; }
		public virtual Recipe Recipe { get; set; }

		[ForeignKey("Food")]
		public int FoodId { get; set; }
		public virtual Food Food { get; set; }
	}
}
