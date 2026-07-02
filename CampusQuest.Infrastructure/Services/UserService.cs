using CampusQuest.Domain.Entities;
using CampusQuest.Infrastructure.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace CampusQuest.Infrastructure.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        // Registers a new user by hashing their password
        public async Task<User?> RegisterAsync(string username, string email, string password)
        {
            if (await _userRepository.UserExistsAsync(username, email))
            {
                return null; // Username or Email is taken
            }

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            var newUser = new User
            {
                Username = username,
                Email = email,
                PasswordHash = Convert.ToBase64String(passwordHash),
                PasswordSalt = Convert.ToBase64String(passwordSalt),
                Xp = 0,
                Level = 1,
                CreatedAt = DateTime.UtcNow
            };

            return await _userRepository.AddUserAsync(newUser);
        }

        // Validates user credentials during login
        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return null; // User not found

            var hashBytes = Convert.FromBase64String(user.PasswordHash);
            var saltBytes = Convert.FromBase64String(user.PasswordSalt);

            if (!VerifyPasswordHash(password, hashBytes, saltBytes))
            {
                return null; // Wrong password
            }

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}