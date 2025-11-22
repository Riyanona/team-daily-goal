// T059: CreateGoalRequestValidator with FluentValidation rules
using FluentValidation;
using TeamGoalTracker.Api.Models;

namespace TeamGoalTracker.Api.Validation;

public class CreateGoalRequestValidator : AbstractValidator<CreateGoalRequest>
{
    public CreateGoalRequestValidator()
    {
        RuleFor(x => x.TeamMemberId)
            .GreaterThan(0)
            .WithMessage("TeamMemberId must be greater than 0");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.Date)
            .Must(BeValidDate)
            .When(x => x.Date.HasValue)
            .WithMessage("Date must be within one year of today");
    }

    private bool BeValidDate(DateTime? date)
    {
        if (!date.HasValue) return true;
        var value = date.Value.Date;
        return value >= DateTime.UtcNow.Date.AddYears(-1) && value <= DateTime.UtcNow.Date.AddYears(1);
    }
}
