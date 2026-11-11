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
            return Ok(new { user.user_id, user.Hoten, user.taikhoan, user.token });
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

        [Route("delete-user")]
        [HttpPost]
        public IActionResult DeleteUser([FromBody] Dictionary<string, object> formData)
        {
            string user_id = "";
            if (formData.TryGetValue("user_id", out object value) && !string.IsNullOrEmpty(Convert.ToString(value))) { user_id = Convert.ToString(value); }
            _userBusiness.Delete(user_id);
            return Ok();
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

        [Route("create-user1")]
        [HttpPost]
        public UserModel CreateUser1([FromBody] UserModel model)
        {
            model.user_id = Guid.NewGuid().ToString();
            _userBusiness.Create(model);
            return model;
        }


        [Route("create-user2")]
        [HttpPost]
        public async Task<UserModel> CreateUser2([FromForm] IFormFile file, [FromForm] string hoten, [FromForm] DateTime? ngaysinh, [FromForm] string taikhoan, [FromForm] string matkhau)
        {
            if (file.Length > 0)
            {
                string filePath = $"upload/{file.FileName}";
                var fullPath = CreatePathFile(filePath);
                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                var model = new UserModel
                {
                    Hoten = hoten,
                    ngaysinh = ngaysinh,
                    taikhoan = taikhoan,
                    matkhau = matkhau,
                    user_id = Guid.NewGuid().ToString(),
                    image_url = filePath
                };
                _userBusiness.Create(model);
                return model;
            }
            else
            {
                var model = new UserModel
                {
                    Hoten = hoten,
                    ngaysinh = ngaysinh,
                    taikhoan = taikhoan,
                    matkhau = matkhau,
                    user_id = Guid.NewGuid().ToString(),
                    image_url = null
                };
                model.user_id = Guid.NewGuid().ToString();
                model.image_url = null;
                _userBusiness.Create(model);
                return model;
            }
        }

        [Route("update-user")]
        [HttpPost]
        public UserModel UpdateUser([FromBody] UserModel model)
        {
            if (model.image_url != null)
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
            return model;
        }

        [Route("get-by-id/{id}")]
        [HttpGet]
        public UserModel GetDatabyID(string id)
        {
            return _userBusiness.GetDatabyID(id);
        }

        [Route("search")]
        [HttpPost]
        public ResponseModel Search([FromBody] Dictionary<string, object> formData)
        {
            var response = new ResponseModel();
            try
            {
                if (int.TryParse(formData["page"].ToString(), out int page) && 
                    int.TryParse(formData["pageSize"].ToString(), out int pageSize))
                {
                    string hoten = "";
                    if (formData.TryGetValue("hoten", out object value1) && 
                        !string.IsNullOrEmpty(Convert.ToString(value1))) 
                    { 
                        hoten = Convert.ToString(value1); 
                    }
                    string taikhoan = "";
                    if (formData.TryGetValue("taikhoan", out object value) && 
                        !string.IsNullOrEmpty(Convert.ToString(value))) 
                    { 
                        taikhoan = Convert.ToString(value); 
                    }
                    var data = _userBusiness.Search(page, pageSize, out long total, hoten, taikhoan);
                    response.TotalItems = total;
                    response.Data = data;
                    response.Page = page;
                    response.PageSize = pageSize;
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return response;
        }
    }
}
