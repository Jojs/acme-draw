using Acme.Draw.Core;
using Xunit;

namespace Acme.Draw.Tests;

public sealed class DrawEntryServiceTests
{
    private static SubmissionRequest ValidRequest(string serial = "ABC123")
        => new(
            FirstName: "Jan",
            LastName: "Johansen",
            Email: "jan@example.com",
            DateOfBirth: new DateOnly(1990, 1, 1),
            SerialNumber: serial
        );

    [Theory]
    [InlineData("", "Johansen")]
    [InlineData("Jan", "")]
    [InlineData("   ", "Johansen")]
    [InlineData("Jan", "   ")]
    public async Task Rejects_missing_or_whitespace_names(string first, string last)
    {
        var serials = new InMemorySerialNumberRepository(new[] { "ABC123" });
        var subs = new InMemorySubmissionRepository();
        var svc = new DrawEntryService(serials, subs);

        var req = ValidRequest("ABC123") with { FirstName = first, LastName = last };
        var res = await svc.SubmitAsync(req, today: new DateOnly(2026, 3, 2));

        Assert.False(res.IsSuccess);
        Assert.Equal(SubmissionErrorCode.InvalidName, res.ErrorCode);
    }

    [Fact]
    public async Task Rejects_invalid_email()
    {
        var serials = new InMemorySerialNumberRepository(new[] { "ABC123" });
        var subs = new InMemorySubmissionRepository();
        var svc = new DrawEntryService(serials, subs);

        var req = ValidRequest("ABC123") with { Email = "not-an-email" };
        var res = await svc.SubmitAsync(req, today: new DateOnly(2026, 3, 2));

        Assert.False(res.IsSuccess);
        Assert.Equal(SubmissionErrorCode.InvalidEmail, res.ErrorCode);
    }

    [Fact]
    public async Task Rejects_under_18()
    {
        var serials = new InMemorySerialNumberRepository(new[] { "ABC123" });
        var subs = new InMemorySubmissionRepository();
        var svc = new DrawEntryService(serials, subs);

        var req = ValidRequest("ABC123") with { DateOfBirth = new DateOnly(2010, 3, 3) }; // 15-ish
        var res = await svc.SubmitAsync(req, today: new DateOnly(2026, 3, 2));

        Assert.False(res.IsSuccess);
        Assert.Equal(SubmissionErrorCode.UnderAge, res.ErrorCode);
    }

    [Fact]
    public async Task Accepts_exactly_18()
    {
        var serials = new InMemorySerialNumberRepository(new[] { "ABC123" });
        var subs = new InMemorySubmissionRepository();
        var svc = new DrawEntryService(serials, subs);

        var req = ValidRequest("ABC123") with { DateOfBirth = new DateOnly(2010, 3, 3) };
        var res = await svc.SubmitAsync(req, today: new DateOnly(2028, 3, 3));

        Assert.True(res.IsSuccess);
        Assert.Equal(SubmissionErrorCode.None, res.ErrorCode);
    }

    [Fact]
    public async Task Rejects_invalid_serial()
    {
        var serials = new InMemorySerialNumberRepository(new[] { "VALID1" });
        var subs = new InMemorySubmissionRepository();
        var svc = new DrawEntryService(serials, subs);

        var res = await svc.SubmitAsync(ValidRequest("NOPE"), today: new DateOnly(2026, 3, 2));

        Assert.False(res.IsSuccess);
        Assert.Equal(SubmissionErrorCode.InvalidSerialNumber, res.ErrorCode);
    }

    [Fact]
    public async Task Persists_submission_on_success()
    {
        var serials = new InMemorySerialNumberRepository(new[] { "ABC123" });
        var subs = new InMemorySubmissionRepository();
        var svc = new DrawEntryService(serials, subs);

        var today = new DateOnly(2026, 3, 2);

        var res = await svc.SubmitAsync(ValidRequest("ABC123"), today);
 
        Assert.True(res.IsSuccess);
        Assert.Single(subs.Items);
    }

    [Fact]
    public async Task Accepts_first_and_second_entry_per_serial()
    {
        var serials = new InMemorySerialNumberRepository(new[] { "ABC123" });
        var subs = new InMemorySubmissionRepository();
        var svc = new DrawEntryService(serials, subs);

        var today = new DateOnly(2026, 3, 2);

        var r1 = await svc.SubmitAsync(ValidRequest("ABC123"), today);
        var r2 = await svc.SubmitAsync(ValidRequest("ABC123"), today);

        Assert.True(r1.IsSuccess);
        Assert.True(r2.IsSuccess);
    }

    [Fact]
    public async Task Rejects_third_entry_per_serial()
    {
        var serials = new InMemorySerialNumberRepository(new[] { "ABC123" });
        var subs = new InMemorySubmissionRepository();
        var svc = new DrawEntryService(serials, subs);

        var today = new DateOnly(2026, 3, 2);

        await svc.SubmitAsync(ValidRequest("ABC123"), today);
        await svc.SubmitAsync(ValidRequest("ABC123"), today);
        var r3 = await svc.SubmitAsync(ValidRequest("ABC123"), today);

        Assert.False(r3.IsSuccess);
        Assert.Equal(SubmissionErrorCode.SerialNumberAlreadyUsedTwice, r3.ErrorCode);
    }
}