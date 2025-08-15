using CPVoiceAssistant.Services;

namespace CPVoiceAssistant;

public class Application
{
    private readonly ITelegramService _telegramService;

    public Application(ITelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    public async Task RunAsync()
    {
        await _telegramService.StartAsync();
    }
}