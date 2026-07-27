namespace MentorOS.Models;

// Migration target for the 8 links that used to live in wwwroot/app.js's
// RESOURCE_SECTIONS array. Now supplementary to the in-app curriculum.
public class Resource
{
    public int Id { get; set; }
    public string Slug { get; set; } = "";
    public int? TopicId { get; set; }
    public Topic? Topic { get; set; }

    // Preserves the original section heading verbatim (e.g. "Data Structures &
    // Algorithms") so nothing is lost even where TopicId is a best-fit guess.
    public string LegacySectionTitle { get; set; } = "";

    public string Title { get; set; } = "";
    public string Label { get; set; } = "";
    public string Url { get; set; } = "";
    public string IconKey { get; set; } = "";
    public int SortOrder { get; set; }
    public DateTime CreatedUtc { get; set; }
}
