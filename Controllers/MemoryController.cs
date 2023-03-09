using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
     [Route("api/[controller]")]
    [ApiController]
    public class MemoryController : Controller
    {
        private IMemoryCache _cache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memoryCache"></param>
        public MemoryController(IMemoryCache memoryCache) 
        {
            _cache = memoryCache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string Set(string name)
        {
            var key = Guid.NewGuid().ToString();
            _cache.Set(key, name,TimeSpan.FromSeconds(20));
            return key;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public string Get(string key)
        {
            return _cache.Get(key).ToString();
        }
    }
}
