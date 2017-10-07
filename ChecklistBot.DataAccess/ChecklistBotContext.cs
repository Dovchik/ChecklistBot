using ChecklistBot.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System;

namespace ChecklistBot.DataAccess
{
    public class ChecklistBotContext : DbContext
    {
        private readonly DbContextOptions<ChecklistBotContext> _options;
        public ChecklistBotContext()
        {

        }
        public ChecklistBotContext(DbContextOptions<ChecklistBotContext> options) : base(options)
        {
            _options = options;
        }
        public DbSet<CheckList> CheckList { get; set; }

        public DbSet<CheckListItem> CheckListItem { get; set; }

        public DbSet<WorkCheckList> WorkCheckList { get; set; }

        public DbSet<WorkCheckListItem> WorkCheckListItem { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(_options?.GetExtension<SqliteOptionsExtension>() == null)
            {
                optionsBuilder.UseSqlite("Data Source=ChecklistBot.db");
            }
            base.OnConfiguring(optionsBuilder);
        }
    }
}
