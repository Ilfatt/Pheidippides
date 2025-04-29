namespace Pheidippides.Api.Contracts.Schedule;

public class ScheduleDto
{
    public required List<ScheduleItemDto> ScheduleItems { get; init; }
}

public class ScheduleItemDto
{
    public required DateTimeOffset From { get; init; }
    public required DateTimeOffset To { get; init; }
    public required long UserId { get; init; }
}