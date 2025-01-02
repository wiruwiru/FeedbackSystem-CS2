using System.Text;
using System.Text.Json;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Core.Attributes;

namespace FeedbackSystem;

[MinimumApiVersion(296)]
public class FeedbackSystemBase : BasePlugin, IPluginConfig<BaseConfigs>
{
    private Dictionary<string, DateTime> _playerCooldowns = new();

    public override string ModuleName => "FeedbackSystem";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "luca.uy";
    public override string ModuleDescription => "Allows players to send feedback on the server as an embed message to Discord.";

    public CounterStrikeSharp.API.Modules.Timers.Timer? intervalMessages;

    public override void Load(bool hotReload)
    {
        foreach (var command in Config.Command)
        {
            AddCommand(command, "Allows players to send feedback", ExecuteFeedbackCommand);
        }
    }

    [CommandHelper(minArgs: 1, usage: " [message]", whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public void ExecuteFeedbackCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null)
        {
            commandInfo.ReplyToCommand(Localizer["Prefix"] + " " + Localizer["NoPlayer"]);
            return;
        }

        var message = commandInfo.ArgString?.Trim() ?? string.Empty;
        message = message.Trim();

        if (string.IsNullOrWhiteSpace(message) || message.Replace(" ", "").Length < Config.MinMessageLength)
        {
            commandInfo.ReplyToCommand(Localizer["Prefix"] + " " + Localizer["InvalidMessage"]);
            return;
        }

        int secondsRemaining;
        var playerId = player.PlayerName;

        if (!CheckCommandCooldown(playerId, out secondsRemaining))
        {
            commandInfo.ReplyToCommand(Localizer["Prefix"] + " " + Localizer["CommandCooldownMessage", secondsRemaining]);
            return;
        }

        _playerCooldowns[playerId] = DateTime.Now;

        FeedbackCommand(player, player.PlayerName ?? Localizer["UnknownPlayer"], message);
        player.PrintToChat(Localizer["Prefix"] + " " + Localizer["FeedbackSendMessage"]);
    }

    private bool CheckCommandCooldown(string playerId, out int secondsRemaining)
    {
        if (_playerCooldowns.TryGetValue(playerId, out var lastCommandTime))
        {
            var secondsSinceLastCommand = (int)(DateTime.Now - lastCommandTime).TotalSeconds;
            secondsRemaining = Config.CommandCooldownSeconds - secondsSinceLastCommand;
            return secondsRemaining <= 0;
        }

        secondsRemaining = 0;
        return true;
    }

    private int ConvertHexToColor(string hex)
    {
        if (hex.StartsWith("#")) hex = hex[1..];
        return int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
    }

    public void FeedbackCommand(CCSPlayerController? caller, string clientName, string? message)
    {
        if (caller == null) return;

        clientName = clientName.Replace("[Ready]", "").Replace("[Not Ready]", "").Trim();

        var fields = new List<object>
    {
        new { name = Localizer["HostnameField"]?.ToString() ?? "Server", value = $"```{ConVar.Find("hostname")?.StringValue ?? "Unknown Hostname"}```", inline = false },
        new { name = Localizer["IPField"]?.ToString() ?? "IP", value = $"```{GetIP()}```", inline = true },
        new { name = Localizer["PlayerSendField"]?.ToString() ?? "Request", value = $"```{clientName}```", inline = true }
    };

        if (!string.IsNullOrEmpty(message))
        {
            if (message.Length > 1024) message = message.Substring(0, 1021) + "...";
            fields.Add(new { name = Localizer["FeedbackMessage"]?.ToString() ?? "Message", value = $"```{message}```", inline = false });
        }

        var embed = new
        {
            title = Localizer["EmbedTitle"]?.ToString() ?? "Feedback",
            description = Localizer["EmbedDescription"]?.ToString() ?? "No description provided.",
            color = ConvertHexToColor(Config.EmbedColor),
            fields,
            footer = Config.EmbedFooter ? new { text = Localizer["EmbedFooterText"]?.ToString() ?? "Powered by NeedSystem", icon_url = Config.EmbedFooterImage } : null,
            author = Config.EmbedAuthor ? new { name = Localizer["EmbedAuthorName"]?.ToString() ?? "NeedSystem", url = Config.EmbedAuthorURL, icon_url = Config.EmbedAuthorImage } : null
        };

        Task.Run(() => SendEmbedToDiscord(embed));
    }

    private async Task SendEmbedToDiscord(object embed)
    {
        try
        {
            var webhookUrl = GetWebhook();

            if (string.IsNullOrEmpty(webhookUrl))
            {
                return;
            }

            using var httpClient = new HttpClient();

            var payload = new { embeds = new[] { embed } };

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });

            var contentString = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(webhookUrl, contentString);

            if (!response.IsSuccessStatusCode)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[FeedbackSystem] Failed to send embed. HTTP Status: {response.StatusCode}");
            }
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[FeedbackSystem] Error: {e.Message}");
            Console.WriteLine(e.StackTrace);
        }
    }

    public required BaseConfigs Config { get; set; }
    public void OnConfigParsed(BaseConfigs config)
    {
        Config = config;
    }

    private string GetWebhook()
    {
        return Config.WebhookUrl;
    }

    private string GetIP()
    {
        string? ip = ConVar.Find("ip")?.StringValue;
        string? port = ConVar.Find("hostport")?.GetPrimitiveValue<int>().ToString();

        if (!string.IsNullOrEmpty(ip) && !string.IsNullOrEmpty(port))
        {
            return $"{ip}:{port}";
        }
        else
        {
            return Localizer["UnknownIP"];
        }
    }
}