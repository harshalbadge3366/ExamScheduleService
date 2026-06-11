using ExamScheduleService.Application.Common;
using ExamScheduleService.Application.DTOs;
using ExamScheduleService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamScheduleService.API.Controllers;

/// <summary>
/// Provides secured endpoints for student exam schedules.
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class ExamScheduleController : ControllerBase
{
    private readonly IExamScheduleService _examScheduleService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExamScheduleController"/> class.
    /// </summary>
    /// <param name="examScheduleService">The exam schedule application service.</param>
    public ExamScheduleController(IExamScheduleService examScheduleService)
    {
        _examScheduleService = examScheduleService;
    }

    /// <summary>
    /// Gets all exam schedules for the authenticated student.
    /// </summary>
    /// <param name="studentId">The student identifier.</param>
    /// <param name="acdSessId">The academic session identifier.</param>
    /// <param name="subjectSearch">The optional subject code or name search text.</param>
    /// <param name="cancellationToken">A token used to cancel the request.</param>
    /// <returns>The matching exam schedules.</returns>
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<ExamScheduleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<ExamScheduleDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<ExamScheduleDto>>>>GetScheduleAsync([FromQuery] int studentId,int acdSessId,string? subjectSearch,CancellationToken cancellationToken)
    {
        if (!IsCurrentStudent(studentId))
        {
            return Forbid();
        }

        var request = new ExamScheduleRequest
        {
            StudentId = studentId,
            AcdSessId = acdSessId,
            SubjectSearch = subjectSearch
        };

        var schedules = await _examScheduleService.GetScheduleAsync(request, cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<ExamScheduleDto>>.Ok(schedules, "Exam schedule retrieved successfully."));
    }

    /// <summary>
    /// Gets upcoming exam schedules for the authenticated student.
    /// </summary>
    /// <param name="studentId">The student identifier.</param>
    /// <param name="acdSessId">The academic session identifier.</param>
    /// <param name="cancellationToken">A token used to cancel the request.</param>
    /// <returns>The upcoming exam schedules.</returns>
    [HttpGet("upcoming")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<ExamScheduleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<ExamScheduleDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<ExamScheduleDto>>>> GetUpcomingScheduleAsync([FromQuery] int studentId, int acdSessId,CancellationToken cancellationToken)
    {
        if (!IsCurrentStudent(studentId))
        {
            return Forbid();
        }

        var request = new ExamScheduleRequest
        {
            StudentId = studentId,
            AcdSessId = acdSessId
        };

        var schedules = await _examScheduleService.GetUpcomingScheduleAsync(request, cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<ExamScheduleDto>>.Ok(schedules,"Upcoming exam schedule retrieved successfully."));
    }

    private bool IsCurrentStudent(int studentId)
    {
        var studentIdClaim = User.FindFirst("STUDENT_ID")?.Value;
        return int.TryParse(studentIdClaim, out var authenticatedStudentId)
            && authenticatedStudentId == studentId;
    }
}
