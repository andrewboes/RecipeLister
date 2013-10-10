using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingNut.Models
{
	public class UserSubmittedImage
	{
		public int Id { get; set; }
		public byte[] Image { get; set; }
	}
}