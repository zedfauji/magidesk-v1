using Magidesk.Application.DTOs;

namespace Magidesk.Application.DTOs.Reports;

public class CreditCardReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<CreditCardTransactionDto> Transactions { get; set; } = new();
    public CreditCardTotalsDto Totals { get; set; } = new();
}

public class CreditCardTransactionDto
{
    public DateTime TransactionTime { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string CardType { get; set; } = string.Empty; // Visa, MasterCard, Amex, etc.
    public string CardLast4 { get; set; } = string.Empty; // PCI compliant - only last 4 digits
    public string AuthorizationCode { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal TipAmount { get; set; }
    public decimal TotalAmount => Amount + TipAmount;
    public string TransactionType { get; set; } = string.Empty; // Sale, Void, Refund
    public string TransactionStatus { get; set; } = string.Empty; // Approved, Declined, Pending
    public string TerminalId { get; set; } = string.Empty;
    public string MerchantId { get; set; } = string.Empty;
}

public class CreditCardTotalsDto
{
    public int TotalTransactions { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalTips { get; set; }
    public decimal TotalVoids { get; set; }
    public decimal TotalRefunds { get; set; }
    public decimal NetAmount => TotalSales - TotalVoids - TotalRefunds;
    
    public List<CardTypeTotalDto> ByCardType { get; set; } = new();
}

public class CardTypeTotalDto
{
    public string CardType { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TipAmount { get; set; }
}
