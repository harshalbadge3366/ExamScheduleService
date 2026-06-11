using AutoMapper;
using ExamScheduleService.Application.DTOs;
using ExamScheduleService.Domain.Entities;

namespace ExamScheduleService.Application.Mappings;

/// <summary>
/// Configures AutoMapper mappings for exam schedule objects.
/// </summary>
public sealed class ExamScheduleProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExamScheduleProfile"/> class.
    /// </summary>
    public ExamScheduleProfile()
    {
        CreateMap<ExamSchedule, ExamScheduleDto>();
    }
}
