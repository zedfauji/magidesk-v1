using System;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

public interface IGroupSettlementRepository
{
    Task AddAsync(GroupSettlement groupSettlement);
    Task<GroupSettlement?> GetByIdAsync(Guid id);
}
