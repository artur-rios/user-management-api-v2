﻿using ArturRios.Common.Data;

namespace ArturRios.UserManagement.Domain.Filters;

public class UserMultiFilter : DataFilter
{
    public IEnumerable<int>? Ids { get; set; }
    public IEnumerable<string>? Names { get; set; }
    public IEnumerable<string>? Emails { get; set; }
    public IEnumerable<int>? RoleIds { get; set; }
    public IEnumerable<DateTime>? CreationDates { get; set; }
}
