using System;
using InventorySaaS.DTOs.StockMovements;

namespace InventorySaaS.Services;

public interface IStockMovementService
{
    Task<IEnumerable<StockMovementDto>> GetAllMovementsAsync(int tenantId);

    Task<StockMovementDto?> GetMovementByIdAsync(
        int tenantId,
        int Id);

    Task<StockMovementDto> CreateMovementAsync(int tenantId, CreateStockMovementDto dto);
  
    Task<Guid> TransferStockAsync(int tenantId, TransferStockDto dto);
}
