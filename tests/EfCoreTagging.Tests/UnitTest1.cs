using System;
using System.Linq;
using System.Threading.Tasks;
using EfCoreTagging.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace EfCoreTagging.Tests
{
    public class UnitTest1
    {
        private readonly ILoggerFactory _loggerFactory;

        public UnitTest1(ITestOutputHelper outputHelper)
        {
            _loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter((category, level) =>
                        category == DbLoggerCategory.Database.Command.Name
                        && level == LogLevel.Information)
                    .AddXUnit(outputHelper);
            });
        }

        [Fact]
        public async Task Test1()
        {
            await using var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<BloggingContext>()
                .UseSqlite(connection)
                .UseLoggerFactory(_loggerFactory)
                .Options;

            var bloggingContext = new BloggingContext(options);
            await bloggingContext.Database.EnsureCreatedAsync();
            var result = await bloggingContext.Blogs
                .Where(i => i.Url.StartsWith("https://"))
                .Take(5)
                .OrderBy(i => i.BlogId)
                .TagWithSource()
                .ToListAsync();
        }

        [Fact]
        public async Task Test1_WithToList()
        {
            await using var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<BloggingContext>()
                .UseSqlite(connection)
                .UseLoggerFactory(_loggerFactory)
                .Options;

            var bloggingContext = new BloggingContext(options);
            await bloggingContext.Database.EnsureCreatedAsync();
            var result = await bloggingContext.Blogs
                .Where(i => i.Url.StartsWith("https://"))
                .Take(5)
                .OrderBy(i => i.BlogId)
                .ToListWithSourceAsync();
        }
    }
}
