namespace Authorization.DTO.Users;

public class UserPutReq
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public List<string> Roles { get; set; } = [];
}