using System;
using System.Collections.Generic;

namespace BazaarBooks_ProgSprint.Models;

public partial class ShoppingCart
{
    public int CartItemId { get; set; }

    public string? Uuid { get; set; }

    public int? OrderId { get; set; }

    public string? Isbn { get; set; }

    public int? Quantity { get; set; }

    public bool? IsPurchased { get; set; }

    public virtual Book? IsbnNavigation { get; set; }

    public virtual Order? Order { get; set; }

    public virtual User Uu { get; set; } = null!;
}
