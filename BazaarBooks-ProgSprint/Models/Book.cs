using System;
using System.Collections.Generic;

namespace BazaarBooks_ProgSprint.Models;

public partial class Book
{
    public string Isbn { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public double Price { get; set; }

    public string Author { get; set; } = null!;

    public string Genre { get; set; } = null!;

    public int AvailableQuantity { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();
}
