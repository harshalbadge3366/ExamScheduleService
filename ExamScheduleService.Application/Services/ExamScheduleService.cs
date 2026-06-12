using AutoMapper;
using ExamScheduleService.Application.DTOs;
using ExamScheduleService.Application.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ExamScheduleService.Application.Services;

/// <summary>
/// Provides exam schedule application use cases.
/// </summary>
public sealed class ExamScheduleService : IExamScheduleService
{
    private readonly IExamScheduleRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<ExamScheduleRequest> _validator;
    private readonly ILogger<ExamScheduleService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExamScheduleService"/> class.
    /// </summary>
    /// <param name="repository">The exam schedule repository.</param>
    /// <param name="mapper">The mapper used to transform domain entities to DTOs.</param>
    /// <param name="validator">The validator for exam schedule requests.</param>
    /// <param name="logger">The logger.</param>
    public ExamScheduleService(IExamScheduleRepository repository,IMapper mapper,IValidator<ExamScheduleRequest> validator,ILogger<ExamScheduleService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _validator = validator;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ExamScheduleDto>> GetScheduleAsync(ExamScheduleRequest request,CancellationToken cancellationToken = default)
    {     

        try
        {
            ArgumentNullException.ThrowIfNull(request);

            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var schedules = await _repository.GetScheduleAsync(request,cancellationToken);

            return _mapper.Map<IReadOnlyCollection<ExamScheduleDto>>(schedules.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving exam schedule for StudentId {StudentId}", request.StudentId);

            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ExamScheduleDto>> GetUpcomingScheduleAsync(ExamScheduleRequest request,CancellationToken cancellationToken = default)
    {
            

            try
            {
                ArgumentNullException.ThrowIfNull(request);

                await _validator.ValidateAndThrowAsync(request, cancellationToken);
                var schedules = await _repository.GetScheduleAsync(request,cancellationToken);

                var upcomingSchedules = schedules.Where(x => x.ExamDate.Date >= DateTime.Today).OrderBy(x => x.ExamDate).ThenBy(x => x.StartTime).ToList();

                return _mapper.Map<IReadOnlyCollection<ExamScheduleDto>>(upcomingSchedules);
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error retrieving upcoming exam schedule for StudentId: {StudentId}", request.StudentId);
                 throw;
             }


    }

}
