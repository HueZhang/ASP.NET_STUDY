using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class JwtController : Controller
    {
        /// <summary>
        /// 注入
        /// </summary>
        public JwtController(IConfiguration configuration) {this.cfg = configuration; }
        /// <summary>
        /// 
        /// </summary>
        public IConfiguration cfg { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost]
        public string CreateToken()
        {
            var tokenModel = cfg.GetSection("Jwt").Get<TokenModelJwt>();
            tokenModel.UserName = "Test";
            tokenModel.UserId = 123;
            tokenModel.Role = "Admin";
            return Common.JWTHelper.CreateJwt(tokenModel);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public IActionResult DeToken([FromHeader]string auth)
        {
            var token = JWTHelper.SerializeJwt(auth.Replace("Bearer ","")); 
            return Ok(token);
        }
    }
}
