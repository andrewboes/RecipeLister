using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ShoppingNut.Models
{
	public class ShoppingListItem
	{
		public int Id { get; set; }
		public string Name { get; set; }
		//public double Quantity { get; set; }
		//public string QuantityType { get; set; }
		//public bool PickedUp { get; set; }
		public int Order { get; set; }

		//[ForeignKey("ShoppingList")]
		//public int ShoppingListId { get; set; }
		//public virtual ShoppingList ShoppingList { get; set; }

		[ForeignKey("Food")]
		public int? FoodId { get; set; }
		public virtual Food Food { get; set; }

		[NotMapped]
		public string UserItemName
		{
			get
			{
				if (this.FoodId.HasValue && this.FoodId != default(int))
					return this.Food.Name;
				return this.Name;
			}
			set
			{
				if (!this.FoodId.HasValue)
					this.Name = value;
			}
		}
	}
}