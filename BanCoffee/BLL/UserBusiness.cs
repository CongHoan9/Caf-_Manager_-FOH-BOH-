using DAL;
using Microsoft.IdentityModel.Tokens;
using Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace BLL
{
    public partial class UserBusiness(IUserRepository res, IConfiguration configuration) : Business<IUserRepository>(res), IUserBusiness
    {
        private readonly string Secret = configuration["AppSettings:Secret"];
        public bool Delete(string id)
        {
            return _res.Delete(id);
        }
        public UserModel Authenticate(string username, string password)
        {
            var user = _res.GetUser(username, password);
            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.Name, user.Hoten.ToString()),
                    new(ClaimTypes.StreetAddress, user.diachi)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.token = tokenHandler.WriteToken(token);

            return user;

        }

        public UserModel GetDatabyID(string id)
        {
            return _res.GetDatabyID(id);
        }
        public bool Create(UserModel model)
        {
            return _res.Create(model);
        }
        public bool Update(UserModel model)
        {
            return _res.Update(model);
        }
        public List<UserModel> Search(int pageIndex, int pageSize, out long total, string hoten, string taikhoan)
        {
            return _res.Search(pageIndex, pageSize, out total,hoten,taikhoan);
        }
    }

}
