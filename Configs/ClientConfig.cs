using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace GifsChat.Configs;

public class ClientConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ClientSide;

    public override void OnLoaded()
    {
        GifsChatMod.ClientConfig = this;
    }

    [Header("GIFs in Chat")]
    [Label("Enable Gifs (Client-side)")]
    [DefaultValue(true)]
    public bool GifsEnabled;

    [Header("API")]
    [Label("Tenor API Key")]
    [Tooltip($"(Suggested) Use \"/gif api\" to get your own instead of using the default one")]
    [DefaultValue("AIzaSyCDP8gN9eZCsFRrVC7vs0BvvFl6THnTm10")]
    public string TenorApiKey;

    [Label("Results Limit (1-20)")]
    [Tooltip(
        "Determines how many results Tenor will send you when requesting a Gif\r\n" +
        "One of N results will be randomly picked, so if this is set to 1, you will always send the same Gif for the same query")]
    [DefaultValue(10)]
    [Range(1, 20)]
    public int ResultsLimit;

    [Header("Gif Appearance")]
    [Label("Scale (20-200)")]
    [Tooltip("Sets the scale of all newly sent Gifs in chat")]
    [DefaultValue(150)]
    [Range(20, 200)]
    public int WidthInChat;

    [Label("Opacity (0-1)")]
    [Tooltip("Sets the opacity of all Gifs in chat")]
    [DefaultValue(0.75f)]
    [Range(0, 1)]
    public float Opacity;

    [Header("Performance")]
    [Label("Skip Every 2nd Frame")]
    [Tooltip(
        "This will skip every second frame of a Gif, resulting in better performance, but worse animation\r\n" +
        "Recommended for lower-end machines")]
    public bool SkipEverySecondFrame;

    [Label("Frame Limit (5-150)")]
    [Tooltip(
        "Determines the max amount of frames that will be extracted from a Gif\r\n" +
        "Lower values recommended for lower-end machines")]
    [DefaultValue(50)]
    [Range(5, 150)]
    public int FramesLimit;
}