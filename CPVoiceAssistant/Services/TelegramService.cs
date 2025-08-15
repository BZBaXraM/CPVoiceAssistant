using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CPVoiceAssistant.Services;

public class TelegramService : ITelegramService, IConvertorService, IAsyncDisposable
{
    private const string Key = "8496460014:AAENyxCthDcE7NApOXIwoWoCcf0_yUyt4dY";
    private static TelegramBotClient? _bot;

    public TelegramService()
    {
        _bot = new TelegramBotClient(Key);
        _bot.OnMessage += OnMessage;
    }

    public async Task StartAsync()
    {
        if (_bot != null)
        {
            var me = await _bot.GetMe();
            Console.WriteLine($"{me.Username} is running... Press Enter to terminate");
        }

        Console.ReadLine();
    }

    public async Task ConvertTextToSpeechAndSend(long chatId, string text)
    {
        var tempAiff = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.aiff");
        var tempOgg = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.ogg");

        try
        {
            ProcessStartInfo sayProcess = new()
            {
                FileName = "say",
                Arguments = $"-o \"{tempAiff}\" \"{text}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using (var process = Process.Start(sayProcess))
            {
                if (process != null)
                    await process.WaitForExitAsync();
            }

            ProcessStartInfo ffmpegProcess = new()
            {
                FileName = "ffmpeg",
                Arguments = $"-i \"{tempAiff}\" -c:a libvorbis \"{tempOgg}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using (var process = Process.Start(ffmpegProcess))
            {
                if (process != null)
                    await process.WaitForExitAsync();
            }

            await using var stream = File.OpenRead(tempOgg);

            if (_bot != null)
            {
                await _bot.SendVoice(chatId, new InputFileStream(stream));
            }
        }
        finally
        {
            if (File.Exists(tempAiff)) File.Delete(tempAiff);
            if (File.Exists(tempOgg)) File.Delete(tempOgg);
        }
    }

    public async Task OnMessage(Message msg, UpdateType type)
    {
        if (msg.Text is null) return;
        await ConvertTextToSpeechAndSend(msg.Chat.Id, msg.Text);
    }

    public async ValueTask DisposeAsync()
    {
        if (_bot != null)
        {
            await _bot.Close();
            _bot = null;
            _bot!.OnMessage -= OnMessage;
        }

        GC.SuppressFinalize(this);
    }
}