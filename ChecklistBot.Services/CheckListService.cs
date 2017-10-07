using ChecklistBot.DataAccess;
using ChecklistBot.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChecklistBot.Services
{
    public class CheckListService
    {
        private readonly ChecklistBotContext _context;
        public CheckListService(ChecklistBotContext context)
        {
            _context = context;
        } 
        public CheckList CreateCheckList(int userId, string listName)
        {
            var list = new CheckList(userId, listName);
            _context.CheckList.Add(list);
            _context.SaveChanges();
            return list;
        }

        public IEnumerable<CheckList> GetQueriedLists(int userId , string query)
        {
            var completed = GetCompletedUserCheckLists(userId);
            if (!string.IsNullOrEmpty(query))
            {
                var invariatnQuery = query.ToUpper() ;
               return completed.Where(x => x.Name.ToUpper().Contains(invariatnQuery)).Take(10);
            }
            return completed.Reverse().Take(10);
        }

        public void AppendCheckListItem(int checkListId, string itemName)
        {
            var checkListItem = new CheckListItem(checkListId, itemName);
            _context.CheckListItem.Add(checkListItem);
            _context.SaveChanges();
        }

        public CheckList GetCheckList(int id)
        {
           return _context.CheckList.FirstOrDefault(x => x.Id == id);
        }

        public void RemoveCheckList(int v)
        {
            _context.CheckList.Remove(GetCheckList(v));
            _context.SaveChanges();
        }

        public IEnumerable<CheckList> GetCompletedUserCheckLists(int userId)
        {
            return _context.CheckList.Include(x => x.CheckListItems).Where(x => x.UserId == userId&&x.CreationCompleted);
        }
        public CheckList GetUserInProcessChecklist(int userId)
        {
            return _context.CheckList.Include(x=>x.CheckListItems).LastOrDefault(x => x.UserId == userId && !x.CreationCompleted);
        }
        public void RemoveUnfinished(int userId)
        {
            var lists = _context.CheckList.Where(x => x.UserId == userId&&!x.CreationCompleted);
            _context.RemoveRange(lists);
            _context.SaveChanges();
        }

        public void CompleteCheckList(CheckList checklist)
        {
            checklist.CreationCompleted = true;
            _context.Entry(checklist).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChangesAsync();
        }
    }
}
