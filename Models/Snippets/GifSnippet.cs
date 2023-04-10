using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System.Diagnostics;
using Terraria;
using Terraria.UI.Chat;

namespace GifsChat.Models.Snippets;

public class GifSnippet : TextSnippet, IImageSnippet
{
    private const short GifDelay = 20;

    private short _framesUntilFrameUpdate;
    private Stopwatch _deathWatch;
    private short _frameCounter;
    private byte _currentFrameIndex;

    private Texture2D[] _frames;

    public GifSnippet(Texture2D[] frames, bool halfDelay)
    {
        _deathWatch = Stopwatch.StartNew();
        _frames = frames;
        Scale = 1f;

        _framesUntilFrameUpdate = (short)(GifDelay * (halfDelay ? 1 : 2));

        int width = _frames[0].Width;
        int height = _frames[0].Height;

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
    public override void Update()
    {
        if (_deathWatch == null || _deathWatch.ElapsedMilliseconds >= GifsChatMod.ServerConfig.GifLifetime * 1000)
        {
            _frames = null;
            _deathWatch = null;
            return;
        }

        if (_frameCounter >= _framesUntilFrameUpdate)
        {
            NextFrame();

            _frameCounter = 0;
        }

        _frameCounter++;
    }
    private void NextFrame()
    {
        if (_currentFrameIndex + 1 == _frames.Length)
        {
            _currentFrameIndex = 0;
        }
        else
        {
            _currentFrameIndex++;
        }
    }

    public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = default, Color color = default, float scale = 1f)
    {
        if (_frames == null)
        {
            size = Vector2.One;
            return true;
        }

        if (!justCheckingString && color != Color.Black)
        {
            spriteBatch.Draw(_frames[_currentFrameIndex], position, null, Color.White * GifsChatMod.ClientConfig.Opacity, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        size = _frames[0].Size() * scale; // 这里拿来作间隔的，GetStringLength不知道拿来干啥的反正绘制没用
        return true;
    }

    public override float GetStringLength(DynamicSpriteFont font)
        => _frames != null ? _frames[0].Width * Scale : 0;

    public int GetChatYOffset()
        => _frames != null ? (int)(_frames[0].Height * Scale) : 0;
}