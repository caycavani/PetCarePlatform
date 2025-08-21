using FluentValidation;
using PetCare.Notification.Application.DTOs;
using PetCare.Notification.Application.DTOs.Requests;

public class CreateNotificationValidator : AbstractValidator<CreateNotificationDto>
{
    public CreateNotificationValidator()
    {
        RuleFor(x => x.RecipientName).NotEmpty();
        RuleFor(x => x.RecipientEmail).EmailAddress().When(x => !string.IsNullOrEmpty(x.RecipientEmail));
        RuleFor(x => x.RecipientPhone).Matches(@"^\+\d{10,15}$").When(x => !string.IsNullOrEmpty(x.RecipientPhone));
        RuleFor(x => x.Subject).NotEmpty();
        RuleFor(x => x.Message).NotEmpty();
        RuleFor(x => x.Channel).Must(c => new[] { "Email", "SMS", "Push" }.Contains(c));
    }
}
