namespace CPVoiceAssistant.Services;

public interface IConvertorService
{
    Task ConvertTextToSpeechAndSend(long chatId, string text);
}