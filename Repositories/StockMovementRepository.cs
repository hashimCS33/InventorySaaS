using System;
using System.Data;
using InventorySaaS.Data;
using InventorySaaS.Models;
using InventorySaaS.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace InventorySaaS.Repositories;

public class StockMovementRepository : IStockMovementRepository
{
    private readonly AppDbContext _context;

    public StockMovementRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task RecordMovementAsync(StockMovement movement)
    {
        const decimal maxQuantity = 999999999999999.999m;

        if (movement.Quantity <= 0 ||
            movement.Quantity > maxQuantity)
        {
            throw new ArgumentException("كمية الحركة غير صالحة.");
        }

        if (decimal.Round(movement.Quantity, 3) != movement.Quantity)
        {
            throw new ArgumentException(
                "الكمية يجب ألا تتجاوز ثلاث منازل عشرية.");
        }

        if (movement.Type != StockMovementType.In &&
            movement.Type != StockMovementType.Out)
        {
            throw new ArgumentException("نوع الحركة غير صالح.");
        }

        await using var transaction = await _context.Database
            .BeginTransactionAsync(IsolationLevel.Serializable);

        var balance = await _context.StockBalances
            .FirstOrDefaultAsync(item =>
                item.TenantId == movement.TenantId &&
                item.ProductId == movement.ProductId &&
                item.WarehouseId == movement.WarehouseId);

        if (movement.Type == StockMovementType.In)
        {
            if (balance is null)
            {
                balance = new StockBalance
                {
                    TenantId = movement.TenantId,
                    ProductId = movement.ProductId,
                    WarehouseId = movement.WarehouseId,
                    Quantity = 0
                };

                _context.StockBalances.Add(balance);
            }

            if (balance.Quantity > maxQuantity - movement.Quantity)
            {
                throw new InvalidOperationException(
                    "الرصيد سيتجاوز الحد المسموح.");
            }

            balance.Quantity += movement.Quantity;
        }
        else
        {
            if (balance is null ||
                balance.Quantity < movement.Quantity)
            {
                throw new InvalidOperationException(
                    "الكمية المتوفرة لا تكفي للإخراج.");
            }

            balance.Quantity -= movement.Quantity;
        }

        movement.CreatedAtUtc = DateTime.UtcNow;

        _context.StockMovements.Add(movement);

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();
    }

    public async Task<IEnumerable<StockMovement>> GetAllMovementsAsync(int tenantId)
    {
        return await _context.StockMovements
           .AsNoTracking()
           .Where(movement => movement.TenantId == tenantId)
           .OrderByDescending(movement => movement.CreatedAtUtc)
           .ThenByDescending(movement => movement.Id)
           .ToListAsync();
    }

    public async Task<StockMovement?> GetMovementByIdAsync(int tenantId, int id)
    {
        return await _context.StockMovements
             .AsNoTracking()
             .FirstOrDefaultAsync(movement =>
                 movement.TenantId == tenantId &&
                 movement.Id == id);
    }

    public async Task TransferStockAsync(int tenantId, int productId, int sourceWarehouseId, int destinationWarehouseId, decimal quantity, Guid transferId)
    {
        const decimal maxQuantity = 999999999999999.999m;

        if (sourceWarehouseId == destinationWarehouseId)
        {
            throw new ArgumentException("مصدر ومستودع الوجهة لا يمكن أن يكونا نفس المستودع.");
        }

        if (quantity <= 0 || quantity > maxQuantity)
        {
            throw new ArgumentException("كمية النقل غير صالحة.");
        }

        if (decimal.Round(quantity, 3) != quantity)
        {
            throw new ArgumentException("الكمية يجب ألا تتجاوز ثلاث منازل عشرية.");
        }

        if (transferId == Guid.Empty)
        {
            throw new ArgumentException("معرف النقل غير صالح.");
        }

        await using var transaction = await _context.Database
            .BeginTransactionAsync(IsolationLevel.Serializable);

        var sourceBalance = await _context.StockBalances
            .FirstOrDefaultAsync(balance =>
                balance.TenantId == tenantId &&
                balance.ProductId == productId &&
                balance.WarehouseId == sourceWarehouseId);
        if (sourceBalance is null || sourceBalance.Quantity < quantity)
        {
            throw new InvalidOperationException("الكمية المتوفرة في المستودع المصدر لا تكفي للنقل.");
        }

        var destinationBalance = await _context.StockBalances
            .FirstOrDefaultAsync(balance =>
                balance.TenantId == tenantId &&
                balance.ProductId == productId &&
                balance.WarehouseId == destinationWarehouseId);       

        if (destinationBalance is null)
        {
            destinationBalance = new StockBalance
            {
                TenantId = tenantId,
                ProductId = productId,
                WarehouseId = destinationWarehouseId,
                Quantity = 0
            };

            _context.StockBalances.Add(destinationBalance);
        }

       if (destinationBalance.Quantity > maxQuantity - quantity)
        {
            throw new InvalidOperationException("الرصيد في المستودع الوجهة سيتجاوز الحد المسموح.");
        }

        sourceBalance.Quantity -= quantity;
        destinationBalance.Quantity += quantity;
        
        var createdAtUtc = DateTime.UtcNow;

        var outMovement = new StockMovement
        {
            TenantId = tenantId,
            ProductId = productId,
            WarehouseId = sourceWarehouseId,
            Quantity = quantity,
            Type = StockMovementType.Out,
            CreatedAtUtc = DateTime.UtcNow,
            TransferId = transferId
        };

        var incomingMovement = new StockMovement
        {
            TenantId = tenantId,
            ProductId = productId,
            WarehouseId = destinationWarehouseId,
            Quantity = quantity,
            Type = StockMovementType.In,
            CreatedAtUtc = DateTime.UtcNow,
            TransferId = transferId
        };

        _context.StockMovements.AddRange(
            outMovement,
            incomingMovement);
        

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();         
    }
    }

