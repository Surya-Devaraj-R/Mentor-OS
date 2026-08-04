using System.Text.Json.Serialization;
using MentorOS.Data;
using MentorOS.Data.Seed;
using MentorOS.Endpoints;
using MentorOS.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

// Sandboxed cloud hosts (Render's free tier, among others) restrict the
// low-level OS file-watching feature (inotify) that .NET's config system
// uses by default to watch appsettings.json for live changes. In that
// restricted environment, the watcher itself crashes the whole process
// with a native segfault -- setting this BEFORE CreateBuilder runs turns
// that watcher off. We don't need live config reloading in production.
Environment.SetEnvironmentVariable("DOTNET_hostBuilder__reloadConfigOnChange", "false");

var builder = WebApplication.CreateBuilder(args);

// Cloud hosts (Render, Railway, etc.) tell the app which port to listen on
// via the PORT environment variable, instead of the fixed port used for
// local development.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

var connectionString = builder.Configuration.GetConnectionString("Default")!;

// On a freshly-attached persistent disk (e.g. Render), the mount directory
// exists but the database file doesn't yet -- SQLite can create the FILE
// itself, but not a missing parent directory. Create it up front so the
// very first deploy doesn't crash before the app ever gets a chance to run.
var dbDirectory = Path.GetDirectoryName(new SqliteConnectionStringBuilder(connectionString).DataSource);
if (!string.IsNullOrEmpty(dbDirectory))
{
    Directory.CreateDirectory(dbDirectory);
}

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<ProgressService>();
builder.Services.AddScoped<SearchService>();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await SeedRunner.ApplyAsync(db);
}

// Render (and similar hosts) terminate HTTPS at their edge and forward
// plain HTTP to this app, adding an X-Forwarded-Proto header to say so.
// Without trusting that header, UseHttpsRedirection below would see every
// request as "still HTTP" and redirect forever.
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();

// Defense-in-depth headers: no user input reaches dynamic HTML here, but
// every response should still tell the browser not to guess content types,
// not to be framed, and not to leak the full referrer URL.
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("Referrer-Policy", "no-referrer");
    await next();
});

app.MapTopicEndpoints();
app.MapModuleEndpoints();
app.MapLessonEndpoints();
app.MapResourceEndpoints();
app.MapProgressEndpoints();
app.MapPlannerEndpoints();
app.MapStreakEndpoints();
app.MapNoteEndpoints();
app.MapBookmarkEndpoints();
app.MapExerciseEndpoints();
app.MapInterviewPrepEndpoints();
app.MapSearchEndpoints();
app.MapChecklistEndpoints();
app.MapProjectEndpoints();

// Serve the frontend (index.html, js/, css/) straight from wwwroot/
app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();
