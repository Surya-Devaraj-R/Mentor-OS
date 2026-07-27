namespace MentorOS.Models.Enums;

// Drives how the frontend renders LessonContentBlock.Body. CodeSnippet
// blocks are dispatched on BlockType instead (raw <pre><code>, no parsing),
// so BodyFormat is set to PlainText there and simply unused by the renderer.
public enum BodyFormat
{
    PlainText,
    MiniMarkdown,
    StructuredSteps,
    AsciiArt,
}
