using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NienLuan.Api.Common;
using NienLuan.Api.Dtos;
using NienLuan.Api.Entities;
using NienLuan.Api.Services;

namespace NienLuan.Api.Controllers;

[ApiController]
[Route("api/users")]
[Produces("application/json")]
public class UsersController(IUserService userService) : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserResponse>> GetProfile(CancellationToken cancellationToken)
    {
        var result = await userService.GetProfileAsync(User.GetUserId(), cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = UserRole.Admin)]
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<UserResponse>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? role = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await userService.GetPagedAsync(page, pageSize, role, search, cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var requesterId = User.GetUserId();
        var requesterRole = User.GetUserRole();

        if (requesterRole != UserRole.Admin && requesterId != id)
        {
            throw ApiExceptionFactory.Forbidden("Không có quyền xem tài khoản này.");
        }

        var result = await userService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = UserRole.Admin)]
    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<UserResponse>> Create([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await userService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [Authorize(Roles = UserRole.Admin)]
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserResponse>> Update(int id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await userService.UpdateAsync(id, request, User.GetUserId(), cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = UserRole.Admin)]
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetActive(int id, [FromBody] SetActiveRequest request, CancellationToken cancellationToken)
    {
        await userService.SetActiveAsync(id, request.IsActive, User.GetUserId(), cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = UserRole.Admin)]
    [HttpPost("{id:int}/reset-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ResetPassword(int id, [FromBody] AdminResetPasswordRequest request, CancellationToken cancellationToken)
    {
        await userService.ResetPasswordAsync(id, request.NewPassword, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = UserRole.Admin)]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await userService.DeleteAsync(id, User.GetUserId(), cancellationToken);
        return NoContent();
    }
}

public class SetActiveRequest
{
    [Required]
    public bool IsActive { get; set; }
}

public class AdminResetPasswordRequest
{
    [Required]
    [StringLength(128, MinimumLength = 6)]
    public string NewPassword { get; set; } = string.Empty;
}
