using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Services;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/v1/tasks")]
public class TasksController : ControllerBase
{
    private readonly TasksService m_TasksService;

    public TasksController(TasksService tasksService)
    {
        m_TasksService = tasksService;
    }

    [HttpGet]
    public async Task<List<Models.Task>> Get() => await m_TasksService.GetAllAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Models.Task>> Get(string id)
    {
        var task = await m_TasksService.GetAsync(id);
        return task is null ? NotFound() : task;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Models.Task newTask)
    {
        await m_TasksService.CreateAsync(newTask);
        return CreatedAtAction(nameof(Get), new { id = newTask.Id }, newTask);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Models.Task updatedTask)
    {
        var task = await m_TasksService.GetAsync(id);
        if (task is null) return NotFound();

        updatedTask.Id = id;

        await m_TasksService.UpdateAsync(updatedTask);
        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        var task = await m_TasksService.GetAsync(id);
        if (task is null) return NotFound();

        await m_TasksService.DeleteAsync(id);
        return NoContent();
    }
}