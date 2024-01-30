using System.Data;

namespace MemwLib.Http.Types.Content.Implementations;

internal class MethodNotAllowedBody : IBody
{
    private RequestMethodType _method;

    private string _route;
    
    public string ContentType => "text/html";

    public MethodNotAllowedBody(RequestMethodType method, string route)
    {
        _method = method;
        _route = route;
    }
    
    public static IBody ParseImpl(string content)
    {
        throw new ConstraintException("This is not meant to be used.");
    }

    public string ToRaw()
    {
        return $$"""
               <!DOCTYPE html>
               <html lang="en">
                    <head>
                        <title>Method not allowed</title>
                        <style>
                        html {
                            background-color: #262626;
                         
                            font-family: ubuntu, 'sans-serif';
                        }

                        .main {
                            display: flex;
                            flex-direction: column;
                         
                            width: 50%;
                         
                            position: absolute;
                         
                            left: 50%;
                            transform: translateX(-50%);
                        }

                        .ex-title {
                            color: #b8483c;
                        }
                        </style>
                    </head>
                    <body>
                        <div class="main">
                            <h1 class="ex-title">Cannot {{_method.ToString().ToUpper()}}: {{_route}}</h1>
                        </div>
                    </body>
               </html>
               """;
    }
}