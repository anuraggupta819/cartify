using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ECommerce.ProductCatalog.Application.Common;
using ECommerce.ProductCatalog.Application.Dtos;

namespace ECommerce.ProductCatalog.IntegrationTests;

public class ProductEndpointsTests(ProductCatalogApiFactory factory) : IClassFixture<ProductCatalogApiFactory>
{
    private readonly HttpClient _client = CreateAuthenticatedClient(factory);

    private static HttpClient CreateAuthenticatedClient(ProductCatalogApiFactory factory)
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestJwt.CreateAdminToken());
        return client;
    }

    [Fact]
    public async Task CreateProduct_ThenGetIt_ReturnsSameProduct()
    {
        var category = await CreateCategoryAsync("Electronics");
        var createRequest = new CreateProductRequest("Keyboard", "Mechanical keyboard", $"SKU-{Guid.NewGuid():N}", 49.99m, category.Id, null, 0);

        var createResponse = await _client.PostAsJsonAsync("/api/products", createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(created);

        var getResponse = await _client.GetAsync($"/api/products/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await getResponse.Content.ReadFromJsonAsync<ProductDto>();
        Assert.Equal(created.Id, fetched!.Id);
        Assert.Equal("Keyboard", fetched.Name);
    }

    [Fact]
    public async Task GetProduct_WithUnknownId_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WithNonPositivePrice_ReturnsBadRequest()
    {
        var category = await CreateCategoryAsync("Books");
        var createRequest = new CreateProductRequest("Free Book", "desc", $"SKU-{Guid.NewGuid():N}", 0m, category.Id, null, 0);

        var response = await _client.PostAsJsonAsync("/api/products", createRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetAllProducts_ReturnsCreatedProduct()
    {
        var category = await CreateCategoryAsync("Toys");
        var createRequest = new CreateProductRequest("Toy Car", "desc", $"SKU-{Guid.NewGuid():N}", 12.5m, category.Id, null, 0);
        var createResponse = await _client.PostAsJsonAsync("/api/products", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        var listResponse = await _client.GetAsync("/api/products?pageNumber=1&pageSize=50");
        var page = await listResponse.Content.ReadFromJsonAsync<PagedResult<ProductDto>>();

        Assert.Contains(page!.Items, p => p.Id == created!.Id);
    }

    [Fact]
    public async Task CreateProduct_WithUnknownCategory_ReturnsBadRequest()
    {
        var createRequest = new CreateProductRequest("Ghost Product", "desc", $"SKU-{Guid.NewGuid():N}", 9.99m, Guid.NewGuid(), null, 0);

        var response = await _client.PostAsJsonAsync("/api/products", createRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_RemovesIt()
    {
        var category = await CreateCategoryAsync("Garden");
        var createRequest = new CreateProductRequest("Shovel", "desc", $"SKU-{Guid.NewGuid():N}", 25m, category.Id, null, 0);
        var createResponse = await _client.PostAsJsonAsync("/api/products", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/products/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/products/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WithoutToken_ReturnsUnauthorized()
    {
        var anonymousClient = factory.CreateClient();
        var createRequest = new CreateProductRequest("Sneaky", "desc", $"SKU-{Guid.NewGuid():N}", 9.99m, Guid.NewGuid(), null, 0);

        var response = await anonymousClient.PostAsJsonAsync("/api/products", createRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_WithoutToken_StillSucceeds()
    {
        var anonymousClient = factory.CreateClient();

        var response = await anonymousClient.GetAsync("/api/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task<CategoryDto> CreateCategoryAsync(string name)
    {
        var response = await _client.PostAsJsonAsync("/api/categories", new CreateCategoryRequest(name));
        response.EnsureSuccessStatusCode();
        var category = await response.Content.ReadFromJsonAsync<CategoryDto>();
        return category!;
    }
}
