using ExamScheduleService.Application.DTOs;
using ExamScheduleService.Domain.Entities;

namespace ExamScheduleService.Application.Interfaces;

/// <summary>
/// Defines persistence operations for exam schedule data.
/// </summary>
public interface IExamScheduleRepository
{
    /// <summary>
    /// Retrieves a student's exam schedule from the data store.
    /// </summary>
    /// <param name="request">The schedule search criteria.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The matching exam schedule rows.</returns>
    Task<IEnumerable<ExamSchedule>> GetScheduleAsync(
        ExamScheduleRequest request,
        CancellationToken cancellationToken = default);
}
