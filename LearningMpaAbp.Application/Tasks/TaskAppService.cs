using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Timing;
using AutoMapper;
using LearningMpaAbp.Tasks.Dtos;

namespace LearningMpaAbp.Tasks
{
    public class TaskAppService : LearningMpaAbpAppServiceBase, ITaskAppService
    {
        private readonly IRepository<Task> _taskRepository;

        protected TaskAppService(IRepository<Task> taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public GetTasksOutput GetTasks(GetTasksInput input)
        {
            var query = _taskRepository.GetAll();
            if (input.AssignedPersonId.HasValue)
            {
                query = query.Where(t => t.AssignedPersonId == input.AssignedPersonId.Value);
            }
            if (input.State.HasValue)
            {
                query = query.Where(t => t.State == input.State.Value);
            }
            return new GetTasksOutput { Tasks = Mapper.Map<List<TaskDto>>(query.ToList()) };
        }

        public void UpdateTask(UpdateTaskInput input)
        {
            //We can use Logger, it's defined in ApplicationService base class.
            Logger.Info("Updating a task for input: " + input);

            //Retrieving a task entity with given id using standard Get method of repositories.
            var task = _taskRepository.Get(input.Id);

            //Updating changed properties of the retrieved task entity.

            if (input.State.HasValue)
            {
                task.State = input.State.Value;
            }

            //We even do not call Update method of the repository.
            //Because an application service method is a 'unit of work' scope as default.
            //ABP automatically saves all changes when a 'unit of work' scope ends (without any exception).
        }

        public int CreateTask(CreateTaskInput input)
        {
            //We can use Logger, it's defined in ApplicationService class.
            Logger.Info("Creating a task for input: " + input);

            //Creating a new Task entity with given input's properties
            var task = new Task
            {
                Description = input.Description,
                Title = input.Title,
                State = input.State,
                CreationTime = Clock.Now
            };

            //Saving entity with standard Insert method of repositories.
            return _taskRepository.InsertAndGetId(task);
        }

        public async Task<TaskDto> GetTaskByIdAsync(int taskId)
        {
            var task = await _taskRepository.GetAsync(taskId);
            return task.MapTo<TaskDto>();
        }

        public TaskDto GetTaskById(int taskId)
        {
            var task = _taskRepository.Get(taskId);

            return task.MapTo<TaskDto>();
        }

        public void DeleteTask(int taskId)
        {
            var task = _taskRepository.Get(taskId);
            if (task != null)
            {
                _taskRepository.Delete(task);
            }
        }

        public IList<TaskDto> GetAllTasks()
        {
            var query = _taskRepository.GetAll();
            return Mapper.Map<IList<TaskDto>>(query.ToList());
        }

        public PagedResultDto<TaskDto> GetPagedTasks(GetTasksInput input)
        {
            //初步过滤
            var query = _taskRepository.GetAll().Include(t => t.AssignedPerson)
                .WhereIf(input.State.HasValue, t => t.State == input.State.Value)
                .WhereIf(!input.Filter.IsNullOrEmpty(), t => t.Title.Contains(input.Filter))
                .WhereIf(input.AssignedPersonId.HasValue, t => t.AssignedPersonId == input.AssignedPersonId.Value);

            //排序
            query = !string.IsNullOrEmpty(input.Sorting) ? query.OrderBy(input.Sorting) : query.OrderByDescending(t => t.CreationTime);

            //获取总数
            var tasksCount = query.Count();
            //默认的分页方式
            //var taskList = query.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

            //ABP提供了扩展方法PageBy分页方式
            var taskList = query.PageBy(input).ToList();

            return new PagedResultDto<TaskDto>(tasksCount, taskList.MapTo<List<TaskDto>>());
        }
    }
}
