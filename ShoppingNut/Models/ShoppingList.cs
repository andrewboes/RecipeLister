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

		public virtual List<ShoppingListItem> Items { get; set; }

		internal object ToJson()
		{
			var Items = this.Items.Select(x => new
			{
				Order = x.Id,
				x.Id,
				x.FoodId,
				Name = x.UserItemName,
				x.Quantity,
				Food = x.Food != null ? x.Food.ToJson() : null,
				QuantityType = x.DatabaseQuantityType != null ? new { x.DatabaseQuantityType.Name, x.DatabaseQuantityType.Id } : new { Name = string.Empty, Id = 0 },
				x.QuantityTypeId,
				x.PickedUp,
				x.ShoppingListId,
				Group = x.Food != null ? new { x.Food.FoodGroup.Description, x.Food.FoodGroup.Id } : new { Description = "Other", Id = 0 }
			});
			return new { this.Id, this.Name, this.Created, Items };
		}
	}
}