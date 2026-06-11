using ExamScheduleService.Application.DTOs;

namespace ExamScheduleService.Application.Interfaces;

/// <summary>
/// Defines application operations for student exam schedules.
/// </summary>
public interface IExamScheduleService
{
    /// <summary>
    /// Retrieves the complete exam schedule for a student and academic session.
    /// </summary>
    /// <param name="request">The schedule search criteria.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The complete matching schedule.</returns>
    Task<IReadOnlyCollection<ExamScheduleDto>> GetScheduleAsync(
        ExamScheduleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves only upcoming exams for a student and academic session.
    /// </summary>
    /// <param name="request">The schedule search criteria.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The upcoming matching schedule.</returns>
    Task<IReadOnlyCollection<ExamScheduleDto>> GetUpcomingScheduleAsync(
        ExamScheduleRequest request,
        CancellationToken cancellationToken = default);
}
