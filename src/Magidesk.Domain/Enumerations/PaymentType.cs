namespace Magidesk.Domain.Enumerations;

/// <summary>
/// Represents the type of payment.
/// </summary>
public enum PaymentType
{
    Cash = 0,
    CreditCard = 1,
    CreditVisa = 2,
    CreditMasterCard = 3,
    CreditAmex = 4,
    CreditDiscover = 5,
    DebitCard = 6,
    DebitVisa = 7,
    DebitMasterCard = 8,
    GiftCertificate = 9,
    CustomPayment = 10
}

