using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using WebApi.Dto;

namespace WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MapperController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        public MapperController(IMapper mapper)
        {
            Mapper = mapper;
        }

        public IMapper Mapper { get; }
        [HttpPost]
        public StudentDto Test(Student student)
        {
            return Mapper.Map<StudentDto>(student);
        }
    }
}
