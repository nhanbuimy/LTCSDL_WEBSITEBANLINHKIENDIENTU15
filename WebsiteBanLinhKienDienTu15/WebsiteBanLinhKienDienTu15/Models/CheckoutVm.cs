﻿namespace WebsiteBanLinhKienDienTu15.Models
{
	public class CheckoutVm
	{
		public Order Order { get; set; }
		public List<CartItemVm> CartItems { get; set; }
	}
}
