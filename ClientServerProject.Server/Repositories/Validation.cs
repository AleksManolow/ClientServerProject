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
            
            return !string.IsNullOrEmpty(name);
        }

        public bool ValidatePassword(string password)
        {
            
            return password.Length >= 6; 
        }
    }
}
