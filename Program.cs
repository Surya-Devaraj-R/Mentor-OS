using System.Text.Json.Serialization;
using MentorOS.Data;
using MentorOS.Data.Seed;
using MentorOS.Endpoints;
using MentorOS.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));
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
