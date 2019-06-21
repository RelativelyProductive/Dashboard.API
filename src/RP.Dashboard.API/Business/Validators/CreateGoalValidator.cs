using FluentValidation;
using RP.Dashboard.API.Models.Data.DB;

namespace RP.Dashboard.API.Validators
{
	public class CreateGoalValidator : AbstractValidator<Goal>
	{
		public CreateGoalValidator()
		{
			RuleFor(g => g.UserId).NotEmpty();
			RuleFor(g => g.DailyGoalTogglID).NotEmpty();
			RuleFor(g => g.DailyGoalTimeInMs).NotEmpty();
			RuleFor(g => g.DailyGoalTrackStyle).NotEmpty();
			RuleFor(g => g.GoalFriendlyName).NotEmpty();
		}
	}
}