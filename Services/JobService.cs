using Hangfire;

namespace Chefster.Services;

public class JobService(
    ConsiderationsService considerationsService,
    FamilyService familyService,
    EmailService emailService,
    MemberService memberService
)
{
    private readonly ConsiderationsService _considerationService = considerationsService;
    private readonly FamilyService _familyService = familyService;
    private readonly EmailService _emailService = emailService;
    private readonly MemberService _memberService = memberService;

    /*
    The service is responsible for created and updating jobs that will
    gather gordons response and then send emails when the correct time comes
    */
    public void CreateEmailJob(string familyId)
    {
        var considerations = _considerationService.GetAllFamilyConsiderations(familyId).Data;
        var family = _familyService.GetById(familyId).Data;
        var members = _memberService.GetByFamilyId(familyId).Data;
        _emailService.SendEmail(
            family!.Email,
            "Chefster",
            $"Thank you for setting up your Chefster profile! {family.GenerationDay} {family.GenerationTime} {family.PhoneNumber}"
        );

        if (family != null)
        {
            RecurringJob.AddOrUpdate(
                family.Id,
                () => _emailService.SendEmail(family.Email, "Chefster", "Reoccuring email job "),
                Cron.Weekly(
                    family.GenerationDay,
                    family.GenerationTime.Hours,
                    family.GenerationTime.Minutes
                )
            );
        }
    }

    public void UpdateEmailJob(string familyId)
    {
        var family = _familyService.GetById(familyId).Data;
        if (family != null)
        {
            RecurringJob.AddOrUpdate(
                family.Id,
                () => _emailService.SendEmail(family.Email, "Chefster", "We sent an email!"),
                Cron.Weekly(
                    family.GenerationDay,
                    family.GenerationTime.Hours,
                    family.GenerationTime.Minutes
                )
            );
        }
    }
}
