namespace Pheidippides.Domain;

public class User
{
    public long Id { get; private init; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string PasswordHash { get; set; }
    public required string FirstName { get; set; }
    public required string SecondName { get; set; }
    public required string Surname { get; set; }
    public bool IsDuty { get; set; } = false;
    public virtual Team? Team { get; set; }
    public long TeamId { get; set; }
}