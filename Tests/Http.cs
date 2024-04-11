using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MemwLib.Http;
using MemwLib.Http.Extensions.Cookies;
using MemwLib.Http.Types;
using MemwLib.Http.Types.Attributes;
using MemwLib.Http.Types.Configuration;
using MemwLib.Http.Types.Content.Common;
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
        _server = new HttpServer(new HttpServerConfig
        {
            Port = 8080,
            SslBehavior = SslBehavior.DoNotUseCertificateIfNotFound
        });
        
        _server.OnLog += message =>
        {
            Assert.That(message.Type, Is.Not.EqualTo(LogType.Error));
        };

        _server.AddEndpoint(RequestMethodType.Get, new Regex("/users/(?'user'[^/]+)", RegexOptions.Compiled), request =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(request.CapturedGroups, Has.Length.EqualTo(1));
                Assert.That(request.CapturedGroups.Contains("user"), Is.True);
                Assert.That(request.CapturedGroups["user"], Is.EqualTo("me"));
                Assert.That(request.Headers["Authorization"], Is.Not.Null);
            });

            string[] auth = Encoding.ASCII
                    .GetString(Convert.FromBase64String(request.Headers["Authorization"]![0].Split(' ')[1]))
                    .Split(':');
            
            Assert.Multiple(() =>
            {
                Assert.That(auth[0], Is.EqualTo("john"));
                Assert.That(auth[1], Is.EqualTo("1234"));
            });
            
            return new ResponseEntity(ResponseCodes.Ok, new StringBody("hello"));
        });

        _server.AddEndpoint(RequestMethodType.Put, "/users", request => {
            UrlEncodedBody body = request.Body.ReadAs<UrlEncodedBody>();
            
            Assert.Multiple(() =>
            {
                Assert.That(request.Path.Parameters, Has.Length.EqualTo(1));
                Assert.That(request.Path.Parameters.Contains("admin"), Is.True);
                Assert.That(request.Path.Parameters["admin"], Is.EqualTo("true"));
                
                Assert.That(body, Is.Not.Null);
                Assert.That(body["name"], Is.EqualTo("john"));
                Assert.That(body["pass"], Is.EqualTo("1234"));
            });
            
            return new ResponseEntity(ResponseCodes.Created);
        });

        _server.AddEndpoint(RequestMethodType.Get, "/cookies", _ 
            => new ResponseEntity(ResponseCodes.Ok)
                    .WithCookie(new Cookie
                    {
                        Name = "Name",
                        Value = "Value",
                        Domain = "example.com",
                        Path = "/",
                        Secure = true,
                        HttpOnly = true,
                        SameSite = SameSiteType.None
                    })
        );

        _server.AddGroup<RoutesFromClass>();

        _server.AddEndpoint(RequestMethodType.Options, new Regex(".+"), _ => new ResponseEntity(ResponseCodes.Ok));
        _server.AddGlobalMiddleware(_ => Middleware.Next.WithHeader("Access-Control-Allow-Origin", "*"));
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
            Assert.That(response.Body.ReadAs<StringBody>().ToString(), Is.EqualTo("hello"));
        });
        
        Assert.That(response.Headers.Contains("Access-Control-Allow-Origin"), Is.True);
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
        Assert.Multiple(() =>
        {
            Assert.That(response.ResponseCode, Is.EqualTo(ResponseCodes.Created));
            Assert.That(response.Headers.Contains("Access-Control-Allow-Origin"), Is.True);
        });
    }

    [Test]
    public async Task TestHeaders()
    {
        ResponseEntity response = await HttpRequests.CreateRequest(new HttpRequestConfig
        {
            Url = "http://localhost:8080/header-test/uhh",
            Method = RequestMethodType.Get
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(response.Headers.Contains("My-Header-From-Middleware"), Is.True);
            Assert.That(response.Headers.Contains("My-Header-From-Handler"), Is.True);
                
            Assert.That(response.Headers["My-Header-From-Middleware"]?[0], Is.EqualTo("true"));
            Assert.That(response.Headers["My-Header-From-Handler"]?[0], Is.EqualTo("true"));
        });
        
        Assert.That(response.Headers.Contains("Access-Control-Allow-Origin"), Is.True);
    }

    [Test]
    public async Task MethodOverlap()
    {
        ResponseEntity response = await HttpRequests.CreateRequest(new HttpRequestConfig
        {
            Url = "http://localhost:8080/header-test/uhh",
            Method = RequestMethodType.Options
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(response.ResponseCode, Is.EqualTo(ResponseCodes.Ok));
            Assert.That(response.Headers.Contains("Access-Control-Allow-Origin"), Is.True);
        });
    }

    [Test]
    public async Task Cookies()
    {
        ResponseEntity response = await HttpRequests.CreateRequest(new HttpRequestConfig
        {
            Url = "http://localhost:8080/cookies",
        });
        
        Assert.That(response.Headers, Contains.Key("Set-Cookie"));
        Cookie responseCookie = Cookie.Parse(response.Headers["Set-Cookie"]![0]);
        
        Assert.That(responseCookie is {
            Name: "Name",
            Value: "Value",
            Domain: "example.com",
            Path: "/",
            Secure: true,
            HttpOnly: true,
            SameSite: SameSiteType.None
        });
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _server.Dispose();
    }
}

public class AddHeaderMiddlewareAttribute(string value) : MiddlewareAttribute
{
    public override IResponsible Handler(RequestEntity request)
    {
        return Middleware.Next
            .WithHeader("My-Header-From-Middleware", value);
    }
}

[RouteGroup("/header-test")]
public class RoutesFromClass
{
    [AddHeaderMiddleware("true")]
    [Route(RequestMethodType.Get, "/uhh"), UsedImplicitly]
    public static ResponseEntity TestHeader(RequestEntity _)
    {
        return new ResponseEntity(ResponseCodes.Ok)
            .WithHeader("My-Header-From-Handler", "true");
    }
}