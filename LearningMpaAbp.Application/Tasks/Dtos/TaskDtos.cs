using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Runtime.Validation;
using LearningMpaAbp.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace LearningMpaAbp.Tasks.Dtos
{
    /// <summary>
    /// A DTO class that can be used in various application service methods when needed to send/receive Task objects.
    /// </summary>
    public class TaskDto : EntityDto
    {
        public long? AssignedPersonId { get; set; }

        public string AssignedPersonName { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreationTime { get; set; }

        public TaskState State { get; set; }

        //This method is just used by the Console Application to list tasks
        public override string ToString()
        {
            return string.Format(
                "[Task Id={0}, Description={1}, CreationTime={2}, AssignedPersonName={3}, State={4}]",
                Id,
                Description,
                CreationTime,
                AssignedPersonId,
                (TaskState)State
                );
        }
    }

    public class GetTasksOutput
    {
        public List<TaskDto> Tasks { get; set; }
    }

    [AutoMapTo(typeof(Task))]
    public class CreateTaskInput
    {
        public int? AssignedPersonId { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Title { get; set; }

        public TaskState State { get; set; }
        public override string ToString()
        {
            return string.Format("[CreateTaskInput > AssignedPersonId = {0}, Description = {1}]", AssignedPersonId, Description);
        }
    }

    /// <summary>
    /// This DTO class is used to send needed data to <see cref="ITaskAppService.UpdateTask"/> method.
    /// 
    /// Implements <see cref="ICustomValidate"/> for additional custom validation.
    /// </summary>
    [AutoMapTo(typeof(Task))]
    public class UpdateTaskInput : ICustomValidate
    {
        [Range(1, Int32.MaxValue)] //Data annotation attributes work as expected.
        public int Id { get; set; }

        public int? AssignedPersonId { get; set; }

        public TaskState? State { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        //Custom validation method. It's called by ABP after data annotation validations.
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (AssignedPersonId == null && State == null)
            {
                context.Results.Add(new ValidationResult("Both of AssignedPersonId and State can not be null in order to update a Task!", new[] { "AssignedPersonId", "State" }));
            }
        }

        public override string ToString()
        {
            return string.Format("[UpdateTaskInput > TaskId = {0}, AssignedPersonId = {1}, State = {2}]", Id, AssignedPersonId, State);
        }
    }

    public class GetTasksInput : PagedSortedAndFilteredInputDto
    {
        public TaskState? State { get; set; }

        public int? AssignedPersonId { get; set; }
    }
}
