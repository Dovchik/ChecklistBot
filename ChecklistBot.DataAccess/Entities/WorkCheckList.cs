using System;
using System.Collections.Generic;
using System.Text;

namespace ChecklistBot.DataAccess.Entities
{
   public class WorkCheckList
    {
        public WorkCheckList()
        {
            WorkCheckListItems = new List<WorkCheckListItem>();
        }
        public int Id { get; set; }

        public int MessageId { get; set; }

        public string InlineMessageId { get; set; }

        public int CheckListId { get; set; }

        public virtual CheckList CheckList { get; set; }

        public virtual List<WorkCheckListItem> WorkCheckListItems { get; set; }
    }
}
