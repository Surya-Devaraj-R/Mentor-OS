namespace MentorOS.Models;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public string? OverviewBody { get; set; }

    public List<InterviewQuestionCompany> QuestionCompanies { get; set; } = [];
}
