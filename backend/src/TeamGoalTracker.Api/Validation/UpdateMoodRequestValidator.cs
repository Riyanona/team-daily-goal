// T076: UpdateMoodRequestValidator with FluentValidation rules
using FluentValidation;
using TeamGoalTracker.Api.Models;
using TeamGoalTracker.Core.Entities;

namespace TeamGoalTracker.Api.Validation;

public class UpdateMoodRequestValidator : AbstractValidator<UpdateMoodRequest>
{
    public UpdateMoodRequestValidator()
    {
        RuleFor(x => x.TeamMemberId)
            .GreaterThan(0)
            .WithMessage("TeamMemberId must be greater than 0");

        RuleFor(x => x.MoodType)
            .IsInEnum()
            .WithMessage("Invalid mood type");
    }
}
