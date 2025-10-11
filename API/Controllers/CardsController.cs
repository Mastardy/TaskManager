using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.Models;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;

[ApiController]
[Route("api/v1/cards")]
[Authorize]
public class CardsController : ControllerBase
{
    private readonly CardsService m_CardsService;

    public CardsController(CardsService cardsService)
    {
        m_CardsService = cardsService;
    }

    [HttpGet]
    public async Task<List<Card>> Get() => await m_CardsService.GetAllAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Card>> Get(string id)
    {
        Console.WriteLine(User.Identity?.Name);
        var card = await m_CardsService.GetAsync(id);
        return card is null ? NotFound() : card;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Card newCard)
    {
        await m_CardsService.CreateAsync(newCard);
        return CreatedAtAction(nameof(Get), new { id = newCard.Id }, newCard);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Card updatedCard)
    {
        var card = await m_CardsService.GetAsync(id);
        if (card is null) return NotFound();

        updatedCard.Id = id;

        await m_CardsService.UpdateAsync(updatedCard);
        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        var card = await m_CardsService.GetAsync(id);
        if (card is null) return NotFound();

        await m_CardsService.DeleteAsync(id);
        return NoContent();
    }
}