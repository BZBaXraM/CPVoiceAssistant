using CPVoiceAssistant;
using CPVoiceAssistant.Services;

var app = new Application(new TelegramService());
await app.RunAsync();