using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CPVoiceAssistant.Services;

public interface ITelegramService
{
    Task StartAsync();
    Task OnMessage(Message msg, UpdateType type);
}