namespace MentorOS.Models;

public class InterviewQuestionTag
{
    public int InterviewQuestionId { get; set; }
    public InterviewQuestion? InterviewQuestion { get; set; }
    public int TagId { get; set; }
    public Tag? Tag { get; set; }
}
