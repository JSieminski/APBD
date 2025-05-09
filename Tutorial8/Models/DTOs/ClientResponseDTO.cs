namespace Tutorial8.Models.DTOs;

public class ClientTripResponseDTO
{
    public string TripName { get; set; }
    public DateTime RegisteredAt { get; set; }
    public DateTime? PaymentDate { get; set; }
    public List<CountryDTO> Countries { get; set; }
}