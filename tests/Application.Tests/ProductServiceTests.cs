using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Exceptions;
using Moq;
using Xunit;

namespace Application.Tests;

public sealed class ProductServiceTests
{
    [Fact]
    public async Task CreateAsync_SetsAuditFields_AndPersists()
    {
        var repo = new Mock<IProductRepository>();
        Product? captured = null;
        repo.Setup(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Callback<Product, CancellationToken>((p, _) => { p.Id = 10; captured = p; })
            .Returns(Task.CompletedTask);
        repo.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var service = new ProductService(repo.Object);

        var result = await service.CreateAsync(new CreateProductRequest(" Monitor "), "admin", CancellationToken.None);

        Assert.Equal(10, result.Id);
        Assert.Equal("Monitor", result.ProductName);
        Assert.Equal("admin", result.CreatedBy);
        Assert.NotNull(captured);
        repo.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMissing_ThrowsNotFoundException()
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);
        var service = new ProductService(repo.Object);

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(99, CancellationToken.None));
    }

    [Fact]
    public async Task GetAsync_ReturnsPagedResult()
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(x => x.GetPagedAsync(2, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Product> { new() { Id = 3, ProductName = "Mouse", CreatedBy = "seed", CreatedOn = DateTime.UtcNow } } as IReadOnlyCollection<Product>, 3));
        var service = new ProductService(repo.Object);

        var result = await service.GetAsync(2, 2, CancellationToken.None);

        Assert.Equal(3, result.TotalCount);
        Assert.Equal(2, result.PageNumber);
        Assert.Single(result.Items);
    }
}
