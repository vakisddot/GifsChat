using GifsChat.Models;
using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace GifsChat.Configs;

public class ServerConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    public override void OnLoaded()
    {
        GifsChatMod.ServerConfig = this;
    }

    [Header("GIFs in Chat")]
    [Label("Enable Gifs (Server-side)")]
    [DefaultValue(true)]
    public bool GifsEnabled;

    [Label("Allow Gifs By URL")]
    [Tooltip(
        $"Gifs sent by URL can be LARGE in size and contain UNFILTERED content!\r\n" +
        "Turning this on is NOT recommended, especially in servers where strangers can join!")]
    [DefaultValue(false)]
    public bool AllowGifsByUrl;

    [Header("Performance")]
    [Label("Gif Lifetime (15-120 sec.)")]
    [Tooltip("Dictates how long a Gif will stay in chat until it gets removed")]
    [DefaultValue(60)]
    [Range(15, 120)]
    public int GifLifetime;

    [Label("Delay Between Gifs (5-15 sec.)")]
    [Tooltip("Dictates the delay between Gifs for each player")]
    [DefaultValue(5)]
    [Range(5, 15)]
    public int GifSendDelay;

    [Header("Filter")]
    [Label("Content Filter")]
    [DefaultValue(0)]
    public ContentFilter ContentFilter;
}