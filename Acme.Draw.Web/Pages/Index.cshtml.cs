using Acme.Draw.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Acme.Draw.Web.Pages;

public class IndexModel : PageModel
{
    private readonly DrawEntryService _svc;

    public IndexModel(DrawEntryService svc) => _svc = svc;

    [BindProperty]
    public IndexInputModel Input { get; set; } = new();

    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return Page();

        // DateOfBirth is required, but keep it defensive
        if (Input.DateOfBirth is null)
        {
            ModelState.AddModelError("Input.DateOfBirth", "Date of birth is required.");
            return Page();
        }

        var req = new SubmissionRequest(
            FirstName: Input.FirstName ?? "",
            LastName: Input.LastName ?? "",
            Email: Input.Email ?? "",
            DateOfBirth: Input.DateOfBirth.Value,
            SerialNumber: (Input.SerialNumber ?? "").Trim()
        );

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var result = await _svc.SubmitAsync(req, today, ct);

        if (result.IsSuccess)
        {
            SuccessMessage = "Your entry has been submitted.";
            ModelState.Clear();
            Input = new();
            return Page();
        }

        // Simple mapping (can be refined later)
        ErrorMessage = result.ErrorMessage ?? "Submission failed.";
        return Page();
    }
}