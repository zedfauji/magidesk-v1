using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;
using Magidesk.Application.Interfaces;
using MediatR;

namespace Magidesk.Application.Commands;

public class SetCategoryParentCommandHandler : IRequestHandler<SetCategoryParentCommand, SetCategoryParentResult>
{
    private readonly IRepository<MenuCategory> _categoryRepository;

    public SetCategoryParentCommandHandler(IRepository<MenuCategory> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<SetCategoryParentResult> Handle(SetCategoryParentCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category == null)
            return new SetCategoryParentResult(false, "Category not found.");

        // Validate circular reference if setting a parent
        if (request.ParentCategoryId.HasValue)
        {
            if (await WouldCreateCircularReference(request.CategoryId, request.ParentCategoryId.Value, cancellationToken))
            {
                return new SetCategoryParentResult(false, "Cannot create circular reference in category hierarchy.");
            }

            // Verify parent exists
            var parent = await _categoryRepository.GetByIdAsync(request.ParentCategoryId.Value, cancellationToken);
            if (parent == null)
                return new SetCategoryParentResult(false, "Parent category not found.");
        }

        // Set parent (domain validation handles self-reference)
        try
        {
            category.SetParent(request.ParentCategoryId);
            await _categoryRepository.UpdateAsync(category, cancellationToken);
            return new SetCategoryParentResult(true, "Category parent updated successfully.");
        }
        catch (Domain.Exceptions.BusinessRuleViolationException ex)
        {
            return new SetCategoryParentResult(false, ex.Message);
        }
    }

    /// <summary>
    /// Checks if setting the new parent would create a circular reference.
    /// Walks up the ancestor chain to detect cycles.
    /// </summary>
    private async Task<bool> WouldCreateCircularReference(Guid categoryId, Guid newParentId, CancellationToken cancellationToken)
    {
        var current = newParentId;
        int safety = 0;

        while (current != Guid.Empty && safety++ < 100)
        {
            if (current == categoryId)
                return true; // Cycle detected

            var parent = await _categoryRepository.GetByIdAsync(current, cancellationToken);
            if (parent == null)
                break; // Chain ends

            current = parent.ParentCategoryId ?? Guid.Empty;
        }

        return false;
    }
}
