using System.Text;
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

[TestFixture]
public class Http : IDisposable
{
    private HttpServer _server = default!;
    
    [OneTimeSetUp]
    public void Setup()
    {
        _server = new(new HttpServerConfig
        {
            Port = 8080,
            SslBehavior = SslBehavior.DoNotUseCertificateIfNotFound
        });
        
        _server.OnLog += message =>
        {
            Assert.That(message.Type, Is.Not.EqualTo(LogType.Error));
        };

        _server.AddEndpoint(RequestMethodType.Get, new Regex("/users/(?'user'[^/]+)"), request =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(request.CapturedGroups, Has.Length.EqualTo(1));
                Assert.That(request.CapturedGroups.Contains("user"), Is.True);
                Assert.That(request.CapturedGroups["user"], Is.EqualTo("me"));
                Assert.That(request.Headers["Authorization"], Is.Not.Null);
            });

            string[] auth = Encoding.ASCII
                    .GetString(Convert.FromBase64String(request.Headers["Authorization"]!.Split(' ')[1]))
                    .Split(':');
            
            Assert.Multiple(() =>
            {
                Assert.That(auth[0], Is.EqualTo("john"));
                Assert.That(auth[1], Is.EqualTo("1234"));
            });
            
            return new ResponseEntity(ResponseCodes.Ok, new RawBody("hello"));
        });

        _server.AddEndpoint(RequestMethodType.Put, "/users", request => {
            UrlEncodedBody? body = request.Body.ReadAs<UrlEncodedBody>();
            
            Assert.Multiple(() =>
            {
                Assert.That(request.Path.Parameters, Has.Length.EqualTo(1));
                Assert.That(request.Path.Parameters.Contains("admin"), Is.True);
                Assert.That(request.Path.Parameters["admin"], Is.EqualTo("true"));
                
                Assert.That(body, Is.Not.Null);
                Assert.That(body!["name"], Is.EqualTo("john"));
                Assert.That(body["pass"], Is.EqualTo("1234"));
            });
            
            return new ResponseEntity(ResponseCodes.Created);
        });
    }
    
    [Test]
    public async Task GetUserInfo()
    {
        ResponseEntity response = await HttpRequests.CreateRequest(new HttpRequestConfig
        {
            Url = "http://john:1234@localhost:8080/users/me",
            Method = RequestMethodType.Get
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(response.ResponseCode, Is.EqualTo(ResponseCodes.Ok));
            Assert.That(response.Body.RawBody, Is.EqualTo("hello"));
        });
    }

    [Test]
    public async Task NewUser()
    {
        ResponseEntity response = await HttpRequests.CreateRequest(new HttpRequestConfig
        {
            Url = "http://localhost:8080/users?admin=true",
            Method = RequestMethodType.Put,
            Body = new UrlEncodedBody
            {
                ["name"] = "john",
                ["pass"] = "1234"
            }
        });
        
        Assert.That(response.ResponseCode, Is.EqualTo(ResponseCodes.Created));
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _server.Dispose();
    }
}