namespace Web_Library.Services.Services.Password
{
        public interface IPasswordService
        {
            string HashPassword(string password);
            bool VerifyPassword(string password, string storedHash);
        }
}
