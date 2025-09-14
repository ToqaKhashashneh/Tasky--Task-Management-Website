using System;
using System.Collections.Generic;

namespace Task_Tracking.Models;

public partial class Contact
{
    public int Id { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Message { get; set; }
}
