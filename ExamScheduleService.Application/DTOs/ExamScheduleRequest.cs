namespace ExamScheduleService.Application.DTOs;

/// <summary>
/// Represents the search criteria used to retrieve a student's exam schedule.
/// </summary>
public sealed class ExamScheduleRequest
{
    /// <summary>
    /// Gets or sets the student identifier.
    /// </summary>
    public int StudentId { get; set; }

    /// <summary>
    /// Gets or sets the academic session identifier.
    /// </summary>
    public int AcdSessId { get; set; }

    /// <summary>
    /// Gets or sets the optional subject code or subject name search value.
    /// </summary>
    public string? SubjectSearch { get; set; }
}
