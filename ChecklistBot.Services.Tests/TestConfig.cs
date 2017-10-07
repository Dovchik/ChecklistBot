using ChecklistBot.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChecklistBot.Services.Tests
{
    class TestConfig
    {
        public static ChecklistBotContext CreateTestContext()
        {
            var builder = new DbContextOptionsBuilder<ChecklistBotContext>();
            var b = InMemoryDbContextOptionsExtensions.UseInMemoryDatabase(builder, "Data Source=:memory:");
            return new ChecklistBotContext(b.Options);
        }
    }
}
