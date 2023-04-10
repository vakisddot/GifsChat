using System;

namespace GifsChat.Utils.Exceptions;

public class GifsChatException : Exception
{
    public GifsChatException(string message)
        : base($"[GIFsChat] {message}") { }
}