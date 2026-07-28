using MentorOS.Models.Enums;

namespace MentorOS.Models;

public class InterviewQuestion
{
    public int Id { get; set; }
    public QuestionType QuestionType { get; set; }
    public string Title { get; set; } = "";
    public string PromptText { get; set; } = "";
    public string? SuggestedApproach { get; set; }
    public string? SampleAnswer { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }

    public List<InterviewQuestionTag> QuestionTags { get; set; } = [];
    public List<InterviewQuestionCompany> QuestionCompanies { get; set; } = [];
}
