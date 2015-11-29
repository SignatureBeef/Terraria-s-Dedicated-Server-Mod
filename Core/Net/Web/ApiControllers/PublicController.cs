using System;
using System.Linq;
using System.Web.Http;
using System.Net;
using System.Net.Http;

using Terraria;

namespace TDSM.Core.Net.Web.ApiControllers
{
    /// <summary>
    /// Public access controllers.
    /// </summary>
    [AllowAnonymous]
    public class PublicController : ApiController
    {
        public static bool ShowPlugins { get; set; }

        [TDSMComponent(ComponentEvent.Initialise)]
        internal static void Init(Entry plugin)
        {
            ShowPlugins = plugin.Config.API_ShowPlugins;
        }

        /// <summary>
        /// Outputs public information to such things like server crawlers
        /// </summary>
        public HttpResponseMessage Get()
        {
            return this.Request.CreateResponse(HttpStatusCode.OK, new {
                //Connection info
                Name = Terraria.Main.ActiveWorldFileData.Name,
                Port = Terraria.Netplay.ListenPort,

                PasswordRequired = !String.IsNullOrEmpty(Terraria.Netplay.ServerPassword),

                //Allocations
                MaxPlayers = Terraria.Main.maxNetPlayers,
                Players = Terraria.Main.player
                    .Where(x => x != null && x.active && !String.IsNullOrEmpty(x.name))
                    .Select(x => x.name)
                    .OrderBy(x => x)
                    .ToArray(),

                //Installed plugins/mods
                Plugins = GetPlugins(),

                //Version info
                OTA = OTA.Globals.BuildInfo,
                Terraria = Terraria.Main.versionNumber,

                //Can be used to determine if the actual server is started or not
                ServerState = OTA.Globals.CurrentState
            });
        }

        private string[] GetPlugins()
        {
            if (ShowPlugins)
            {
                return OTA.Plugin.PluginManager._plugins
                    .Where(x => x.Value.IsEnabled)
                    .Select(y => y.Value.Name)
                    .OrderBy(z => z)
                    .ToArray();
            }

            return null;
        }
    }
}

