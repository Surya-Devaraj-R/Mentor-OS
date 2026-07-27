namespace MentorOS.Models.Enums;

// Absence of any ExerciseSubmission row means "not attempted" — this enum
// only covers states that actually get persisted.
public enum SelfAssessment
{
    Attempted,
    Solved,
    NeedsReview,
}
