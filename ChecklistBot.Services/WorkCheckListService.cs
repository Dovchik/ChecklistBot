using ChecklistBot.DataAccess;
using ChecklistBot.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChecklistBot.Services
{
    public class WorkCheckListService
    {
        private readonly ChecklistBotContext _context;

        public WorkCheckListService(ChecklistBotContext context)
        {
            _context = context;
        }

        public bool ChangeWorkListItemCompletionStatus(WorkCheckListItem item)
        {
            item.IsCompleted = !item.IsCompleted;
            _context.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return item.IsCompleted;
        }
        public List<WorkCheckListItem> InitWorkItems(CheckList list)
        {
            return list.CheckListItems.Select(item => new WorkCheckListItem()
            {
                CheckListItem = item,
                IsCompleted = false
            }).ToList();
        }

        public WorkCheckList UpdateWorkCheckList(WorkCheckList workCheckList, int workListItemId)
        {
            var item = workCheckList.WorkCheckListItems.FirstOrDefault(x => x.Id == workListItemId);
            ChangeWorkListItemCompletionStatus(item);
            return workCheckList;
        }

        public WorkCheckList GetWorkCheckList(int messageId)
        {
            return _context.WorkCheckList.FirstOrDefault(x => x.MessageId == messageId);
        }

        public WorkCheckList GetWorkCheckList(string inlineMessageId)
        {
            return _context.WorkCheckList.Include("WorkCheckListItems.CheckListItem").FirstOrDefault(x => x.InlineMessageId == inlineMessageId);
        }

        public void SetWorkCheckListMessageId(WorkCheckList list, int messageId)
        {
            list.MessageId = messageId;
            _context.Entry(list).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
        }
        public WorkCheckList ChooseWorkCheckList(CheckList checklist)
        {
            var existed = _context.WorkCheckList.Include("WorkCheckListItems.CheckListItem").LastOrDefault(x => x.CheckListId == checklist.Id && x.MessageId == 0 || x.InlineMessageId == null);
            return existed ?? CreateWorkCheckList(checklist);
        }
        public WorkCheckList CreateWorkCheckList(CheckList checklist)
        {
            var workChecklist = new WorkCheckList()
            {
                CheckList = checklist,
                WorkCheckListItems = InitWorkItems(checklist)
            };
            _context.WorkCheckList.Add(workChecklist);
            _context.SaveChanges();
            return workChecklist;
        }

        public WorkCheckList CreateWorkCheckList(CheckList checklist, int messageId)
        {
            var workChecklist = new WorkCheckList()
            {
                MessageId = messageId,
                CheckList = checklist,
                WorkCheckListItems = InitWorkItems(checklist)
            };
            _context.WorkCheckList.Add(workChecklist);
            _context.SaveChanges();
            return workChecklist;
        }

        public void BindWorkCheckListToInlineMessage(int workListId, string inlineMessageId)
        {
            var workCheckList = _context.WorkCheckList.LastOrDefault(x => x.Id == workListId);
            workCheckList.InlineMessageId = inlineMessageId;
            _context.Entry(workCheckList).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
