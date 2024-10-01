using Microsoft.CodeAnalysis.Scripting;
using POECLDPart1.Models;
using POECLDPart1.Repositories;
using System.Text;

namespace POECLDPart1.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> RegisterAsync(string email, string password, string fullName)
        {
            // check if user already exists
            var existingUser = await _userRepository.GetUserByEmailAsync(email);
            if (existingUser != null)
                return false; // user already exists

            // hash password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            // craete user
            var user = new User
            {
                RowKey = email,
                Email = email,
                FullName = fullName,
                PasswordHash = passwordHash
            };

            return await _userRepository.CreateUserAsync(user);
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                return null;

            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return isValid ? user : null;
        }
    }
}
