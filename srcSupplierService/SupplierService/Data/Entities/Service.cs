using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;

namespace SupplierService.Data.Entities;

public class Service : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public Guid SupplierId { get; set; } //TODO: SUPPLIERID, aggiungere anche campo apposta per "Altro" in modo che un utente possa mettere la descrizione che vuole
}