﻿using AutoMapper;
using PIMTool.Core.Domain.Entities;
using PIMTool.Dtos.Employee;
using PIMTool.Dtos.Group;
using PIMTool.Dtos.Project;

namespace PIMTool.MappingProfiles
{
    public class ProjectAutoMapperProfile : Profile
    {
        public ProjectAutoMapperProfile()
        {
            // project
            CreateMap<Project, ProjectDto>().ReverseMap();
            CreateMap<Project, ProjectCreateDto>().ReverseMap();
            CreateMap<Project, ProjectUpdateDto>().ReverseMap();

            // employee
            CreateMap<Employee, EmployeeDto>().ReverseMap();
            CreateMap<Employee, EmployeeCreateDto>().ReverseMap();
            CreateMap<Employee, EmloyeeUpdateDto>().ReverseMap();

            // group
            CreateMap<Group, GroupDto>().ReverseMap();

        }
    }
}