using FluentValidation;
using Magidesk.Application.DTOs;

namespace Magidesk.Application.Commands;

public class BulkUpdateInventoryItemsCommandValidator : AbstractValidator<BulkUpdateInventoryItemsCommand>
{
    public BulkUpdateInventoryItemsCommandValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Items list must not be empty.");

        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                item.RuleFor(e => e.NewStockQuantity)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Stock quantity must be greater than or equal to 0.");

                item.RuleFor(e => e.NewReorderPoint)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Reorder point must be greater than or equal to 0.");
            });
    }
}
