using ECommerce.StockManagement.Application.Dtos;
using ECommerce.StockManagement.Domain.Entities;

namespace ECommerce.StockManagement.Application.Mapping;

public static class StockMappingExtensions
{
    public static StockDto ToDto(this Stock stock) =>
        new(stock.ProductId, stock.Quantity, stock.Reserved, stock.Available);
}
