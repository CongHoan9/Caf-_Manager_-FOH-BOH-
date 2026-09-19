using DAL;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
            if (user == null)
                return null;

            // Dang nhap dung -> sinh JWT token (het han 7 ngay)
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.Name, user.Hoten ?? string.Empty),
                    new(ClaimTypes.StreetAddress, user.diachi ?? string.Empty),
                    new(ClaimTypes.Role, user.role ?? Role.Staff)
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
            model.user_id = model.user_id ?? Guid.NewGuid().ToString();
            return _res.Create(model);
        }

        public bool Update(UserModel model)
        {
            return _res.Update(model);
        }

        public List<UserModel> Search(int pageIndex, int pageSize, out long total, string hoten, string taikhoan)
        {
            return _res.Search(pageIndex, pageSize, out total, hoten, taikhoan);
        }
    }
}
