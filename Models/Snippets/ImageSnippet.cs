using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.UI.Chat;

namespace GifsChat.Models.Snippets;

public class ImageSnippet : TextSnippet, IImageSnippet
{
    public Texture2D Texture;

    public ImageSnippet(Texture2D texture)
    {
        Texture = texture;
        Scale = 1f;

        int width = Texture.Width;
        int height = Texture.Height;

        int snippetWidth = GifsChatMod.ClientConfig.WidthInChat;
        if (width > snippetWidth || height > snippetWidth)
        {
            if (width > height)
            {
                Scale = (float)snippetWidth / width;
            }
            else
            {
                Scale = (float)snippetWidth / height;
            }
        }
    }

    public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = default, Color color = default, float scale = 1f)
    {
        if (!justCheckingString && color != Color.Black)
        {
            spriteBatch.Draw(Texture, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        size = Texture.Size() * scale; // 这里拿来作间隔的，GetStringLength不知道拿来干啥的反正绘制没用
        return true;
    }

    public override float GetStringLength(DynamicSpriteFont font)
        => Texture.Width * Scale;

    public int GetChatYOffset()
        => (int)(Texture.Height * Scale);
}