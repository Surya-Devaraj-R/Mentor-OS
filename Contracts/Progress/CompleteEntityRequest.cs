using MentorOS.Models.Enums;

namespace MentorOS.Contracts.Progress;

public record CompleteEntityRequest(EntityKind EntityKind, int EntityId, bool Completed);
