using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UnknownShores.Entities;

namespace UnknownShores.Services
{
    public interface IUserService
    {
        TokenModel Authenticate(string username, string password);
        UserModel Profile(int id);
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private List<UserModel> _users = new List<UserModel>
        {
            new UserModel { Id = 1, Username = "RGBKnights", Email = "master@rgbknights.com", Password = "password1234" }
        };

        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public TokenModel Authenticate(string username, string password)
        {
            var user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);

            // return null if user not found
            if (user == null) return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var expires = DateTime.UtcNow.AddHours(1);
            var delta = (expires - DateTime.UtcNow);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                }),
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var reponse = new TokenModel();
            reponse.Token = tokenHandler.WriteToken(token);
            reponse.ExpiresIn = Math.Round(delta.TotalSeconds).ToString();
            reponse.TokenType = "Bearer";

            return reponse;
        }

        public UserModel Profile(int id)
        {
            var user = _users.SingleOrDefault(x => x.Id == id);
            return user;
        }
    }
}
