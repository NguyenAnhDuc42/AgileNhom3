using System;
using Microsoft.AspNetCore.Identity;

namespace Agile3.Models;

public class User : IdentityUser
{
    public List<CartItem> CartItems { get; set; }
}
