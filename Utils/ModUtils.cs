using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Terraria;
using Microsoft.Xna.Framework;

namespace GifsChat.Utils;

public static class ModUtils
{
    public static async Task<T> DeserializeResults<T>(HttpResponseMessage response)
    {
        string json = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(json))
        {
            ModUtils.NewText("Failed to deserialize response!", true);
            return default;
        }

        return JsonConvert.DeserializeObject<T>(json);
    }

    /// <summary>
    /// Sends a message to the chat
    /// </summary>
    public static void NewText(string message, bool isException = false) 
        => Main.NewText($"[GifsChat] {message}", isException ? Color.DarkOrange : Color.Yellow);

    public static void RerouteToApiSite()
        => RerouteToSite("https://developers.google.com/tenor/guides/quickstart");

    public static void RerouteToSite(string url)
        => Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
}
