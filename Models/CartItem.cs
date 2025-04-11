using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Agile3.Models;

public class CartItem
{
    [Key]
    public int id { get; set; }
    public string UserId { get; set; }
    public int  ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Total { get; set; }

    public Product Product { get; set; }
    public User User { get; set; }
}
