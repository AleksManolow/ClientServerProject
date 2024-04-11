using ClientServerProject.Server.Models;
using ClientServerProject.Server.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ClientServerProject.Server.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddUser(User user)
        {
            SqlConnection sqlConnection = new SqlConnection(_connectionString);

            sqlConnection.Open();

            var sqlQueryAddUser = "INSERT INTO Users (Email, FirstName, LastName, Password, IsVerified) VALUES (@Email, @FirstName, @LastName, @Password, @IsVerified)";

            var sqlCommandAddUser = new SqlCommand(sqlQueryAddUser, sqlConnection);

            sqlCommandAddUser.Parameters.AddWithValue("@Email", user.Email);
            sqlCommandAddUser.Parameters.AddWithValue("@FirstName", user.FirstName);
            sqlCommandAddUser.Parameters.AddWithValue("@LastName", user.LastName);
            sqlCommandAddUser.Parameters.AddWithValue("@Password", user.Password);
            sqlCommandAddUser.Parameters.AddWithValue("@IsVerified", user.IsVerified);

            sqlCommandAddUser.ExecuteNonQuery();
        }

        public User GetByEmail(string email)
        {
            SqlConnection sqlConnection = new SqlConnection(_connectionString);

            sqlConnection.Open();

            var slqQueryGetUserByEmail = "SELECT * FROM Users WHERE Email = @Email";

            var command = new SqlCommand(slqQueryGetUserByEmail, sqlConnection);
            command.Parameters.AddWithValue("@Email", email);

            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new User
                {
                    Id = (int)reader["Id"],
                    Email = (string)reader["Email"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    Password = (string)reader["Password"],
                    IsVerified = (bool)reader["IsVerified"]
                };
            }
            else
            {
                return null!;
            }
        }

        public User GetById(int id)
        {
            SqlConnection sqlConnection = new SqlConnection(_connectionString);

            sqlConnection.Open();

            var slqQueryGetUserById = "SELECT * FROM Users WHERE Id = @Id";

            var command = new SqlCommand(slqQueryGetUserById, sqlConnection);
            command.Parameters.AddWithValue("@Id", id);

            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new User
                {
                    Id = (int)reader["Id"],
                    Email = (string)reader["Email"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    Password = (string)reader["Password"],
                    IsVerified = (bool)reader["IsVerified"]
                };
            }
            else
            {
                return null!;
            }
        }

        public void InitializeDatabase()
        {
            SqlConnection sqlConnection = new SqlConnection(_connectionString);

            sqlConnection.Open();

            if (!UsersTableExist(sqlConnection))
            {
                var createUserTableQuery = @"
                    CREATE TABLE Users (
                        Id INT PRIMARY KEY IDENTITY,
                        Email NVARCHAR(100) NOT NULL,
                        FirstName NVARCHAR(100) NOT NULL,
                        LastName NVARCHAR(100) NOT NULL,
                        Password NVARCHAR(100) NOT NULL,
                        IsVerified BIT NOT NULL
                    )
                ";

                SqlCommand sqlCommandCreatUserTable = new SqlCommand(createUserTableQuery, sqlConnection);
                sqlCommandCreatUserTable.ExecuteNonQuery();
            }

            sqlConnection.Close();
        }
        private bool UsersTableExist(SqlConnection connection)
        {
            var query = @"
                SELECT COUNT(*)
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_NAME = 'Users'
                ";

            var command = new SqlCommand(query, connection);
            int count = (int)command.ExecuteScalar();
            return count > 0;
        }
        public string GenerateJwt(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ecawiasqrpqrgyhwnolrudpbsrwaynbqdayndnmcehjnwqyouikpodzaqxivwkconwqbhrmxfgccbxbyljguwlxhdlcvxlutbnwjlgpfhjgqbegtbxbvwnacyqnltrby"));
            var _TokenExpiryTimeInHour = Convert.ToInt64("3");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "http://localhost:8075",
                Audience = "http://localhost:8075",
                Expires = DateTime.UtcNow.AddHours(_TokenExpiryTimeInHour),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
