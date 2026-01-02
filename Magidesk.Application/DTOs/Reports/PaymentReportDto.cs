using Magidesk.Application.DTOs;

namespace Magidesk.Application.DTOs.Reports;

public class PaymentReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<PaymentTypeTotalDto> PaymentTypes { get; set; } = new();
    public PaymentReportTotalsDto Totals { get; set; } = new();
}

public class PaymentTypeTotalDto
{
    public string PaymentType { get; set; } = string.Empty; // Cash, Credit Card, Debit Card, Gift Certificate, House Account, Check, Other
    public string SubType { get; set; } = string.Empty; // For credit cards: Visa, MasterCard, Amex, etc.
    public int TransactionCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TipAmount { get; set; }
    public decimal NetAmount => TotalAmount + TipAmount;
}

public class PaymentReportTotalsDto
{
    public int TotalTransactions { get; set; }
    public decimal TotalCash { get; set; }
    public decimal TotalCreditCards { get; set; }
    public decimal TotalDebitCards { get; set; }
    public decimal TotalGiftCertificates { get; set; }
    public decimal TotalHouseAccounts { get; set; }
    public decimal TotalChecks { get; set; }
    public decimal TotalOther { get; set; }
    public decimal GrandTotal => TotalCash + TotalCreditCards + TotalDebitCards + TotalGiftCertificates + TotalHouseAccounts + TotalChecks + TotalOther;
    public decimal TotalTips { get; set; }
}
