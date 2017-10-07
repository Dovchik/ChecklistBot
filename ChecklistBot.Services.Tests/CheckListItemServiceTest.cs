using ChecklistBot.DataAccess;
using ChecklistBot.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ChecklistBot.Services.Tests
{
    public class CheckListItemServiceTest
    {
        private readonly ChecklistBotContext _context;
        private readonly CheckListService _checklistService;
        public CheckListItemServiceTest()
        {
            _context = TestConfig.CreateTestContext();
            _checklistService = new CheckListService(_context);
        }
        [Fact]
        public void AppendCheckListItemTest()
        {
            var cl = _checklistService.CreateCheckList(142, "items");
            _checklistService.AppendCheckListItem(cl.Id, "Item1");
            _checklistService.AppendCheckListItem(cl.Id, "Item2");
            var newList = _checklistService.GetCheckList(cl.Id);
            Assert.Equal(2, newList.CheckListItems.Count);
        }
    }
}
