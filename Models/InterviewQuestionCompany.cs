namespace MentorOS.Models;

public class InterviewQuestionCompany
{
    public int InterviewQuestionId { get; set; }
    public InterviewQuestion? InterviewQuestion { get; set; }
    public int CompanyId { get; set; }
    public Company? Company { get; set; }
}
