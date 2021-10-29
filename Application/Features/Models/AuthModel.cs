using System.Collections.Generic;

namespace Application.Features.Models
{
    public class AuthModel
    {
        public List<string> Messages { get; set; }
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
    }
}