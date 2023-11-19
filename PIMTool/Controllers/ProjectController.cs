﻿using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PIMTool.Core.Domain.Entities;
using PIMTool.Core.Interfaces.Services;
using PIMTool.Dtos;
using PIMTool.Dtos.Employee;
using PIMTool.Dtos.Project;

namespace PIMTool.Controllers;

[ApiController]
[Route("projects")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly IProjectEmployeesService _projectEmployeesService;
    private readonly IMapper _mapper;
    private readonly ResponseDto _responseDto;

    public ProjectController(IProjectService projectService, IMapper mapper, IProjectEmployeesService projectEmployeesService)
    {
        _projectService = projectService;
        _mapper = mapper;
        _responseDto = new ResponseDto();
        _projectEmployeesService = projectEmployeesService;
    }

    [HttpGet("{id}")]
    public ResponseDto Get([FromRoute][Required] int id)
    {
        try
        {
            var entity = _projectService.GetProjectInclude(id);
            _responseDto.Data = _mapper.Map<ProjectUpdateDto>(entity);
        }
        catch (Exception ex)
        {
            _responseDto.isSuccess = false;
            _responseDto.Error = ex.Message;
        }
        return _responseDto;
    }

    [HttpGet]
    public async Task<ResponseDto> GetAll()
    {
        try
        {
            IEnumerable<Project> list = await _projectService.GetAll();
            _responseDto.Data = _mapper.Map<IEnumerable<ProjectDto>>(list);
        }
        catch (Exception ex)
        {
            _responseDto.isSuccess = false;
            _responseDto.Error = ex.Message;
        }
        return _responseDto;
    }
    [HttpPost]
    public async Task<ResponseDto> Create(ProjectCreateDto projectCreateDto)
    {
        try
        {
            var project = _mapper.Map<Project>(projectCreateDto);
            await _projectService.Create(project);
            _responseDto.Data = projectCreateDto;
        }
        catch (Exception ex)
        {
            _responseDto.isSuccess = false;
            _responseDto.Error = ex.Message;
        }
        return _responseDto;
    }
    [HttpPut]
    public async Task<ResponseDto> Update(ProjectUpdateDto projectUpdateDto)
    {
        try
        {
            var project = _mapper.Map<Project>(projectUpdateDto);
            await _projectService.Update(project);
            _responseDto.Data = projectUpdateDto;
        }
        catch (Exception ex)
        {
            _responseDto.isSuccess = false;
            _responseDto.Error = ex.Message;
        }
        return _responseDto;
    }
    [HttpDelete("{id}")]
    public async Task<ResponseDto> Delete([FromRoute][Required] int id)
    {
        try
        {
            await _projectService.Delete(id);
            _responseDto.Data = NoContent();
        }
        catch (Exception ex)
        {
            _responseDto.isSuccess = false;
            _responseDto.Error = ex.Message;
        }
        return _responseDto;
    }
    
    [HttpGet("checkProjectNumber")]
    public async Task<bool> CheckProjectNumber(int projectNumber)
    {
        bool result = await _projectService.CheckExist(projectNumber);
        return result;
    }

    //[HttpGet("search")]
    //public async Task<ResponseDto> Search(string? searchText, string searchStatus)
    //{
    //    try
    //    {
    //        var list = await _projectService.SearchProject(searchText, searchStatus);
    //        _responseDto.Data = _mapper.Map<IEnumerable<ProjectDto>>(list);
    //    }
    //    catch (Exception ex)
    //    {
    //        _responseDto.isSuccess = false;
    //        _responseDto.Error = ex.Message;
    //    }
    //    return _responseDto;
    //}
    [HttpGet("paging")]
    public async Task<ResponseDto> Paging(int pageSize, int pageIndex, string? searchText, string searchStatus, string sortNumber, string sortName, string sortStatus, string sortCustomer, string sortStartDate)
    {
        try
        {
            var list = await _projectService.SearchProject(searchText, searchStatus, sortNumber, sortName, sortStatus, sortCustomer, sortStartDate);
            decimal count = (decimal)list.Count() / pageSize;
            decimal totalPage = Math.Ceiling(count);
            var result = _projectService.PagingProject(pageSize, pageIndex, list);
            _responseDto.Data = new
            {
                totalPage = totalPage,
                result = result
            };
        }
        catch (Exception ex)
        {
            _responseDto.isSuccess = false;
            _responseDto.Error = ex.Message;
        }
        return _responseDto;
    }
    [HttpPost("removeRange")]
    public async Task<ResponseDto> RemoveRange(List<int> projects)
    {
        try
        {
            await _projectService.RemoveRangeById(projects);
            _responseDto.Data = "Remove success!";
        } catch (Exception ex)
        {
            _responseDto.isSuccess = false;
            _responseDto.Error = ex.Message;
        }
        return _responseDto;
    }
}