using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBankApplication.Models;

public record Remittance
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public decimal Amount { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;


    public Guid SenderId { get; set; }
    public Guid RecipientId { get; set; }


    [ForeignKey(nameof(SenderId))]
    public virtual User? Sender { get; set; }

    [ForeignKey(nameof(RecipientId))]
    public virtual User? Recipient { get; set; }
}
