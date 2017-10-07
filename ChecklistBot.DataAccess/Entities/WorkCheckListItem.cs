using System;
using System.Collections.Generic;
using System.Text;

namespace ChecklistBot.DataAccess.Entities
{
    public class WorkCheckListItem
    {
        public int Id { get; set; }

        public int WorkCheckListId { get; set; }

        public int CheckListItemId { get; set; }

        public virtual CheckListItem CheckListItem { get; set; }

        public virtual WorkCheckList WorkCheckList {get;set;}

        public bool IsCompleted { get; set; }
    }
}
