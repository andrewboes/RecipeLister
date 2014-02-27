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
		public DateTime Created { get; set; }

		[ForeignKey("User")]
		public int UserId { get; set; }
		public virtual UserProfile User { get; set; }

		public virtual List<ShoppingListShoppingListItem> Items { get; set; }

		internal object ToJson()
		{
			var Items = this.Items.Select(x => new
			{
				x.Id,
				ShoppingListItem = new
				{
					x.ShoppingListItem.Id,
					x.ShoppingListItem.Order,
					x.ShoppingListItem.Name,
					Food = x.ShoppingListItem.Food != null ? x.ShoppingListItem.Food.ToJson() : null,
					x.ShoppingListItem.FoodId,
					Group = x.ShoppingListItem.Food != null ? new { x.ShoppingListItem.Food.FoodGroup.Description, x.ShoppingListItem.Food.FoodGroup.Id } : new { Description = "Other", Id = 0 }
				},
				x.Quantity,
				QuantityType = x.DatabaseQuantityType != null ? new { x.DatabaseQuantityType.Name, x.DatabaseQuantityType.Id } : new { Name = string.Empty, Id = 0 },
				x.QuantityTypeId,
				x.PickedUp,
				x.ShoppingListId
			});
			return new { this.Id, this.Name, this.Created, Items };
		}
	}
}