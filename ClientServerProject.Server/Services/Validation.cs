using System.Text.RegularExpressions;

namespace ClientServerProject.Server.Repositories
{
    public class Validation
    {
        public bool ValidateEmail(string email)
        {
            bool isEmail = Regex.IsMatch(email, 
                @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", 
                RegexOptions.IgnoreCase);

            return isEmail;
        }

        public bool ValidateName(string name)
        {
            return !string.IsNullOrEmpty(name) && name.Length > 2 && name.All(x => char.IsLetter(x));
        }

        public bool ValidatePassword(string password)
        {
            bool isPassword = Regex.IsMatch(password,
                @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$",
                RegexOptions.IgnoreCase);

            return isPassword; 
        }
    }
}
