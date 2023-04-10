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

    [Header("API")]
    [Label("Tenor API Key")]
    [Tooltip($"(Suggested) Use '/gif apiKey' to get your own instead of using the default one")]
    [DefaultValue("AIzaSyCDP8gN9eZCsFRrVC7vs0BvvFl6THnTm10")]
    public string TenorApiKey;

    [Label("Results Limit")]
    [Tooltip("Determines how many results Tenor will send you when requesting a GIF")]
    [DefaultValue(10)]
    [Range(1, 20)]
    public int ResultsLimit;

    [Header("GIF Appearance")]
    [Label("Scale")]
    [Tooltip("Sets the scale of all newly sent GIFs in chat")]
    [DefaultValue(150)]
    [Range(20, 200)]
    public int WidthInChat;

    [Label("Opacity")]
    [Tooltip("Sets the opacity of all GIFs in chat")]
    [DefaultValue(0.75f)]
    [Range(0, 1)]
    public float Opacity;

    [Header("Performance")]
    [Label("Skip Every Second Frame")]
    public bool SkipEverySecondFrame;
}