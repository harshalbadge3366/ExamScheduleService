using AutoMapper;
using ExamScheduleService.Application.DTOs;
using ExamScheduleService.Application.Interfaces;
using ExamScheduleService.Application.Mappings;
using ExamScheduleService.Application.Validators;
using ExamScheduleService.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using ServiceUnderTest = ExamScheduleService.Application.Services.ExamScheduleService;

namespace ExamScheduleService.Tests.Services;

/// <summary>
/// Tests for exam schedule application business rules.
/// </summary>
public sealed class ExamScheduleServiceTests
{
    /// <summary>
    /// Verifies that the service returns all schedules supplied by the repository.
    /// </summary>
    [Fact]
    public async Task GetScheduleAsync_ReturnsData()
    {
        var request = new ExamScheduleRequest
        {
            StudentId = 1001,
            AcdSessId = 1
        };

        var repository = new Mock<IExamScheduleRepository>(MockBehavior.Strict);
        repository
            .Setup(repo => repo.GetScheduleAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ExamSchedule>
            {
                CreateSchedule(1, request.StudentId, request.AcdSessId, "CS101", "Computer Science", DateTime.Today.AddDays(2)),
                CreateSchedule(2, request.StudentId, request.AcdSessId, "MA101", "Mathematics", DateTime.Today.AddDays(4))
            });

        var service = new ServiceUnderTest(repository.Object,CreateMapper(), new ExamScheduleRequestValidator(),NullLogger<ServiceUnderTest>.Instance);
        var result = await service.GetScheduleAsync(request);

        result.Should().HaveCount(2);
        result.Select(schedule => schedule.SubjectCode).Should().Contain(new[] { "CS101", "MA101" });
        repository.Verify(repo => repo.GetScheduleAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifies that upcoming schedules include today's exams and exclude past exams.
    /// </summary>
    [Fact]
    public async Task GetUpcomingScheduleAsync_ReturnsOnlyFutureExams()
    {
        var request = new ExamScheduleRequest
        {
            StudentId = 1001,
            AcdSessId = 1
        };

        var repository = new Mock<IExamScheduleRepository>(MockBehavior.Strict);
        repository
            .Setup(repo => repo.GetScheduleAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ExamSchedule>
            {
                CreateSchedule(1, request.StudentId, request.AcdSessId, "HS101", "History", DateTime.Today.AddDays(-1)),
                CreateSchedule(2, request.StudentId, request.AcdSessId, "CS101", "Computer Science", DateTime.Today),
                CreateSchedule(3, request.StudentId, request.AcdSessId, "MA101", "Mathematics", DateTime.Today.AddDays(3))
            });

        var service = new ServiceUnderTest(repository.Object,CreateMapper(),new ExamScheduleRequestValidator(),NullLogger<ServiceUnderTest>.Instance);

        var result = await service.GetUpcomingScheduleAsync(request);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(schedule => schedule.ExamDate.Date >= DateTime.Today);
        result.Select(schedule => schedule.SubjectCode).Should().Equal("CS101", "MA101");
        repository.Verify(repo => repo.GetScheduleAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    private static IMapper CreateMapper()
    {
        var configuration = new MapperConfiguration(
            config => config.AddProfile<ExamScheduleProfile>(),
            NullLoggerFactory.Instance);
        configuration.AssertConfigurationIsValid();
        return configuration.CreateMapper();
    }

    private static ExamSchedule CreateSchedule(
        int examId,
        int studentId,
        int acdSessId,
        string subjectCode,
        string subjectName,
        DateTime examDate)
    {
        return new ExamSchedule
        {
            ExamId = examId,
            StudentId = studentId,
            AcdSessId = acdSessId,
            SubjectCode = subjectCode,
            SubjectName = subjectName,
            ExamDate = examDate,
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(12, 0, 0),
            RoomNo = "A-101",
            ExamType = "Theory"
        };
    }
}
