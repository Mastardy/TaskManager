using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Services;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/v1/")]
public class TasksController : ControllerBase
{
    private readonly TasksService _tasksService;

    public TasksController(TasksService tasksService) => _tasksService = tasksService;

    [HttpGet("tasks")]
    public async Task<List<Models.Task>> Get()
    {
        return await _tasksService.GetTasksAsync();
    }
    
    
}