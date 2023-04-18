using System.Net.Http;
using System.Threading.Tasks;

namespace GifsChat.Models.Communicators;

public interface ICommunicator
{
    /// <summary>
    /// Queries the search engine
    /// </summary>
    /// <returns>URL if response was OK, null otherwise</returns>
    Task<string> QueryGifUrl(string query);
}