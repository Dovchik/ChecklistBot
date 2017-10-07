using ChecklistBot.DataAccess.Entities;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace ChecklistBot.Services
{
    public class BotMessageManager
    {
        private readonly WorkCheckListService _workCheckListService;
        private readonly TelegramBotClient _bot;

        public BotMessageManager(TelegramBotClient bot, WorkCheckListService workCheckListService)
        {
            _bot = bot;
            _workCheckListService = workCheckListService;
        }

        public void SendChecklist(long chatId, CheckList checklist)
        {
            WorkCheckList workCheckList;
            InlineKeyboardMarkup btn = BuildWorkListWithMarkup(checklist, out workCheckList);

            var msg = _bot.SendTextMessageAsync(chatId,
              checklist.Name, replyMarkup: btn);
            _workCheckListService.SetWorkCheckListMessageId(workCheckList, msg.Result.MessageId);
        }

        public InlineKeyboardMarkup BuildWorkListWithMarkup(CheckList checklist, out WorkCheckList workCheckList)
        {
            workCheckList = _workCheckListService.CreateWorkCheckList(checklist);
            return ResponseMessageHelper.BuildMarkup(workCheckList);
        }
    }
}
