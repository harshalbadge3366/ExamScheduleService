using ExamScheduleService.Application.DTOs;
using FluentValidation;

namespace ExamScheduleService.Application.Validators
{
    public sealed class ExamScheduleRequestValidator : AbstractValidator<ExamScheduleRequest>
    {
        public ExamScheduleRequestValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0)
                .WithMessage("StudentId must be greater than 0.");

            RuleFor(x => x.AcdSessId)
                .GreaterThan(0)
                .WithMessage("Academic Session Id must be greater than 0.");

            RuleFor(x => x.SubjectSearch)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.SubjectSearch));
        }
    }
}
