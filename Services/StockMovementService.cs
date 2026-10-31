using System;
using InventorySaaS.DTOs.StockMovements;
using InventorySaaS.Models;
using InventorySaaS.Models.Enums;
using InventorySaaS.Repositories;

namespace InventorySaaS.Services;

public class StockMovementService : IStockMovementService
{

    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly ITenantRepository _tenantRepository;

    private readonly IProductRepository _productRepository;

    private readonly IWarehouseRepository _warehouseRepository;


    public StockMovementService(
        IStockMovementRepository stockMovementRepository,
        ITenantRepository tenantRepository,
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository)
    {
        _stockMovementRepository = stockMovementRepository;
        _tenantRepository = tenantRepository;
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
    }



   public async Task<IEnumerable<StockMovementDto>> GetAllMovementsAsync(
        int tenantId)
    {
        await ValidateTenantAsync(tenantId);

        var movements = await _stockMovementRepository
            .GetAllMovementsAsync(tenantId);

        return movements.Select(ToDto).ToList();
    }

    public async Task<StockMovementDto?> GetMovementByIdAsync(
        int tenantId,
        int id)
    {
        await ValidateTenantAsync(tenantId);

        var movement = await _stockMovementRepository
            .GetMovementByIdAsync(tenantId, id);

        return movement is null ? null : ToDto(movement);
    }

    public async Task<StockMovementDto> CreateMovementAsync(
        int tenantId,
        CreateStockMovementDto dto)
    {
        await ValidateTenantAsync(tenantId, requireActive: true);

        var product = await _productRepository
            .GetProductByIdAsync(tenantId, dto.ProductId);

        if (product is null)
        {
            throw new KeyNotFoundException(
                "المنتج غير موجود ضمن هذه الشركة.");
        }

        var warehouse = await _warehouseRepository
            .GetWarehouseByIdAsync(tenantId, dto.WarehouseId);

        if (warehouse is null)
        {
            throw new KeyNotFoundException(
                "المستودع غير موجود ضمن هذه الشركة.");
        }

        if (dto.Type is null ||
            (dto.Type != StockMovementType.In &&
             dto.Type != StockMovementType.Out))
        {
            throw new ArgumentException("نوع الحركة غير صالح.");
        }

        if (dto.Quantity is null ||
            dto.Quantity <= 0 ||
            dto.Quantity > 999999999999999.999m)
        {
            throw new ArgumentException("كمية الحركة غير صالحة.");
        }

        if (decimal.Round(dto.Quantity.Value, 3) != dto.Quantity.Value)
        {
            throw new ArgumentException(
                "الكمية يجب ألا تتجاوز ثلاث منازل عشرية.");
        }

        var movement = new StockMovement
        {
            TenantId = tenantId,
            ProductId = dto.ProductId,
            WarehouseId = dto.WarehouseId,
            Type = dto.Type.Value,
            Quantity = dto.Quantity.Value
        };

        await _stockMovementRepository.RecordMovementAsync(movement);

        return ToDto(movement);
    }

    private async Task ValidateTenantAsync(
        int tenantId,
        bool requireActive = false)
    {
        var tenant = await _tenantRepository
            .GetTenantByIdAsync(tenantId);

        if (tenant is null)
        {
            throw new KeyNotFoundException("الشركة غير موجودة.");
        }

        if (requireActive && !tenant.IsActive)
        {
            throw new InvalidOperationException(
                "لا يمكن تسجيل حركة لشركة غير مفعّلة.");
        }
    }

    private static StockMovementDto ToDto(StockMovement movement)
    {
        return new StockMovementDto
        {
            Id = movement.Id,
            TenantId = movement.TenantId,
            ProductId = movement.ProductId,
            WarehouseId = movement.WarehouseId,
            Type = movement.Type,
            Quantity = movement.Quantity,
            CreatedAtUtc = movement.CreatedAtUtc,
            TransferId = movement.TransferId
        };
    }

    public async Task<Guid> TransferStockAsync(int tenantId, TransferStockDto dto)
    {
        
        await ValidateTenantAsync(tenantId, requireActive: true);


        if (dto.sourceWarehouseId == dto.destinationWarehouseId)
        {
            throw new ArgumentException("مصدر ومستودع الوجهة لا يمكن أن يكونا نفس المستودع.");
        }

        if (dto.quantity is null || dto.quantity <= 0 || dto.quantity > 999999999999999.999m)
        {
            throw new ArgumentException("كمية النقل غير صالحة.");
        }

        var quantity = dto.quantity.Value;

        if (decimal.Round(quantity, 3) != quantity)
        {
            throw new ArgumentException(
                "الكمية يجب ألا تتجاوز ثلاث منازل عشرية.");
        }

        var product = await _productRepository.GetProductByIdAsync(tenantId, dto.productId);
        if (product is null)
        {
            throw new KeyNotFoundException("المنتج غير موجود ضمن هذه الشركة.");
        }

        var destinationWarehouse = await _warehouseRepository.GetWarehouseByIdAsync(tenantId, dto.destinationWarehouseId);
        if (destinationWarehouse is null)
        {
            throw new KeyNotFoundException("المستودع الوجهة غير موجود ضمن هذه الشركة.");
        }

        var transferId = Guid.NewGuid();

        await _stockMovementRepository.TransferStockAsync(
            tenantId,
            dto.productId,
            dto.sourceWarehouseId,
            dto.destinationWarehouseId,
            quantity,
            transferId);

         return transferId;   
        
    }
}
