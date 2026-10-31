using System;
using InventorySaaS.Models;

namespace InventorySaaS.Repositories;

public interface IStockMovementRepository
{
   
   Task<IEnumerable<StockMovement>> GetAllMovementsAsync(int tenantId);

    Task<StockMovement?> GetMovementByIdAsync(
        int tenantId,
        int Id);

     Task RecordMovementAsync(StockMovement movement);

     Task TransferStockAsync(int tenantId, int productId, int sourceWarehouseId, int destinationWarehouseId, decimal quantity, Guid transferId);

}
