using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace FeedbackSystem;

public class BaseConfigs : BasePluginConfig
{
    [JsonPropertyName("WebhookUrl")]
    public string WebhookUrl { get; set; } = "";

    [JsonPropertyName("UseHostname")]
    public bool UseHostname { get; set; } = true;

    [JsonPropertyName("CommandCooldownSeconds")]
    public int CommandCooldownSeconds { get; set; } = 120;

    [JsonPropertyName("Command")]
    public List<string> Command { get; set; } = new List<string> { "css_feedback", ".feedback" };

    [JsonPropertyName("EmbedImage")]
    public bool EmbedImage { get; set; } = true;

    [JsonPropertyName("EmbedColor")]
    public string EmbedColor { get; set; } = "#ffb800";

    [JsonPropertyName("EmbedFooter")]
    public bool EmbedFooter { get; set; } = true;

    [JsonPropertyName("EmbedFooterImage")]
    public string EmbedFooterImage { get; set; } = "https://avatars.githubusercontent.com/u/61034981?v=4";

    [JsonPropertyName("EmbedAuthor")]
    public bool EmbedAuthor { get; set; } = true;

    [JsonPropertyName("EmbedAuthorURL")]
    public string EmbedAuthorURL { get; set; } = "https://lucauy.dev";

    [JsonPropertyName("EmbedAuthorImage")]
    public string EmbedAuthorImage { get; set; } = "https://avatars.githubusercontent.com/u/61034981?v=4";

}