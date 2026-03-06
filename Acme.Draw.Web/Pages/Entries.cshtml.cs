using Acme.Draw.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Acme.Draw.Web.Pages;

public class EntriesModel : PageModel
{
    private readonly DrawDbContext _db;

    public EntriesModel(DrawDbContext db)
    {
        _db = db;
    }

    public List<EntryViewModel> Entries { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public int TotalPages { get; set; }

    private const int PageSize = 10;

    public async Task OnGetAsync()
    {
        if (PageNumber < 1)
            PageNumber = 1;
        if (PageNumber > TotalPages && TotalPages > 0)
            PageNumber = TotalPages;

        var totalCount = await _db.Submissions.CountAsync();
        TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

        Entries = await _db.Submissions
            .OrderByDescending(x => x.CreatedAtUtc)
            .ThenByDescending(x => x.Id)
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .Select(x => new EntryViewModel
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                SerialNumber = x.SerialNumber,
                CreatedAtUtc = x.CreatedAtUtc
            })
            .ToListAsync();
    }

    public class EntryViewModel
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public DateTimeOffset CreatedAtUtc { get; set; }
    }
}