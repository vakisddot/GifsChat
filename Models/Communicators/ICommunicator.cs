using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GifsChat.Models.Communicators;

public interface ICommunicator
{
    Task<HttpResponseMessage> GetResponse(string query);
    Type ResultType { get; }
}