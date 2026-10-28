namespace PharmacyInventoryManagement.Models;

public class Sale
{
    public int SaleId { get; set; }
    public int MedicineId { get; set; }
    public string MedicineNameSnapshot { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime SoldAt { get; set; }
    public string SaleStatus { get; set; } = "Completed";
}