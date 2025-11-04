using Core.Requests;
using Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAuthService
    {
        public Task<LoginResponse> LoginAsync(LoginRequest userInfo);
        public Task<RegisterResponse> RegisterAsync(RegisterRequest userInfo);
    }
}
