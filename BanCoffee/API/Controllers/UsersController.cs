using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Model;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUserBusiness userBusiness, IConfiguration configuration, IWebHostEnvironment env) : ControllerBase
    {
        private readonly IUserBusiness _userBusiness = userBusiness;
        private readonly string _path = configuration["AppSettings:PATH"];
        private readonly IWebHostEnvironment _env = env ?? throw new ArgumentNullException(nameof(env));
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthenticateModel model)
        {
            var user = _userBusiness.Authenticate(model.Username, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });
            return Ok(new { user.user_id, user.hoten, user.taikhoan, user.role, user.token });
        }
        [NonAction]
        public string SaveFileFromBase64String(string RelativePathFileName, string dataFromBase64String)
        {
            if (dataFromBase64String.Contains("base64,"))
            {
                dataFromBase64String = dataFromBase64String[(dataFromBase64String.IndexOf("base64,", 0) + 7)..];
            }
            return WriteFileToAuthAccessFolder(RelativePathFileName, dataFromBase64String);
        }
        [NonAction]
        public string WriteFileToAuthAccessFolder(string RelativePathFileName, string base64StringData)
        {
            try
            {
                string result = "";
                string serverRootPathFolder = _path;
                string fullPathFile = $@"{serverRootPathFolder}\{RelativePathFileName}";
                string fullPathFolder = Path.GetDirectoryName(fullPathFile);
                if (!Directory.Exists(fullPathFolder))
                    Directory.CreateDirectory(fullPathFolder);
                System.IO.File.WriteAllBytes(fullPathFile, Convert.FromBase64String(base64StringData));
                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [Authorize(Roles = "Admin")]
        [Route("delete-user")]
        [HttpPost]
        public IActionResult DeleteUser([FromBody] Dictionary<string, object> formData)
        {
            string user_id = GetString(formData, "user_id");
            _userBusiness.Delete(user_id);
            return Ok(new { message = "Th�nh c�ng" });
        }
        [Route("download/{filename}")]
        [HttpGet]
        public IActionResult DownloadSetupSataProduct(string filename)
        {
            try
            {
                var webRoot = _env.ContentRootPath;
                var filePath = Path.Combine(webRoot + @$"/upload/", filename);
                var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                return File(stream, "application/octet-stream");
            }
            catch (Exception)
            {
                throw new Exception("Có lỗi trong quá trình download.");
            }
        }
        [Route("upload")]
        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file.Length > 0)
                {
                    string filePath = $"upload/{file.FileName}";
                    var fullPath = CreatePathFile(filePath);
                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    return Ok(new { filePath });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Không tìm thây");
            }
        }
        [NonAction]
        private string CreatePathFile(string RelativePathFileName)
        {
            try
            {
                string serverRootPathFolder = _path;
                string fullPathFile = $@"{serverRootPathFolder}\{RelativePathFileName}";
                string fullPathFolder = Path.GetDirectoryName(fullPathFile);
                if (!Directory.Exists(fullPathFolder))
                    Directory.CreateDirectory(fullPathFolder);
                return fullPathFile;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        
        public class UserCreateRequest
        {
            public string HoTen { get; set; }
            public DateTime? NgaySinh { get; set; }
            public string TaiKhoan { get; set; }
            public string MatKhau { get; set; }
            public string Role { get; set; } = Model.Role.Staff;
            public string Email { get; set; }
            public string Sdt { get; set; }
            public string DiaChi { get; set; }
            public string GioiTinh { get; set; } = "Nam";
            public IFormFile Avatar { get; set; }
        }

        [Authorize(Roles = "Admin")]
        [Route("create-user")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromForm] UserCreateRequest request)
        {
            string filePath = null;
            if (request.Avatar != null && request.Avatar.Length > 0)
            {
                string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(request.Avatar.FileName)}";
                filePath = Path.Combine("upload", uniqueFileName);
                var fullPath = CreatePathFile(filePath);
                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await request.Avatar.CopyToAsync(fileStream);
                }
            }

            var model = new UserModel
            {
                user_id = Guid.NewGuid().ToString(),
                hoten = request.HoTen,
                ngaysinh = request.NgaySinh ?? (DateTime.TryParse("2000-01-01", out var d) ? d : DateTime.Now),
                taikhoan = request.TaiKhoan,
                matkhau = request.MatKhau,
                role = string.IsNullOrEmpty(request.Role) ? Model.Role.Staff : request.Role,
                email = request.Email,
                diachi = request.DiaChi ?? "",
                gioitinh = string.IsNullOrEmpty(request.GioiTinh) ? "Nam" : request.GioiTinh,
                image_url = filePath
            };

            _userBusiness.Create(model);
            model.matkhau = null;
            return Ok(model);
        }

        // Tương thích ngược với JSON payload từ frontend
        [Authorize(Roles = "Admin")]
        [Route("create-user1")]
        [HttpPost]
        public UserModel CreateUser1([FromBody] UserModel model)
        {
            model.user_id = Guid.NewGuid().ToString();
            if (model.ngaysinh == default || model.ngaysinh < new DateTime(1753, 1, 1))
            {
                model.ngaysinh = new DateTime(2000, 1, 1);
            }
            if (string.IsNullOrEmpty(model.diachi)) model.diachi = "";
            if (string.IsNullOrEmpty(model.gioitinh)) model.gioitinh = "Nam";
            if (string.IsNullOrEmpty(model.role)) model.role = Model.Role.Staff;
            _userBusiness.Create(model);
            model.matkhau = null;
            return model;
        }
        
        [Route("update-user")]
        [HttpPost]
        public IActionResult UpdateUser([FromBody] UserModel model)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var currentRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (currentRole != "Admin" && currentUserId != model.user_id)
            {
                return StatusCode(403, new { message = "Bạn không có quyền sửa tài khoản của người khác" });
            }

            var existing = _userBusiness.GetDatabyID(model.user_id);
            if (existing == null)
            {
                return NotFound(new { message = "Tài khoản không tồn tại" });
            }

            // Nếu không nhập mật khẩu mới, giữ nguyên mật khẩu hiện tại trong DB
            if (string.IsNullOrEmpty(model.matkhau))
            {
                model.matkhau = existing.matkhau;
            }

            // Tránh SqlDateTime overflow (1/1/0001) nếu frontend không gửi ngaysinh
            if (model.ngaysinh == default || model.ngaysinh < new DateTime(1753, 1, 1))
            {
                model.ngaysinh = existing.ngaysinh > new DateTime(1753, 1, 1) ? existing.ngaysinh : new DateTime(2000, 1, 1);
            }

            if (string.IsNullOrEmpty(model.diachi)) model.diachi = existing.diachi ?? "";
            if (string.IsNullOrEmpty(model.gioitinh)) model.gioitinh = existing.gioitinh ?? "Nam";
            if (string.IsNullOrEmpty(model.image_url)) model.image_url = existing.image_url;

            if (model.image_url != null && model.image_url.Contains(";"))
            {
                var arrData = model.image_url.Split(';');
                if (arrData.Length == 3)
                {
                    var savePath = $@"assets/images/{arrData[0]}";
                    model.image_url = $"{savePath}";
                    SaveFileFromBase64String(savePath, arrData[2]);
                }
            }
            _userBusiness.Update(model);
            model.matkhau = null;
            return Ok(model);
        }
        [Route("get-by-id/{id}")]
        [HttpGet]
        public UserModel GetDatabyID(string id)
        {
            var u = _userBusiness.GetDatabyID(id); if (u != null) u.matkhau = null; return u;
        }
        
        [Authorize(Roles = "Admin")]
        [Route("search")]
        [HttpPost]
                public ResponseModel Search([FromBody] Dictionary<string, object> formData)
        {
            var response = new ResponseModel();
            try
            {
                int page = GetInt(formData, "page", 1);
                int pageSize = GetInt(formData, "pageSize", 10);
                string hoten = GetString(formData, "hoten", "");
                string taikhoan = GetString(formData, "taikhoan", "");

                var data = _userBusiness.Search(page, pageSize, out long total, hoten, taikhoan);
                response.TotalItems = total;
                response.Data = data;
                response.Page = page;
                response.PageSize = pageSize;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return response;
        }
        
        [Authorize(Roles = "Admin")]
        [Route("get-all")]
        [HttpGet]
        public List<UserModel> GetAll()
        {
            return _userBusiness.GetAll();
        }
        
        [Route("change-password")]
        [HttpPost]
        public IActionResult ChangePassword([FromBody] ChangePasswordModel model)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var currentRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (currentRole != "Admin" && currentUserId != model.UserId)
            {
                return StatusCode(403, new { message = "Bạn không có quyền đổi mật khẩu của người khác" });
            }

            try
            {
                _userBusiness.ChangePassword(model.UserId, model.OldPassword, model.NewPassword);
                return Ok(new { message = "Đổi mật khẩu thành công" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Mật khẩu cũ không đúng hoặc có lỗi xảy ra" });
            }
        }
        [Route("check-exists/{taikhoan}")]
        [HttpGet]
        public IActionResult CheckExists(string taikhoan)
        {
            var exists = _userBusiness.CheckExists(taikhoan);
            return Ok(new { exists });
        }
    }
}









