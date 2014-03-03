using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebApplication2.Startup))]
namespace WebApplication2
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
        
    }

    public class OSIPI
    {
        PISDK.Server PI_SERVER;
        public string rpiCONNECT;
        
        public void connect_Server(string Piservername)     // Got from AlpacaFinal / OSIPI.cs
        {
            PISDK.PISDK SDK = new PISDK.PISDK();            //Creates new instance of PI SDK
            PI_SERVER = SDK.Servers["ESMARTSERVER-PC"];     //Assign PI server to local machine [Piservername]
            PI_SERVER.Open(PI_SERVER.DefaultUser);          //Open connection through default user

            PISDK.PIValue PIconnect = new PISDK.PIValue();  //Create new instance of PI value
            PIconnect = (PISDK.PIValue)PI_SERVER.PIPoints["SP14VICE_Connection"].Data;
            rpiCONNECT = PIconnect.ToString();
        }


    }
}
