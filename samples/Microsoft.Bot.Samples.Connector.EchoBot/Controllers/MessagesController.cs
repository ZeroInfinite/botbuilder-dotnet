﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Samples.Middleware;
using System.Text.RegularExpressions;
using System.Threading;

namespace Microsoft.Bot.Samples.Connector.EchoBot.Controllers
{
    [Route("api/[controller]")]
    public class MessagesController : Controller
    {
        Builder.Bot _bot; 

        public MessagesController()
        {
            var connector = new BotFrameworkConnector("", "");

            _bot = new Builder.Bot(connector)                
                .Use(new RegExpRecognizerMiddleare()
                    .AddIntent("echoIntent", new Regex("echo", RegexOptions.IgnoreCase))
                    .AddIntent("helpIntent", new Regex("help", RegexOptions.IgnoreCase)))
                .Use(new EchoMiddleWare())
                .OnReceive( async (context, token) =>
                    {
                        // Example of handling the Help intent w/o using Middleware
                        if (context.IfIntent("helpIntent"))                            
                        {                            
                            context.Reply("Ask this bot to 'Echo something' and it will!");                                
                            return new ReceiveResponse(true);
                        }
                        return new ReceiveResponse(false);
                    }
                );
        }

        [HttpPost]
        public async void Post([FromBody]Activity activity)
        {
            BotFrameworkConnector connector = (BotFrameworkConnector)_bot.Connector; 
            await connector.Receive(HttpContext.Request.Headers, activity, CancellationToken.None);
            return;
        }      
    }
}