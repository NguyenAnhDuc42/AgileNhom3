using Microsoft.EntityFrameworkCore;
using System;

namespace Agile3.Models;

public class RegisterViewModel
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
