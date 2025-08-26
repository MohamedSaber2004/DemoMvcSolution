using AutoMapper;
using Demo.BusinessLogic.DTOs.EmployeeDtos;
using Demo.DataAccess.Models.Employee_Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace Demo.BusinessLogic.Profiles
{
    public class MappingProfiles: Profile
    {
        public MappingProfiles() : base()
        {
            CreateMap<Employee, EmployeeDTO>()
                .ForMember(destination => destination.EmpGender, Options => Options.MapFrom(source => source.Gender))
                .ForMember(destination => destination.EmpType, Options => Options.MapFrom(source => source.EmployeeType))
                .ForMember(destination => destination.Department, Options => Options.MapFrom(source => source.Department != null ? source.Department.Name : null));


            CreateMap<Employee, EmployeeDetailsDto>()
                .ForMember(destination => destination.Gender, Options => Options.MapFrom(source => source.Gender))
                .ForMember(destination => destination.EmployeeType, Options => Options.MapFrom(source => source.EmployeeType))
                .ForMember(destination => destination.HiringDate, Options => Options.MapFrom(source => DateOnly.FromDateTime(source.HiringDate)))
                .ForMember(destination => destination.Department, Options => Options.MapFrom(source => source.Department != null ? source.Department.Name : null))
                .ForMember(destination => destination.ImageName, Options => Options.MapFrom(source => source.ImageName));


            //CreateMap<CreatedEmployeeDto, Employee>().ReverseMap();
            CreateMap<CreatedEmployeeDto, Employee>()
                .ForMember(destination => destination.HiringDate, Options => Options.MapFrom(source => source.HiringDate.ToDateTime(TimeOnly.MinValue)));

            CreateMap<UpdatedEmployeeDto, Employee>()
                .ForMember(destination => destination.HiringDate, Options => Options.MapFrom(source => source.HiringDate.ToDateTime(TimeOnly.MinValue)))
                .ForMember(destination => destination.ImageName, Options => Options.MapFrom(source => source.ImageName)).ReverseMap();
        }

    }
}
