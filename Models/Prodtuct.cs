using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Agile3.Models;

public class Product
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

}
