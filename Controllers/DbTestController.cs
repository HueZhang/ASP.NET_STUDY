using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Service;
using SqlSugar;

namespace WebApi.Controllers
{
    /// <summary>
    /// sqlsugar_Test
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DbTestController : ControllerBase
    {
        /// <summary>
        /// 注入
        /// </summary>
        public DbTestController(IStudentService studentService)
        {
            this.studenService = studentService;
        }

        private IStudentService studenService { get;  }
        /*
        /// <summary>
        /// 建表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public void CreateTable()
        {
             db.CodeFirst.InitTables(typeof(Student));
        }*/

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Student> GetList()
        {
            return studenService.GetList();
        }
        /// <summary>
        /// 
        /// </summary>
        [HttpPost]
        public List<Student> Test([FromBody]Student student)
        {
            return studenService.GetList();
        }
    }
}
