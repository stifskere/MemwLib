using System.Text.RegularExpressions;
using MemwLib.Http;
using MemwLib.Http.Types;
using MemwLib.Http.Types.Configuration;
using MemwLib.Http.Types.Content.Implementations;
using MemwLib.Http.Types.Entities;
using MemwLib.Http.Types.Logging;
using MemwLib.Http.Types.SSL;
using NUnit.Framework;

namespace Tests;

public class Tests
{
    private readonly HttpServer _server = new(new HttpServerConfig
    {
        Port = 8080,
        SslBehavior = SslBehavior.DoNotUseCertificateIfNotFound
    });
    
    [SetUp]
    public void Setup()
    {
        _server.OnLog += message =>
        {
            Console.WriteLine(message);
            Assert.That(message.Type, Is.Not.EqualTo(LogType.Error));
        };

        _server.AddEndpoint(RequestMethodType.Get, new Regex("/users/(?'user'[^/]+)"), request =>
        {
            string user = request.CapturedGroups["user"];
            
            Assert.Fail(user, Is.EqualTo("john"));
            
            return new ResponseEntity(ResponseCodes.Accepted, new RawBody("hello"));
        });

        _server.AddEndpoint(RequestMethodType.Post, "/users", request =>
        {
            Assert.That(request.Body.RawBody, Is.EqualTo("hello"));

            return new ResponseEntity(ResponseCodes.Created, new RawBody(string.Empty));
        });
    }
    
    [Test]
    public async Task GetUserInfo()
    {
        ResponseEntity response = await HttpRequests.CreateRequest(new HttpRequestConfig
        {
            Url = "http://localhost:8080/users/john",
            Method = RequestMethodType.Get
        });

        Console.WriteLine(response.Body.RawBody);
        
        Assert.Multiple(() =>
        {
            Assert.That(response.ResponseCode, Is.EqualTo(ResponseCodes.Ok));
            Assert.That(response.Body.RawBody, Is.EqualTo("hello"));
        });
    }

    [Test]
    public void NewUser()
    {
        
    }
}