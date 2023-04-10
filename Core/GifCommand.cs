using Terraria.ModLoader;
using GifsChat.Utils;
using GifsChat.Models.Json;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using GifsChat.Utils.Exceptions;
using GifsChat.Models.Communicators;

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

    public override async void Action(CommandCaller caller, string input, string[] args)
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
            var response = await communicator.GetResponse(query);

            if (response == null)
                return;

            var results = await ModUtils.DeserializeResults<TenorResults>(response);
            var result = results.GetRandomResult();
            var gifUrl = result.GetFormat(FormatType.TinyGif).Url;

            var gifStream = await ModUtils.GetStreamFromUrl(gifUrl);
            var gifFramesStreams = await ModUtils.ExtractGifFrames(gifStream);

            GifsChatSystem.EnqueueGifFramesStreams(gifFramesStreams);

            _timeSinceLastCommand.Restart();
        }
        catch (GifsChatException e)
        {
            caller.Reply(e.Message, Color.Orange);
        }
        catch { }
    }
}