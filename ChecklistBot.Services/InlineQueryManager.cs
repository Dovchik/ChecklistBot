using ChecklistBot.DataAccess.Entities;
using ChecklistBot.Services;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot;

namespace ChecklistBot.Services
{
    public class InlineQueryManager
    {
        private int _userId;
        private readonly CheckListService _checkListService;
        private readonly WorkCheckListService _workCheckListService;
        private readonly TelegramBotClient _bot;
        private readonly BotMessageManager _botMessageManager;
        public InlineQueryManager(TelegramBotClient bot, CheckListService checkListService, BotMessageManager botMessageManager, WorkCheckListService workCheckListService)
        {
            _checkListService = checkListService;
            _bot = bot;
            _botMessageManager = botMessageManager;
            _workCheckListService = workCheckListService;
        }

        public void HandleInlineQuery(InlineQuery inlineQuery)
        {
            _userId =  inlineQuery.From.Id;
           var list = _checkListService.GetQueriedLists(_userId, inlineQuery.Query);
            var responseMarkup = BuildInlineQueryResponse(list);
            _bot.AnswerInlineQueryAsync(inlineQuery.Id, responseMarkup, switchPmText: "Create new checklist", switchPmParameter: "_", cacheTime: 0);
        }

        private InlineQueryResult[] BuildInlineQueryResponse(IEnumerable<CheckList> checkLists)
        {
            var checklist = new WorkCheckList();
            var resultedList = checkLists.Select(item =>
            {
                var workCheckList = _workCheckListService.ChooseWorkCheckList(item);
                return new InlineQueryResultArticle
                {
                    Id = item.Id.ToString() + ":" + workCheckList.Id,
                    Title = item.Name,
                    ReplyMarkup = ResponseMessageHelper.BuildMarkup(workCheckList),
                    InputMessageContent = new InputTextMessageContent()
                    {
                        MessageText = item.Name,
                    }
                };
            });
            return resultedList.ToArray();
        }
        
        private WorkCheckList WorkCheckList(CheckList i)
        {
            return new WorkCheckList()
            {
                CheckList = i,
                WorkCheckListItems = i.CheckListItems.Select(t => new WorkCheckListItem()).ToList()
            };
        }
        public void HandleChosenInlineResult(ChosenInlineResult chosenInlineResult)
        {
            var ids = chosenInlineResult.ResultId.Split(':');
            var list = _checkListService.GetCheckList(int.Parse(ids[0]));
            _workCheckListService.BindWorkCheckListToInlineMessage(int.Parse(ids[1]), chosenInlineResult.InlineMessageId);
                
        }
    }
}
