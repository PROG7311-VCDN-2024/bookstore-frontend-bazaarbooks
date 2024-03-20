using System;
using System.Collections.Generic;

namespace BazaarBooks_ProgSprint.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public string Uuid { get; set; } = null!;

    public DateTime? PurchaseDate { get; set; }

    public double? Total { get; set; }

    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();

    public virtual User Uu { get; set; } = null!;
}
