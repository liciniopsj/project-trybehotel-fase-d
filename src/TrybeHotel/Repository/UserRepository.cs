#nullable disable
using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class UserRepository : IUserRepository
    {
        protected readonly ITrybeHotelContext _context;
        public UserRepository(ITrybeHotelContext context)
        {
            _context = context;
        }
        public UserDto GetUserById(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            if (user != null)
            {
                return new UserDto
                {
                    Email = user.Email,
                    Name = user.Name,
                    UserId = user.UserId,
                    UserType = user.UserType
                };
            }

            throw new Exception("User not found");
        }

        public UserDto Login(LoginDto login)
        {
            var userLogin = _context.Users
                .FirstOrDefault(user => user.Email == login.Email && user.Password == login.Password) ?? throw new Exception("Incorrect e-mail or password");
            return new UserDto
            {
                Email = userLogin.Email,
                Name = userLogin.Name,
                UserId = userLogin.UserId,
                UserType = userLogin.UserType
            };
        }
        public UserDto Add(UserDtoInsert user)
        {
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                throw new Exception("User email already exists");
            }

            var newUser = new User
            {
                Email = user.Email,
                Password = user.Password,
                Name = user.Name,
                UserType = "client"
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return new UserDto
            {
                Email = newUser.Email,
                Name = newUser.Name,
                UserId = newUser.UserId,
                UserType = newUser.UserType
            };
        }

        public UserDto GetUserByEmail(string userEmail)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

            if (user != null)
            {
                return new UserDto
                {
                    Email = user.Email,
                    Name = user.Name,
                    UserId = user.UserId,
                    UserType = user.UserType
                };
            }

            throw new Exception("User not found");
        }

        public IEnumerable<UserDto> GetUsers()
        {
            return _context.Users.Select(
                u => new UserDto
                {
                    Email = u.Email,
                    Name = u.Name,
                    UserId = u.UserId,
                    UserType = u.UserType
                }
            );
        }

    }
}