using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Tests.TestDoubles;

internal sealed class InMemoryMenuRepository : IMenuRepository
{
    private readonly Dictionary<Guid, MenuItem> _items = new();
    private readonly Dictionary<Guid, MenuModifier> _modifiers = new();

    public Task AddAsync(MenuItem menuItem, CancellationToken cancellationToken = default)
    {
        _items[menuItem.Id] = menuItem;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(MenuItem menuItem, CancellationToken cancellationToken = default)
    {
        _items.Remove(menuItem.Id);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<MenuItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<MenuItem>>(_items.Values.ToList());
    }

    public Task<IEnumerable<MenuItem>> GetByGroupAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        var result = _items.Values.Where(i => i.GroupId == groupId).ToList();
        return Task.FromResult<IEnumerable<MenuItem>>(result);
    }

    public Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _items.TryGetValue(id, out var item);
        return Task.FromResult(item);
    }

    public Task<MenuModifier?> GetModifierByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _modifiers.TryGetValue(id, out var modifier);
        return Task.FromResult(modifier);
    }

    public Task UpdateAsync(MenuItem menuItem, CancellationToken cancellationToken = default)
    {
        _items[menuItem.Id] = menuItem;
        return Task.CompletedTask;
    }

    // Helper for setup
    public void AddModifier(MenuModifier modifier)
    {
        _modifiers[modifier.Id] = modifier;
    }

    private readonly Dictionary<Guid, ComboDefinition> _comboDefinitions = new();

    public Task<ComboDefinition?> GetComboDefinitionByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _comboDefinitions.TryGetValue(id, out var def);
        return Task.FromResult(def);
    }
    
    public void AddComboDefinition(ComboDefinition comboDefinition)
    {
        _comboDefinitions[comboDefinition.Id] = comboDefinition;
    }
}
