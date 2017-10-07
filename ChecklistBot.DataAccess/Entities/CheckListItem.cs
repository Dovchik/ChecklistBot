using System;
using System.Collections.Generic;
using System.Text;

namespace ChecklistBot.DataAccess.Entities
{
    public class CheckListItem
    {
        public int Id { get; internal set; }

        public CheckListItem()
        {

        }
        public CheckListItem(int checkListId, string name)
        {
            CheckListId = checkListId;
            Name = name;
        }
        public string Name { get; set; }

        public int CheckListId { get; set; }

        public virtual CheckList CheckList { get; set; }
    }
}
