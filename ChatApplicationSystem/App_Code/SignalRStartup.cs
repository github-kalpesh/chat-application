using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup("SignalRConfiguration", typeof(SignalRStartup))]
public class SignalRStartup
{
    public SignalRStartup()
    {
    }

    public void Configuration(IAppBuilder app)
    {
        app.MapSignalR();
    }
}
