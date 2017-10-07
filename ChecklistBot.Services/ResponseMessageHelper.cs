using ChecklistBot.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;

namespace ChecklistBot.Services
{
    public static class ResponseMessageHelper
    {
        public static InlineKeyboardMarkup BuildMarkup(WorkCheckList checklist)
        {
            var btn = new InlineKeyboardMarkup();
            btn.InlineKeyboard = new InlineKeyboardButton[checklist.WorkCheckListItems.Count][];
            for (int i = 0; i < checklist.WorkCheckListItems.Count; i++)
            {
                var item = checklist.WorkCheckListItems[i];
                var text = item.IsCompleted ? $"\u2705 {item.CheckListItem.Name}" : item.CheckListItem.Name;
                btn.InlineKeyboard[i] = new InlineKeyboardButton[]
                {
                    new InlineKeyboardCallbackButton(text, item.Id.ToString() + ":" + item.CheckListItem.Name)
                };
            }

            return btn;
        }

        public static InlineKeyboardMarkup BuildCreationCompleteMarkup(string checklistName)
        {
            var btn = new InlineKeyboardMarkup
            {
                InlineKeyboard = new InlineKeyboardButton[1][]
            };
            btn.InlineKeyboard[0] = new InlineKeyboardButton[]
            {
                    new InlineKeyboardSwitchInlineQueryButton($"Send your checklist", checklistName)
            };

            return btn;
        }
    }
}
