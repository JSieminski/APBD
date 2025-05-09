using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;
using Tutorial8.Services;

namespace Tutorial8.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly ITripsService _tripsService;

    public ClientsController(ITripsService tripsService)
    {
        _tripsService = tripsService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateClient([FromBody] ClientDTO clientDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (clientDto.Pesel.Length != 11 || !clientDto.Pesel.All(char.IsDigit)) 
            return BadRequest("Nieprawidłowy PESEL");

        try
        {
            var existingClient = await _tripsService.GetClientByPesel(clientDto.Pesel);
            if (existingClient != null) return Conflict("Klient już istnieje");

            var clientId = await _tripsService.CreateClient(clientDto);
            return CreatedAtAction(nameof(GetClient), new { id = clientId }, clientDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetClientTrips(int id)
    {
        if (!await _tripsService.ClientExists(id)) return NotFound();
        
        var trips = await _tripsService.GetClientTrips(id);
        return Ok(trips);
    }

    [HttpPut("{id}/trips/{tripId}")]
    public async Task<IActionResult> RegisterForTrip(int id, int tripId)
    {
        try
        {
            if (!await _tripsService.ClientExists(id)) return NotFound("Klient nie istnieje");
            if (!await _tripsService.TripExists(tripId)) return NotFound("Wycieczka nie istnieje");

            await _tripsService.RegisterClientForTrip(id, tripId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}/trips/{tripId}")]
    public async Task<IActionResult> DeleteRegistration(int id, int tripId)
    {
        try
        {
            if (!await _tripsService.ClientExists(id)) return NotFound("Klient nie istnieje");
            if (!await _tripsService.TripExists(tripId)) return NotFound("Wycieczka nie istnieje");

            await _tripsService.DeleteClientTrip(id, tripId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetClient(int id)
    {
        var client = await _tripsService.GetClientById(id);
        return client != null ? Ok(client) : NotFound();
    }
}