using AutoMapper;
using Models;
using WebApi.Dto;

namespace WebApi.Profiles
{
    /// <summary>
    /// 
    /// </summary>
    public class MapperProfiles : Profile
    {

        public MapperProfiles()
        {
            CreateMap<Student,StudentDto>();
        }
    }
}
