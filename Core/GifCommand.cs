using Terraria.ModLoader;
using GifsChat.Utils;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using GifsChat.Utils.Exceptions;
using GifsChat.Models.Communicators;
using Terraria.ID;
using Terraria;
using System.Linq;

namespace GifsChat.Core;
public class GifCommand : ModCommand
{
    private Stopwatch _timeSinceLastCommand = Stopwatch.StartNew();
    public override string Command
        => "gif";
    public override CommandType Type
        => CommandType.Chat;
    public override string Description
        => "Send a GIF in chat ('/gif apiKey' will send you to a site where you can get your own key)";

    public async override void Action(CommandCaller caller, string input, string[] args)
    {
        if (!GifsChatMod.ClientConfig.GifsEnabled || !GifsChatMod.ServerConfig.GifsEnabled)
        {
            Main.NewText("[GIFsChat] GIFs are disabled!", Color.Orange);
            return;
        }

        if (args == null || !SanitizeInput(string.Join(' ', args)))
        {
            Main.NewText("[GIFsChat] Invalid input!", Color.Orange);
            return;
        }

        if (args[0] == "apiKey")
        {
            ModUtils.RerouteToApiSite();
            return;
        }

        int msBetweenCommands = GifsChatMod.ServerConfig.GifSendDelay * 1000;

        if (_timeSinceLastCommand.ElapsedMilliseconds < msBetweenCommands)
        {
            int timeRemaining = (msBetweenCommands - (int)_timeSinceLastCommand.ElapsedMilliseconds) / 1000;
            caller.Reply($"Must wait at least {timeRemaining} {(timeRemaining == 1 ? "second" : "seconds")} until next GIF!", Color.Yellow);
            return;
        }

        try
        {
            string query = string.Join(' ', args);

            ICommunicator communicator = new TenorCommunicator();

            string gifUrl = await communicator.QueryGifUrl(query);

            if (string.IsNullOrWhiteSpace(gifUrl))
            {
                Main.NewText("[GIFsChat] Failed to get valid URL from Tenor!", Color.Orange);
                return;
            }

            communicator.ExtractAndSendGif(gifUrl, Main.LocalPlayer.name);

            if (Main.netMode is NetmodeID.MultiplayerClient)
            {
                var packet = Mod.GetPacket();

                packet.Write((byte)2);
                packet.Write(gifUrl);
                packet.Write(Main.LocalPlayer.name);

                packet.Send();
            }

            _timeSinceLastCommand.Restart();
        }
        catch (GifsChatException e)
        {
            caller.Reply(e.Message, Color.Orange);
        }
        catch { }
    }

    private bool SanitizeInput(string args)
        => !(string.IsNullOrWhiteSpace(args)
        || !args.Any(char.IsLetter)
        || args.Any(c => c == '\'')
        || args.Any(c => c == '#')
        || args.Any(c => c == '"')
        || args.Any(c => c == '\\')
        || args.Any(c => c == '/')
        );
}