using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ShoppingNut.Models
{
	public class RecipeImage
	{
		public int Id { get; set; }

		[ForeignKey("Recipe")]
		public int RecipeId { get; set; }
		public virtual Recipe Recipe { get; set; }

		[ForeignKey("Image")]
		public int UserSubmittedImageId { get; set; }
		public virtual UserSubmittedImage Image { get; set; }
	}
}