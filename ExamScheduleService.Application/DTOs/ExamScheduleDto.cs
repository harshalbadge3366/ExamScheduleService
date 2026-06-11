namespace ExamScheduleService.Application.DTOs;

/// <summary>
/// Represents an exam schedule item returned to API clients.
/// </summary>
public sealed class ExamScheduleDto
{
    /// <summary>
    /// Gets or sets the unique exam schedule identifier.
    /// </summary>
    public int ExamId { get; set; }

    /// <summary>
    /// Gets or sets the student identifier.
    /// </summary>
    public int StudentId { get; set; }

    /// <summary>
    /// Gets or sets the academic session identifier.
    /// </summary>
    public int AcdSessId { get; set; }

    /// <summary>
    /// Gets or sets the subject code.
    /// </summary>
    public string SubjectCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the subject name.
    /// </summary>
    public string SubjectName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the exam date.
    /// </summary>
    public DateTime ExamDate { get; set; }

    /// <summary>
    /// Gets or sets the exam start time.
    /// </summary>
    public TimeSpan StartTime { get; set; }

    /// <summary>
    /// Gets or sets the exam end time.
    /// </summary>
    public TimeSpan EndTime { get; set; }

    /// <summary>
    /// Gets or sets the exam room number.
    /// </summary>
    public string RoomNo { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the exam type.
    /// </summary>
    public string ExamType { get; set; } = string.Empty;
}
