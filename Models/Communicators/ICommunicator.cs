using System.Net.Http;
using System.Threading.Tasks;

namespace GifsChat.Models.Communicators;

public interface ICommunicator
{
    Task<string> QueryGifUrl(string query);
    Task<HttpResponseMessage> GetResponse(string query);
    void ExtractAndSendGif(string gifUrl, string sentBy);
}