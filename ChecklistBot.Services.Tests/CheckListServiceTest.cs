using ChecklistBot.DataAccess;
using ChecklistBot.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ChecklistBot.Services.Tests
{
    public class CheckListServiceTest
    {
        private readonly ChecklistBotContext _context;
        private readonly CheckListService _service;
        public CheckListServiceTest()
        {
            var builder = new DbContextOptionsBuilder<ChecklistBotContext>();
            var b = InMemoryDbContextOptionsExtensions.UseInMemoryDatabase(builder, "Data Source=:memory:");
            _context = new ChecklistBotContext(b.Options);
            _service = new CheckListService(_context);

        }
        [Fact]
        public void CreateCheckListTest()
        {
            var listName = "DaD";
            _service.CreateCheckList(7453, listName);
            var list = _context.CheckList.FirstOrDefault(x => x.Name == listName);
            Assert.NotNull(list);
            Assert.Equal(listName, list.Name);
        }
        [Fact]
        public void GetUserCheckListsTest()
        {
            var lists = new List<CheckList>()
            {
                _service.CreateCheckList(1, "1"),
                _service.CreateCheckList(2, "1"),
                _service.CreateCheckList(1, "2")
            };
            IEnumerable<CheckList> userCheckLists = _service.GetCompletedUserCheckLists(1);
            Assert.Equal(2, userCheckLists.Count());
        }
        [Fact]
        public void GetCheckListTest()
        {
            var list = _service.CreateCheckList(3, "3");
            var existed = _service.GetCheckList(list.Id);
            Assert.NotNull(existed);
            Assert.Equal(list.Id, existed.Id);
        }
        [Fact]
        public void RemoveCheckListTest()
        {
            var listtoRemove = _service.CreateCheckList(6, "6");
            _service.RemoveCheckList(listtoRemove.Id);
            var list = _service.GetCheckList(listtoRemove.Id);
            Assert.Null(list);
        }
    }
}
