# FeedbackSystem CS2
Allows players to send feedback on the server as an embed message to Discord.

> This plugin is quite simple, I don't think it is necessary to make major changes, but if someone needs it to be improved, you can create an [issue](https://github.com/wiruwiru/FeedbackSystem-CS2/issues)



## Installation
1. Install [CounterStrike Sharp](https://github.com/roflmuffin/CounterStrikeSharp) and [Metamod:Source](https://www.sourcemm.net/downloads.php/?branch=master).

2. Download [FeedbackSystem.zip](https://github.com/wiruwiru/FeedbackSystem-CS2/releases) from the releases section.

3. Unzip the archive and upload it to the game server.

4. Start the server and wait for the configuration file to be generated.

5. Edit the configuration file with the parameters of your choice.

---

## Config
The configuration file will be automatically generated when the plugin is first loaded. Below are the parameters you can customize:

| Parameter | Description | Required     |
| :------- | :------- | :------- |
| `WebhookUrl` | You must create it in the channel where you will send the notices. |**YES** |
| `CommandCooldownSeconds` | Command cooldown time in seconds. | **YES** |
| `Command` | You can change the command to be used by the players or add extra commands. | **YES** |
| `EmbedColor` | You can change this to your favorite color, in Hex format. | **YES** |
| `EmbedFooter` | You can use this option to disable or enable the embed footer. | **YES** |
| `EmbedFooterImage` | It will be the image (logo) that will appear in the embed footer. | **YES** |
| `EmbedAuthor` | You can use this option to disable or enable the embed author. | **YES** |
| `EmbedAuthorURL` | This will be the url that will be redirected to when a user clicks on the embed author. | **YES** |
| `EmbedAuthorImage` | It will be the image (logo) that will appear as the author of the embed. | **YES** |

---

## Configuration Example
Here is an example configuration file:
```json
{
  "WebhookUrl": "",
  "CommandCooldownSeconds": 120,
  "Command": [
    "css_feedback",
    ".feedback"
  ],
  "EmbedColor": "#ffb800",
  "EmbedFooter": true,
  "EmbedFooterImage": "https://avatars.githubusercontent.com/u/61034981?v=4",
  "EmbedAuthor": true,
  "EmbedAuthorURL": "https://lucauy.dev",
  "EmbedAuthorImage": "https://avatars.githubusercontent.com/u/61034981?v=4"
}
  "ConfigVersion": 1
```