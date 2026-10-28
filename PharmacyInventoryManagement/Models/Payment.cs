namespace PharmacyInventoryManagement.Models;

public class Payment
{
    public int PaymentId { get; set; }
    public int SaleId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = "Cash";
    public string PaymentStatus { get; set; } = "Paid";
    public string? TransactionReference { get; set; }
    public DateTime PaidAt { get; set; }
}