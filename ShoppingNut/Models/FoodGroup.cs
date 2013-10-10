using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingNut.Models
{
	public class FoodGroup
	{
		public int Id { get; set; }
		public string Description { get; set; }

		public virtual List<Food> Foods { get; set; }
	}
}