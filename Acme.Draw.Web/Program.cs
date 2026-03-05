using Acme.Draw.Core;
using Acme.Draw.Web.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<DrawDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ISerialNumberRepository, EfSerialNumberRepository>();
builder.Services.AddScoped<ISubmissionRepository, EfSubmissionRepository>();
builder.Services.AddScoped<DrawEntryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

// Apply migrations and seed initial serial numbers
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<DrawDbContext>();
await DbSeeder.SeedAsync(db);

app.Run();
