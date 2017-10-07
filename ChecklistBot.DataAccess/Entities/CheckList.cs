using System;
using System.Collections.Generic;
using System.Text;

namespace ChecklistBot.DataAccess.Entities
{
    public class CheckList
    {
        public CheckList(int userId, string name)
        {
            UserId = userId;
            Name = name;
            CheckListItems = new List<CheckListItem>();
        }
        public CheckList()
        {
            CheckListItems = new List<CheckListItem>();
        }

        public bool CreationCompleted { get; set; }

        public int Id { get; internal set; }

        public int UserId { get; set; }

        public string Name { get; set; }

        public virtual List<CheckListItem> CheckListItems { get; set; }
   
    }
}
