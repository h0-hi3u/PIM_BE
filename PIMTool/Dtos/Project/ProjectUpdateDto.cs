﻿using PIMTool.Dtos.Employee;
using System.ComponentModel.DataAnnotations;

namespace PIMTool.Dtos.Project;

public class ProjectUpdateDto
{
    public int Id { get; set; }

    [Required]
    public int GroupId { get; set; }

    [Required, Range(0, 999)]
    public int ProjectNumber { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; } = null!;

    [Required, MaxLength(50)]
    public string Customer { get; set; }
    public ICollection<EmployeeDto> Employees { get; set; }

    [Required, MaxLength(3)]
    public string Status { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }
    public byte[]? Version { get; set; }
}
