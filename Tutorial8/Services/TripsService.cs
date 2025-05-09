using Microsoft.Data.SqlClient;
using Tutorial8.Models;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString;

    public TripsService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    
    public async Task<List<TripDTO>> GetTrips()
    {
        var trips = new Dictionary<int, TripDTO>();

        const string query = @"
            SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, c.Name AS CountryName
            FROM Trip t
            JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip
            JOIN Country c ON ct.IdCountry = c.IdCountry
            ORDER BY t.DateFrom DESC";

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(query, conn);
        
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            var tripId = reader.GetInt32(0);
            if (!trips.ContainsKey(tripId))
            {
                trips[tripId] = new TripDTO
                {
                    IdTrip = tripId,
                    Name = reader.GetString(1),
                    Description = reader.GetString(2),
                    DateFrom = reader.GetDateTime(3),
                    DateTo = reader.GetDateTime(4),
                    MaxPeople = reader.GetInt32(5),
                    Countries = new List<CountryDTO>()
                };
            }
            trips[tripId].Countries.Add(new CountryDTO { Name = reader.GetString(6) });
        }
        
        return trips.Values.ToList();
    }
    
    public async Task<List<ClientTripResponseDTO>> GetClientTrips(int clientId)
    {
        var trips = new List<ClientTripResponseDTO>();

        const string query = @"
            SELECT t.Name, ct.RegisteredAt, ct.PaymentDate, c.Name AS CountryName
            FROM Client_Trip ct
            JOIN Trip t ON ct.IdTrip = t.IdTrip
            JOIN Country_Trip ct2 ON t.IdTrip = ct2.IdTrip
            JOIN Country c ON ct2.IdCountry = c.IdCountry
            WHERE ct.IdClient = @ClientId";

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@ClientId", clientId);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            var trip = new ClientTripResponseDTO
            {
                TripName = reader.GetString(0),
                RegisteredAt = DateTime.ParseExact(reader.GetInt32(1).ToString(), "yyyyMMdd", null),
                PaymentDate = reader.IsDBNull(2) ? null : DateTime.ParseExact(reader.GetInt32(2).ToString(), "yyyyMMdd", null),
                Countries = new List<CountryDTO>()
            };
            trip.Countries.Add(new CountryDTO { Name = reader.GetString(3) });
            trips.Add(trip);
        }
        
        return trips;
    }
    
    public async Task<int> CreateClient(ClientDTO clientDto)
    {
        const string query = @"
            INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel)
            OUTPUT INSERTED.IdClient
            VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel)";

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(query, conn);
        
        cmd.Parameters.AddWithValue("@FirstName", clientDto.FirstName);
        cmd.Parameters.AddWithValue("@LastName", clientDto.LastName);
        cmd.Parameters.AddWithValue("@Email", clientDto.Email);
        cmd.Parameters.AddWithValue("@Telephone", clientDto.Telephone ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@Pesel", clientDto.Pesel);

        await conn.OpenAsync();
        return (int)await cmd.ExecuteScalarAsync();
    }


    public async Task RegisterClientForTrip(int clientId, int tripId)
    {
        var trip = await GetTrip(tripId);
        if (trip == null) throw new ArgumentException("Wycieczka nie istnieje");
        if (trip.DateFrom < DateTime.Now) throw new InvalidOperationException("Wycieczka już się rozpoczęła");
        
        var participants = await GetTripParticipantsCount(tripId);
        if (participants >= trip.MaxPeople) throw new InvalidOperationException("Brak miejsc");
        if (await IsClientRegistered(clientId, tripId)) throw new InvalidOperationException("Klient już zapisany");

        const string query = @"
            INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt)
            VALUES (@ClientId, @TripId, @RegisteredAt)";
        
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(query, conn);
        
        cmd.Parameters.AddWithValue("@ClientId", clientId);
        cmd.Parameters.AddWithValue("@TripId", tripId);
        cmd.Parameters.AddWithValue("@RegisteredAt", DateTime.Now.ToString("yyyyMMdd"));

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }


    public async Task DeleteClientTrip(int clientId, int tripId)
    {
        const string query = @"
            DELETE FROM Client_Trip 
            WHERE IdClient = @ClientId AND IdTrip = @TripId";
        
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(query, conn);
        
        cmd.Parameters.AddWithValue("@ClientId", clientId);
        cmd.Parameters.AddWithValue("@TripId", tripId);
        
        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }



    public async Task<bool> ClientExists(int clientId)
    {
        const string query = "SELECT 1 FROM Client WHERE IdClient = @ClientId";
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@ClientId", clientId);
        await conn.OpenAsync();
        return await cmd.ExecuteScalarAsync() != null;
    }

    public async Task<bool> TripExists(int tripId)
    {
        const string query = "SELECT 1 FROM Trip WHERE IdTrip = @TripId";
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@TripId", tripId);
        await conn.OpenAsync();
        return await cmd.ExecuteScalarAsync() != null;
    }

    public async Task<Client> GetClientByPesel(string pesel)
    {
        const string query = "SELECT * FROM Client WHERE Pesel = @Pesel";
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@Pesel", pesel);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        
        if (await reader.ReadAsync())
        {
            return new Client
            {
                IdClient = reader.GetInt32(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2),
                Email = reader.GetString(3),
                Telephone = reader.IsDBNull(4) ? null : reader.GetString(4),
                Pesel = reader.GetString(5)
            };
        }
        return null;
    }

    private async Task<Trip> GetTrip(int tripId)
    {
        const string query = "SELECT * FROM Trip WHERE IdTrip = @TripId";
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@TripId", tripId);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        
        if (await reader.ReadAsync())
        {
            return new Trip
            {
                IdTrip = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                DateFrom = reader.GetDateTime(3),
                DateTo = reader.GetDateTime(4),
                MaxPeople = reader.GetInt32(5)
            };
        }
        return null;
    }

    private async Task<int> GetTripParticipantsCount(int tripId)
    {
        const string query = "SELECT COUNT(*) FROM Client_Trip WHERE IdTrip = @TripId";
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@TripId", tripId);
        await conn.OpenAsync();
        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }

    private async Task<bool> IsClientRegistered(int clientId, int tripId)
    {
        const string query = "SELECT 1 FROM Client_Trip WHERE IdClient = @ClientId AND IdTrip = @TripId";
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@ClientId", clientId);
        cmd.Parameters.AddWithValue("@TripId", tripId);
        await conn.OpenAsync();
        return await cmd.ExecuteScalarAsync() != null;
    }
    public async Task<Client> GetClientById(int id)
    {
        const string query = "SELECT * FROM Client WHERE IdClient = @Id";
    
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@Id", id);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
    
        if (await reader.ReadAsync())
        {
            return new Client
            {
                IdClient = reader.GetInt32(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2),
                Email = reader.GetString(3),
                Telephone = reader.IsDBNull(4) ? null : reader.GetString(4),
                Pesel = reader.GetString(5)
            };
        }
        return null;
    }
}