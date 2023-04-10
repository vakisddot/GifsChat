using Terraria.ModLoader;
using GifsChat.Utils;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using GifsChat.Utils.Exceptions;
using GifsChat.Models.Communicators;
using Terraria.ID;
using Terraria;

namespace GifsChat.Core;
public class GifCommand : ModCommand
{
    private Stopwatch _timeSinceLastCommand = Stopwatch.StartNew();
    public override string Command
        => "gif";
    public override CommandType Type
        => CommandType.Chat;
    public override string Description
        => "Send a GIF in chat ('/gif apiKey' will reroute you to the site to get your own)";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (args == null)
            return;

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
            communicator.HandleQuery(query);

            if (Main.netMode is NetmodeID.MultiplayerClient)
            {
                var packet = Mod.GetPacket();

                packet.Write((byte)2);
                packet.Write(query);

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
}