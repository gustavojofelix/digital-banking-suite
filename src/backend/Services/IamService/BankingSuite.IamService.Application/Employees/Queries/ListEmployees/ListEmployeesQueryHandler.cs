using BankingSuite.BuildingBlocks.Application.Models;
using BankingSuite.IamService.Application.Employees.Dtos;
using BankingSuite.IamService.Application.Employees.Queries;
using BankingSuite.IamService.Domain;
using BankingSuite.IamService.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BankingSuite.IamService.Application.Employees.Queries.ListEmployees;

public sealed class ListEmployeesQueryHandler
    : IRequestHandler<ListEmployeesQuery, PagedResult<EmployeeSummaryDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ListEmployeesQueryHandler(UserManager<ApplicationUser> userManager) =>
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

    public async Task<PagedResult<EmployeeSummaryDto>> Handle(
        ListEmployeesQuery request,
        CancellationToken cancellationToken
    )
    {
        var query = _userManager.Users.AsNoTracking();

        if (!request.IncludeInactive)
        {
            query = query.Where(u => u.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLowerInvariant();

            // Remove usage of EF.Functions.ILike, which is not available.
            // Use ToLower().Contains for case-insensitive search instead.
            query = query.Where(u =>
                (u.Email != null && u.Email.ToLower().Contains(search)) ||
                u.FirstName.ToLower().Contains(search) ||
                u.LastName.ToLower().Contains(search) ||
                ((u.FirstName + " " + u.LastName).Trim().ToLower().Contains(search))
            );
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .OrderBy(u => u.Email)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var items = new List<EmployeeSummaryDto>(users.Count);

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            items.Add(
                new EmployeeSummaryDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName,
                    EmailConfirmed = user.EmailConfirmed,
                    IsActive = user.IsActive,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    Roles = roles.ToArray(),
                }
            );
        }

        return new PagedResult<EmployeeSummaryDto>(
            Items: items,
            PageNumber: request.PageNumber,
            PageSize: request.PageSize,
            TotalCount: totalCount
        );
    }
}
