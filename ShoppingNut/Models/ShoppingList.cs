using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ShoppingNut.Models
{
	public class ShoppingList
	{
		public int Id { get; set; }
		public string Name { get; set; }

		[ForeignKey("User")]
		public int UserId { get; set; }
		public virtual UserProfile User { get; set; }

		public virtual List<ShoppingListItem> Items { get; set; }

		internal object ToJson()
		{
			var Items = this.Items.Select(x => new { x.Id, x.FoodId, Name = x.UserItemName, x.Quantity, QuantityType = x.UserQuantityType });
			return new { this.Id, this.Name, Items };
		}
	}
}