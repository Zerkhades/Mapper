using Mapper.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Tests.Common;

public class PaginatedListTests
{
    [Fact]
    public void Constructor_ShouldCalculateTotalPagesCorrectly()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3, 4, 5 };
        var totalCount = 23;
        var pageNumber = 1;
        var pageSize = 5;

        // Act
        var paginatedList = new PaginatedList<int>(items, totalCount, pageNumber, pageSize);

        // Assert
        Assert.Equal(5, paginatedList.TotalPages);
        Assert.Equal(23, paginatedList.TotalCount);
        Assert.Equal(1, paginatedList.PageNumber);
        Assert.Equal(5, paginatedList.Items.Count);
    }

    [Fact]
    public void HasPreviousPage_FirstPage_ShouldBeFalse()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3 };
        var paginatedList = new PaginatedList<int>(items, 10, 1, 3);

        // Act & Assert
        Assert.False(paginatedList.HasPreviousPage);
    }

    [Fact]
    public void HasPreviousPage_SecondPage_ShouldBeTrue()
    {
        // Arrange
        var items = new List<int> { 4, 5, 6 };
        var paginatedList = new PaginatedList<int>(items, 10, 2, 3);

        // Act & Assert
        Assert.True(paginatedList.HasPreviousPage);
    }

    [Fact]
    public void HasNextPage_LastPage_ShouldBeFalse()
    {
        // Arrange
        var items = new List<int> { 9, 10 };
        var paginatedList = new PaginatedList<int>(items, 10, 4, 3);

        // Act & Assert
        Assert.False(paginatedList.HasNextPage);
    }

    [Fact]
    public void HasNextPage_NotLastPage_ShouldBeTrue()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3 };
        var paginatedList = new PaginatedList<int>(items, 10, 1, 3);

        // Act & Assert
        Assert.True(paginatedList.HasNextPage);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreatePaginatedListFromQueryable()
    {
        // Arrange
        var data = Enumerable.Range(1, 25).ToList();
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new TestDbContext(options);
        context.TestEntities.AddRange(data.Select(i => new TestEntity { Id = i, Value = $"Item{i}" }));
        await context.SaveChangesAsync();

        var queryable = context.TestEntities.OrderBy(x => x.Id);

        // Act
        var result = await PaginatedList<TestEntity>.CreateAsync(queryable, 2, 10);

        // Assert
        Assert.Equal(25, result.TotalCount);
        Assert.Equal(3, result.TotalPages);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(10, result.Items.Count);
        Assert.True(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
    }

    [Fact]
    public async Task CreateAsync_FirstPage_ShouldReturnCorrectItems()
    {
        // Arrange
        var data = Enumerable.Range(1, 15).ToList();
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new TestDbContext(options);
        context.TestEntities.AddRange(data.Select(i => new TestEntity { Id = i, Value = $"Item{i}" }));
        await context.SaveChangesAsync();

        var queryable = context.TestEntities.OrderBy(x => x.Id);

        // Act
        var result = await PaginatedList<TestEntity>.CreateAsync(queryable, 1, 5);

        // Assert
        Assert.Equal(5, result.Items.Count);
        Assert.Equal(1, result.Items.First().Id);
        Assert.Equal(5, result.Items.Last().Id);
    }

    [Fact]
    public void TotalPages_WithExactDivision_ShouldCalculateCorrectly()
    {
        // Arrange & Act
        var paginatedList = new PaginatedList<int>(new List<int>(), 20, 1, 5);

        // Assert
        Assert.Equal(4, paginatedList.TotalPages);
    }

    [Fact]
    public void TotalPages_WithRemainder_ShouldRoundUp()
    {
        // Arrange & Act
        var paginatedList = new PaginatedList<int>(new List<int>(), 21, 1, 5);

        // Assert
        Assert.Equal(5, paginatedList.TotalPages);
    }

    private class TestEntity
    {
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty;
    }

    private class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
        public DbSet<TestEntity> TestEntities { get; set; } = null!;
    }
}
