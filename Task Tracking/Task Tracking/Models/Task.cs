using System;
using System.Collections.Generic;

namespace Task_Tracking.Models;

public partial class Task
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Status { get; set; }

    public int UserId { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public string? Description { get; set; }

    public DateOnly? DueDate { get; set; }

    public virtual User User { get; set; } = null!;
}
