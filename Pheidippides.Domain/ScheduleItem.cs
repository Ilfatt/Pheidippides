namespace Pheidippides.Domain;

public class ScheduleItem
{
    public required DateTimeOffset From { get; init; }
    public required DateTimeOffset To { get; init; }
    public required long UserId { get; init; }
}