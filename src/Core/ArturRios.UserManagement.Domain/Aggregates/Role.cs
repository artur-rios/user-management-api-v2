using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ArturRios.Common.Data;

namespace ArturRios.UserManagement.Domain.Aggregates;

public class Role : Entity
{
    [Column(Order = 2)] [MaxLength(300)] public string Name { get; set; } = string.Empty;

    [Column(Order = 3)] [MaxLength(300)] public string Description { get; set; } = string.Empty;
}
