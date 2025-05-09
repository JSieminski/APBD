using Tutorial8.Models;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface ITripsService
{
    Task<List<TripDTO>> GetTrips();
    Task<List<ClientTripResponseDTO>> GetClientTrips(int clientId);
    Task<int> CreateClient(ClientDTO clientDto);
    Task RegisterClientForTrip(int clientId, int tripId);
    Task DeleteClientTrip(int clientId, int tripId);
    Task<bool> ClientExists(int clientId);
    Task<bool> TripExists(int tripId);
    Task<Client> GetClientByPesel(string pesel);
    Task<Client> GetClientById(int id);
}