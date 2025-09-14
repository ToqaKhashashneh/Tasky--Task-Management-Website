using System;
using System.Collections.Generic;

namespace Task_Tracking.Models;

public partial class User
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string? Email { get; set; }

    public string Password { get; set; } = null!;

    public string? ProfilePicture { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
