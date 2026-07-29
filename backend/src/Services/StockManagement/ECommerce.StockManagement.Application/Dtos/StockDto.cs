namespace ECommerce.StockManagement.Application.Dtos;

public record StockDto(Guid ProductId, int Quantity, int Reserved, int Available);

public record SetStockQuantityRequest(int Quantity);

public record StockAdjustmentRequest(int Quantity);
