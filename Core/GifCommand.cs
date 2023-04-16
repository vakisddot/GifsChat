using Terraria.ModLoader;
using GifsChat.Utils;
using System.Diagnostics;
using GifsChat.Models.Communicators;
using Terraria.ID;
using Terraria;
using System.Linq;
using System;

namespace GifsChat.Core;
public class GifCommand : ModCommand
{
    private Stopwatch _timeSinceLastCommand = Stopwatch.StartNew();
    public override string Command
        => "gif";
    public override CommandType Type
        => CommandType.Chat;
    public override string Description 
        => $"Sends a Gif in chat {Environment.NewLine}";
    public override string Usage =>
        $"{Environment.NewLine}" +
        $" - \"/gif john xina\" will send a random gif of John Xina {Environment.NewLine}" +
        $" - \"/gif https://.../john-xina.gif\" will send a specific gif of John Xina {Environment.NewLine}" +
        $" - \"/gif api\" will send you to a site where you can get your own API key {Environment.NewLine}" +
        $"{Environment.NewLine}";

    public async override void Action(CommandCaller caller, string input, string[] args)
    {
        if (args == null || !SanitizeInput(string.Join(' ', args)))
        {
            ModUtils.NewText("Invalid input!");
            return;
        }

        if (args[0].ToLower() == "api")
        {
            ModUtils.RerouteToApiSite();
            return;
        }
        
        int msBetweenCommands = GifsChatMod.ServerConfig.GifSendDelay * 1000;
        if (_timeSinceLastCommand.ElapsedMilliseconds < msBetweenCommands)
        {
            int timeRemaining = (msBetweenCommands - (int)_timeSinceLastCommand.ElapsedMilliseconds) / 1000;
            ModUtils.NewText($"Must wait at least {timeRemaining} {(timeRemaining == 1 ? "second" : "seconds")} until next Gif!");
            return;
        }

        if (!GifsChatMod.ClientConfig.GifsEnabled || !GifsChatMod.ServerConfig.GifsEnabled)
        {
            ModUtils.NewText("Gifs are currently disabled!");
            return;
        }


        try
        {
            string gifUrl = string.Empty;

            if (args[0].IsUrl())
            {
                if (args[0].IsValidGifUrl())
                {
                    if (GifsChatMod.ServerConfig.AllowGifsByUrl)
                        gifUrl = args[0];
                    else
                        ModUtils.NewText("Server does not allow Gifs to be sent by URL!");
                }
                else
                {
                    ModUtils.NewText("Invalid Gif URL! Note: URL must end with \".gif\" or \".webp\"");
                }
            }
            else
            {
                string query = string.Join(' ', args);

                ICommunicator communicator = new TenorCommunicator();

                gifUrl = await communicator.QueryGifUrl(query);
            }

            if (!string.IsNullOrWhiteSpace(gifUrl))
            {
                SendGif(gifUrl);
            }
        }
        catch (Exception e)
        {
            ModUtils.NewText(e.Message, true);
        }
        finally
        {
            _timeSinceLastCommand.Restart();
        }
    }

    /// <summary>
    /// Sends the Gif to all players
    /// </summary>
    private void SendGif(string gifUrl)
    {
        ModUtils.ExtractAndSendGif(gifUrl, Main.LocalPlayer.name);

        if (Main.netMode is NetmodeID.MultiplayerClient)
        {
            NetHandler.SendGifURLPacket(gifUrl, Main.LocalPlayer.name);
        }
    }

    private bool SanitizeInput(string args)
        => !(string.IsNullOrWhiteSpace(args)
        || !args.Any(char.IsLetter)
        || args.Any(c => c == '\'')
        || args.Any(c => c == '#')
        || args.Any(c => c == '"')
        || args.Any(c => c == '\\')
        );
}