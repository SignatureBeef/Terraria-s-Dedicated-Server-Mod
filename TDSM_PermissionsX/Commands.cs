using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Plugins;
using Terraria_Server;
using Terraria_Server.Commands;

namespace TDSM_PermissionsX
{
	public partial class PermissionsX
	{
		public void User(ISender sender, ArgumentList args)
		{
			var add = args.TryPop("add"); //xuser adduser username
			var forced = args.TryPop("-f");
			var save = args.TryPop("-save");

			if (add)
			{
				var username = args.GetString(0);
				var user = Server.GetPlayerByName(username);

				if (user == null && !forced) 
					throw new CommandError("No online player found, Use -f if you know for certain that the name is correct.");

				var trueUser = user == null ? username : (user.Name ?? username);

				XmlParser.Users.Add(new User()
				{
					Name = trueUser
				});

				if (save) XmlParser.Save();

				sender.sendMessage(
					String.Format("Permissions for `{0}` have been created.", trueUser)
				);
			}
			else throw new CommandError("Arguments expected.");
		}
	}
}
