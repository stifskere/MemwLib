using System.Data;

namespace MemwLib.Http.Types.Content.Implementations;

internal class ExceptionBody : IBody
{
    public string ContentType => "text/html";

    private readonly Exception _exception;

    private readonly bool _dev;
    
    public ExceptionBody(Exception exception, bool dev)
    {
        _exception = exception;
        _dev = dev;
    }
    
    public static IBody ParseImpl(string content)
    {
        throw new ConstraintException("This constructor is not to be used.");
    }

    public string ToRaw()
    {
        return $$"""
                    <!DOCTYPE html>
                    <html lang="en">
                        <head>
                            <title>500: Internal server error!</title>
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
                             
                             .ex-subtitle {
                                color: #b8483c;
                                font-size: 1.1em;
                             }
                             
                             .ex-body {
                               background-color: gray;
                               
                               padding: 1vh 1vw;
                               
                               border-radius: 8px;
                             }
                             
                             .expansible {
                                color: white;
                             }
                            </style>
                        </head>
                        <body>
                            <div class="main">
                               <h1 class="ex-title">An error occured!</h1>
                               {{(_dev ? $"""
                                          <h2 class="ex-subtitle">An instance of {_exception.GetType().Name} was thrown.</h2>
                                          <div class="ex-body">
                                          {_exception}
                                          </div> 
                                          """ 
                                   : """
                                     <h2 class="ex-subtitle">There was an internal error and the server could not fulfill your request.</h2>
                                     <details class="expansible">
                                        <summary>Are you a site administrator?</summary>
                                        <p>Enable the development mode in your server using the server State property to ServerStates.Development</p>
                                     </details>
                                     """)}}
                             </div>
                        </body>
                    </html>
                 """;
    }
}