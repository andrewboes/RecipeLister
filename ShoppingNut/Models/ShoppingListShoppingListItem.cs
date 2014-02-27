using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ShoppingNut.Models
{
	public class ShoppingListShoppingListItem
	{
		public int Id { get; set; }
		public double Quantity { get; set; }
		public string QuantityType { get; set; }
		public bool PickedUp { get; set; }

		[ForeignKey("ShoppingList")]
		public int ShoppingListId { get; set; }
		public virtual ShoppingList ShoppingList { get; set; }

		[ForeignKey("ShoppingListItem")]
		public int ShoppingListItemId { get; set; }
		public virtual ShoppingListItem ShoppingListItem { get; set; }

		[ForeignKey("DatabaseQuantityType")]
		public int? QuantityTypeId { get; set; }
		public virtual QuantityType DatabaseQuantityType { get; set; }
	}
}