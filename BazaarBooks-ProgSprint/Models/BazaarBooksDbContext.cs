using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BazaarBooks_ProgSprint.Models;

public partial class BazaarBooksDbContext : DbContext
{
    public BazaarBooksDbContext()
    {
    }

    public BazaarBooksDbContext(DbContextOptions<BazaarBooksDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=labVMH8OX\\SQLEXPRESS;Initial Catalog=BazaarBooksDB; Encrypt=False; Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Isbn).HasName("PK__book__447D36EB248FE8BF");

            entity.ToTable("book");

            entity.Property(e => e.Isbn)
                .HasMaxLength(225)
                .IsUnicode(false)
                .HasColumnName("ISBN");
            entity.Property(e => e.Author)
                .HasMaxLength(225)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(225)
                .IsUnicode(false);
            entity.Property(e => e.Genre)
                .HasMaxLength(225)
                .IsUnicode(false);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(225)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(225)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAFF715AA6D");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.Uuid)
                .HasMaxLength(225)
                .IsUnicode(false)
                .HasColumnName("UUID");

            entity.HasOne(d => d.Uu).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Uuid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__UUID__5165187F");
        });

        modelBuilder.Entity<ShoppingCart>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("PK__Shopping__488B0B2ADB377A83");

            entity.ToTable("ShoppingCart");

            entity.Property(e => e.CartItemId).HasColumnName("CartItemID");
            entity.Property(e => e.IsPurchased).HasColumnName("isPurchased");
            entity.Property(e => e.Isbn)
                .HasMaxLength(225)
                .IsUnicode(false)
                .HasColumnName("ISBN");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.Uuid)
                .HasMaxLength(225)
                .IsUnicode(false)
                .HasColumnName("UUID");

            entity.HasOne(d => d.IsbnNavigation).WithMany(p => p.ShoppingCarts)
                .HasForeignKey(d => d.Isbn)
                .HasConstraintName("FK__ShoppingCa__ISBN__571DF1D5");

            entity.HasOne(d => d.Order).WithMany(p => p.ShoppingCarts)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__ShoppingC__Order__5535A963");

            entity.HasOne(d => d.Uu).WithMany(p => p.ShoppingCarts)
                .HasForeignKey(d => d.Uuid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ShoppingCa__UUID__5629CD9C");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Uuid).HasName("PK__Users__65A475E7B5438336");

            entity.Property(e => e.Uuid)
                .HasMaxLength(225)
                .IsUnicode(false)
                .HasColumnName("UUID");
            entity.Property(e => e.Email)
                .HasMaxLength(225)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(225)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(225)
                .IsUnicode(false);
            entity.Property(e => e.Level).HasDefaultValue(1);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
