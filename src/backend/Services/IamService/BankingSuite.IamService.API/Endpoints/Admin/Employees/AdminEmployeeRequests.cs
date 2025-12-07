namespace BankingSuite.IamService.Api.Endpoints.Admin.Employees;

public sealed class ListEmployeesRequest
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Search { get; init; }
    public bool IncludeInactive { get; init; } = true;
}

public sealed class EmployeeByIdRequest
{
    public Guid Id { get; init; } // bound from route {id}
}

public sealed class UpdateEmployeeRequest
{
    public Guid Id { get; init; } // route {id}

    public string? FullName { get; init; }
    public string? PhoneNumber { get; init; }
    public string[] Roles { get; init; } = [];
}

public sealed class EmployeeStatusRequest
{
    public Guid Id { get; init; } // route {id}
}
