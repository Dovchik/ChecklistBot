using ChecklistBot.DataAccess;
using ChecklistBot.DataAccess.Entities;
using ChecklistBot.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ChecklistBot.Services
{
    public class ChecklistBotService
    {
        private readonly TelegramBotClient bot;
        private readonly CheckListService _checkListService;
        private readonly WorkCheckListService _workCheckListService;
        private readonly InlineQueryManager _inlineQueryManager;
        private readonly BotMessageManager _botMessageManager;


        public ChecklistBotService(BotConfig botToken, ChecklistBotContext botcontext)
        {
            var context = botcontext;
            bot = new TelegramBotClient(botToken.BotToken);

            _checkListService = new CheckListService(context);
            _workCheckListService = new WorkCheckListService(context);

            _botMessageManager = new BotMessageManager(bot, _workCheckListService);
            _inlineQueryManager = new InlineQueryManager(bot, _checkListService, _botMessageManager, _workCheckListService);
        }

        public static async void SetWebhook(string botToken, string webHookUrl)
        {
            var bot = new TelegramBotClient(botToken);
            var info = await bot.GetWebhookInfoAsync();
            if (webHookUrl.Equals(info.Url))
                return;
            await bot.SetWebhookAsync(webHookUrl);
        }

        public string GetStatus()
        {
            return bot.GetWebhookInfoAsync().Result.Url.ToString();
        }

        private Timer _timer;
        private int _offset = 0;

        public void StartBot()
        {
            Console.WriteLine("Bot started and will check updates every second");

            _timer = new Timer(StartBotInternal, null, Timeout.Infinite, Timeout.Infinite);

            _timer.Change(0, Timeout.Infinite);

            Console.WriteLine("Hit any key to shutdown");
            Console.ReadKey();
        }

        private void StartBotInternal(Object state)
        {
            List<Update> updates = bot.GetUpdatesAsync(_offset).Result.ToList();
            updates = SkipKnownMessages(ref _offset, updates);
            foreach (Update update in updates)
            {
                HandleUpdate(update);
                _offset++;
            }
            _timer.Change(1000, Timeout.Infinite);
        }

        public static void RemoveWebhook(string botToken)
        {
            new TelegramBotClient(botToken).DeleteWebhookAsync();
        }

        public void HandleUpdate(Update update)
        {
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.MessageUpdate)
            {
                var message = update.Message;
                if (message.Type == Telegram.Bot.Types.Enums.MessageType.TextMessage)
                {
                    HandleMessage(message);
                }
            }
            try
            {
                if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQueryUpdate)
                {
                    HandleCallbackQuery(update.CallbackQuery);
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.InlineQueryUpdate)
            {
                _inlineQueryManager.HandleInlineQuery(update.InlineQuery);
            }
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.ChosenInlineResultUpdate)
            {
                try
                {
                    _inlineQueryManager.HandleChosenInlineResult(update.ChosenInlineResult);
                }
                catch (Exception ex)
                {
                    Debug.Fail(ex.Message, ex.StackTrace);
                }
            }
        }

        private void HandleCallbackQuery(CallbackQuery clq)
        {
            var parsedQuery = clq.Data.Split(':');
            bot.AnswerCallbackQueryAsync(clq.Id, $"{parsedQuery[1]} selected");
            var workListItemId = int.Parse(parsedQuery[0]);
            if (clq.Message != null)
            {
                var workCheckList = _workCheckListService.GetWorkCheckList(clq.Message.MessageId);
                var changedWorkList = _workCheckListService.UpdateWorkCheckList(workCheckList, workListItemId);
                var success = bot.EditMessageReplyMarkupAsync(clq.Message.Chat.Id, clq.Message.MessageId, replyMarkup: ResponseMessageHelper.BuildMarkup(changedWorkList));
            }
            else
            {
                var workCheckList = _workCheckListService.GetWorkCheckList(clq.InlineMessageId);
                var changedWorkList = _workCheckListService.UpdateWorkCheckList(workCheckList, workListItemId);
                var success = bot.EditInlineMessageReplyMarkupAsync(clq.InlineMessageId, ResponseMessageHelper.BuildMarkup(changedWorkList));
            }
        }
        private void HandleMessage(Message message)
        {
            var chatId = message.Chat.Id;
            var userId = message.From.Id;
            var checklist = _checkListService.GetUserInProcessChecklist(userId);
            var text = message.Text;

            if ("/START".Equals(text, StringComparison.OrdinalIgnoreCase) ||
                "/START _".Equals(text, StringComparison.OrdinalIgnoreCase))
            {
                StartNewListCreation(message, userId);
                return;
            }

            if ("/ALL".Equals(text, StringComparison.OrdinalIgnoreCase))
            {
                ViewAllLists(chatId, userId);
                return;
            }

            if ("/DONE".Equals(text, StringComparison.OrdinalIgnoreCase))
            {
                CompleteListCreation(message, checklist);
                return;
            }

            if ("/LIST".Equals(text, StringComparison.OrdinalIgnoreCase))
            {
                ViewCurrentList(chatId, checklist);
                return;
            }

            if (checklist == null)
            {
                CreateCheckList(message, chatId, userId);
                return;
            }

            AppendCheckListItem(message, chatId, checklist);
        }

        private void AppendCheckListItem(Message message, long chatId, CheckList checklist)
        {
            var listIsEmpty = checklist.CheckListItems.Count == 0;
            _checkListService.AppendCheckListItem(checklist.Id, message.Text);
            if (listIsEmpty)
                bot.SendTextMessageAsync(chatId,
                   $"Added {message.Text}! Now add items how much you want or type /done to complete. Type /list to check our list");
            else
                bot.SendTextMessageAsync(chatId,
                   $"Added {message.Text}!");
        }

        private void CreateCheckList(Message message, long chatId, int userId)
        {
            _checkListService.CreateCheckList(userId, message.Text);
            bot.SendTextMessageAsync(chatId,
                $"Nice, created checklist {message.Text}! Now add first item to your list.");
        }

        private void ViewCurrentList(long chatId, CheckList checklist)
        {
            if (checklist == null)
                return;
            if (checklist.CheckListItems.Count < 1)
                return;
            _botMessageManager.SendChecklist(chatId, checklist);
            return;
        }

        private void CompleteListCreation(Message message, CheckList checklist)
        {
            if (checklist == null)
            {
                bot.SendTextMessageAsync(message.Chat.Id,
                    "There is no list yet. Type /start to begin checklist creation");
                return;
            }
            if (checklist.CheckListItems.Count < 1)
            {
                bot.SendTextMessageAsync(message.Chat.Id,
                    "I can't create empty list. Add at least one item.");
                return;
            }
            _checkListService.CompleteCheckList(checklist);
            bot.SendTextMessageAsync(message.Chat.Id,
                "Your list is created! Now send it.",
                replyMarkup: ResponseMessageHelper.BuildCreationCompleteMarkup(checklist.Name));
            return;
        }

        private void ViewAllLists(long chatId, int userId)
        {
            var userCheckLists = _checkListService.GetCompletedUserCheckLists(userId);
            if (!userCheckLists.Any())
            {
                bot.SendTextMessageAsync(chatId, $"You doesn't have any checklist. Type /start to create your first list.");
            }
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Your lists:");
                userCheckLists.ToList().ForEach(x =>
                {
                    sb.AppendLine($"- {x.Name}");
                });
                bot.SendTextMessageAsync(chatId, sb.ToString());
            }
        }

        private void StartNewListCreation(Message message, int userId)
        {
            _checkListService.RemoveUnfinished(userId);
            bot.SendTextMessageAsync(message.Chat.Id,
                "Let's create new checklist! First, name your list.");
        }

        private List<Update> SkipKnownMessages(ref int offset, List<Update> updates)
        {
            if (updates.Count > 0 && offset == 0)
            {
                offset = updates[0].Id;
                return updates.Skip(1).ToList();
            }
            return updates;
        }

        async void TestApiAsync()
        {
            var me = await bot.GetMeAsync();
            Console.WriteLine("Hello my name is " + me.FirstName);
        }
    }
}
