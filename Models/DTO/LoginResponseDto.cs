namespace CodePulse.API.Models.DTO
{
    public class LoginResponseDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public List<String> Roles { get; set; }
    }
}
