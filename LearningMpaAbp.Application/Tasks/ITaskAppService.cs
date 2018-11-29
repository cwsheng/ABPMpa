using Abp.Application.Services;
using Abp.Application.Services.Dto;
using LearningMpaAbp.Tasks.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningMpaAbp.Tasks
{

    /*
        ABP建议命名输入/输出参数为：MethodNameInput和MethodNameOutput
        并为每个应用服务方法定义单独的输入和输出DTO（如果为每个方法的输入输出都定义一个dto，那将有一个庞大的dto类需要定义维护。一般通过定义一个公用的dto进行共用）
        即使你的方法只接受/返回一个参数，也最好是创建一个DTO类
        一般会在对应实体的应用服务文件夹下新建Dtos文件夹来管理Dto类。
     */

    public interface ITaskAppService : IApplicationService
    {
        GetTasksOutput GetTasks(GetTasksInput input);

        void UpdateTask(UpdateTaskInput input);

        int CreateTask(CreateTaskInput input);

        Task<TaskDto> GetTaskByIdAsync(int taskId);

        TaskDto GetTaskById(int taskId);

        void DeleteTask(int taskId);

        IList<TaskDto> GetAllTasks();
        PagedResultDto<TaskDto> GetPagedTasks(GetTasksInput filter);
    }
}
