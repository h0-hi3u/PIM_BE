using AutoMapper.Configuration.Conventions;
using Microsoft.EntityFrameworkCore;
using PIMTool.Core.Constants;
using PIMTool.Core.Domain.Entities;
using PIMTool.Core.Interfaces.Repositories;
using PIMTool.Core.Interfaces.Services;
using PIMTool.Extensions;
using System;
using System.Text;

namespace PIMTool.Services;

public class ProjectService : IProjectService
{
    private readonly IRepository<Project> _repository;
    private readonly string DES = "DES";
    private readonly string ASC = "ASC";
    public ProjectService(IRepository<Project> repository)
    {
        _repository = repository;
    }

    public async Task Create(Project project)
    {
        IEnumerable<Project> projectEnumerator = await _repository.GetAll();
        bool check = project.UniqueProjectNumber(projectEnumerator);
        if (check)
        {
            // project.Status = ProjectStatusConstants.NEW;
            await _repository.AddAsync(project);

            await _repository.SaveChangesAsync();
        }
        else
        {
            throw new Exception("Duplicate project number!");
        }
    }

    public async Task Delete(int id)
    {
        var existing = await _repository.GetAsync(id);
        if (existing != null)
        {
            _repository.Delete(existing);
            await _repository.SaveChangesAsync();
        }
        else
        {
            throw new Exception($"Not found projectId {id}");
        }
    }

    public async Task<IEnumerable<Project>> GetAll()
    {
        return await _repository.GetAll();
    }

    public async Task<Project?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetAsync(id, cancellationToken);
        return entity;
    }

    public async Task Update(Project project)
    {
        //var existing = await _repository.GetUpdate(project.Id);
        var existing = _repository.Get().Include(x => x.Employees).Where(x => x.Id == project.Id).FirstOrDefault();
        if (existing != null)
        {
            //_repository.ClearChangeTracker();
            
            existing.GroupId = project.GroupId;
            existing.ProjectNumber = project.ProjectNumber;
            existing.Name = project.Name;
            existing.Customer = project.Customer;
            //existing.Employees.Clear();
            existing.Employees = project.Employees;
            existing.Status = project.Status;
            existing.StartDate = project.StartDate;
            existing.EndDate = project.EndDate;
            //_repository.Update(existing);
            await _repository.SaveChangesAsync();
        }
        else
        {
            throw new Exception($"Not found projectId {project.Id}");
        }
    }

    public async Task<IEnumerable<Project>> SearchProject(string? searchText, string searchStatus, string sortNumber, string sortName, string sortStatus, string sortCustomer, string sortStartDate)
    {
        List<Project> listProject = (List<Project>)await _repository.GetAll();
        IEnumerable<Project> result;
        // Search
        if (string.IsNullOrEmpty(searchText) && searchStatus.Equals("0"))
        {
            result = listProject;
        }
        else if (string.IsNullOrEmpty(searchText))
        {
            result = listProject.Where(p => p.Status == searchStatus);
        }
        else if (searchStatus.Equals("0"))
        {
            result = listProject.Where(p => p.Name.Contains(searchText) || p.Customer.Contains(searchText) || p.ProjectNumber.ToString().Contains(searchText));
        }
        else
        {
            result = listProject.Where(p => (p.Name.Contains(searchText) || p.Customer.Contains(searchText) || p.ProjectNumber.ToString().Contains(searchText)) && p.Status == searchStatus);
        }

        // Sort
        if (sortNumber != "0")
        {
            if (sortNumber == ASC)
            {
                return result.OrderBy(p => p.ProjectNumber);
            }
            else
            {
                return result.OrderBy(p => p.ProjectNumber).Reverse();
            }

        }
        else if (sortName != "0")
        {
            if (sortName == ASC)
            {
                return result.OrderBy(p => p.Name);
            }
            else
            {
                return result.OrderBy(p => p.Name).Reverse();
            }
        }
        else if (sortStatus != "0")
        {
            if (sortStatus == ASC)
            {
                return result.OrderBy(p => p.Status);
            }
            else
            {
                return result.OrderBy(p => p.Status).Reverse();
            }
        }
        else if (sortCustomer != "0")
        {
            if (sortCustomer == ASC)
            {
                return result.OrderBy(p => p.Customer);
            }
            else
            {
                return result.OrderBy(p => p.Customer).Reverse();
            }
        }
        else if (sortStartDate != "0")
        {
            if (sortStartDate == ASC)
            {
                return result.OrderBy(p => p.StartDate);
            }
            else
            {
                return result.OrderBy(p => p.StartDate).Reverse();
            }
        }
        else
        {
            return result;
        }
    }

    public IEnumerable<Project> PagingProject(int pageSize, int pageIndex, IEnumerable<Project> list)
    {
        int skip = (pageIndex - 1) * pageSize;
        return list.Skip(skip).Take(pageSize);
    }

    public async Task<bool> CheckExist(int projectNumber)
    {
        bool checkExist = false;
        IEnumerable<Project> projects = await _repository.GetAll();
        foreach (var project in projects)
        {
            if (project.ProjectNumber == projectNumber)
            {
                checkExist = true;
                break;
            }
        }
        return checkExist;
    }

    public async Task RemoveRangeById(List<int> listRemoveId)
    {
        foreach (var projectId in listRemoveId)
        {
            var project = await _repository.GetAsync(projectId);
            if (project != null)
            {
                _repository.Delete(project);
            }
        }
        await _repository.SaveChangesAsync();
    }

    public Project? GetProjectInclude(int projectId)
    {
        var project = _repository.Get().Include(x => x.Employees).Where(x=> x.Id == projectId).AsNoTracking().FirstOrDefault();
        return project;
    }
}