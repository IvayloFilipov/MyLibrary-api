namespace Common.Models.InputDTOs
{
    public class LoginUserWithRolesDto : LoginUserDto
    {
        public string? Id { get; set; }
        public ICollection<string> Roles { get; set; } = new HashSet<string>();
    }
}
