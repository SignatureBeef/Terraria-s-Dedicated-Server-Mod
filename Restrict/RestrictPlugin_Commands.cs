using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

using Terraria_Server;
using Terraria_Server.Commands;
using Terraria_Server.Misc;
using Terraria_Server.Logging;
using System.IO;

using NDesk.Options;

namespace RestrictPlugin
{
	public partial class RestrictPlugin
	{
		void RegisterCommand (ISender sender, ArgumentList argz)
		{
			try
			{
				var op = false;
				var force = false;
				string password = null;
				var options = new OptionSet ()
				{
					{ "o|op", v => op = true },
					{ "f|force", v => force = true },
					{ "p|password=", v => password = v },
				};
				
				var args = options.Parse (argz);
				
				if (args.Count == 0 || args.Count > 2)
					throw new CommandError ("");
				
				var name = args[0];
				var player = FindPlayer (name);
				
				if (player == null)
				{
					if (! force)
					{
						sender.sendMessage ("restrict.ru: Player not found online, use -f to assume name is correct.");
						return;
					}
				}
				else
					name = player.Name;
				
				var pname = NameTransform (name);
				var oname = OldNameTransform (name);
				
				string hash = null;
				if (password != null)
					hash = Hash (name, password);
				else if (args.Count == 2)
					hash = Hash(name, args[1]);
					//hash = args[1];

				String.Format("User: {0}, Pass: {1}, Hash: {2}", name, password, hash);
				
				if (hash != null)
				{
					var old = users.getValue (pname) ?? users.getValue (oname);
					
					var val = hash;
					if (op) val += ":op";
					
					users.setValue (oname, null);
					users.setValue (pname, val);
                    users.Save(false);
					
					if (player != null)
					{
						player.AuthenticatedAs = name;
						
						if (player != sender)
						{
							if (op)
								player.sendMessage ("<Restrict> You have been registered as an operator.");
							else
								player.sendMessage ("<Restrict> You have been registered.");
						}
						player.Op = op;
					}
					
					if (old != null)
						sender.sendMessage ("restrict.ru: Changed password for: " + name);
					else if (op)
					{
						sender.sendMessage ("restrict.ru: Registered operator: " + name);
						ProgramLog.Admin.Log ("<Restrict> Manually registered new operator: " + name);
					}
					else
					{
						sender.sendMessage ("restrict.ru: Registered user: " + name);
						ProgramLog.Admin.Log ("<Restrict> Manually registered new user: " + name);
					}

				}
				else if (args.Count == 1)
				{
					var entry = users.getValue (pname) ?? users.getValue (oname);
					
					if (entry == null)
					{
						sender.sendMessage ("restrict.ru: No such user in database: " + name);
						return;
					}
					
					var split = entry.Split(':');
					var oldop = split.Length > 1 && split[1] == "op";
					
					if (player != null)
					{
						player.Op = op;
						if (player != sender)
						{
							if (op && !oldop)
								player.sendMessage ("<Restrict> You have been registered as an operator.");
							else if (oldop && !op)
								player.sendMessage ("<Restrict> You have been unregistered as an operator.");
						}
					}
					
					if (oldop != op)
					{
						var val = split[0];
						if (op) val += ":op";
						
						users.setValue (oname, null);
						users.setValue (pname, val);
                        users.Save(false);
						
						if (oldop && !op)
						{
							sender.sendMessage ("restrict.ru: De-opped: " + name);
							ProgramLog.Admin.Log ("<Restrict> De-opped: " + name);
						}
						else if (op && !oldop)
						{
							sender.sendMessage ("restrict.ru: Opped: " + name);
							ProgramLog.Admin.Log ("<Restrict> Opped: " + name);
						}
					}
				}
			}
			catch (OptionException)
			{
				throw new CommandError ("");
			}
		}

		void UnregisterCommand (ISender sender, ArgumentList argz)
		{
			try
			{
				var force = false;
				var options = new OptionSet ()
				{
					{ "f|force", v => force = true },
				};
				
				var args = options.Parse (argz);
				
				if (args.Count == 0 || args.Count > 1)
					throw new CommandError ("");
				
				var name = args[0];
				var player = FindPlayer (name);
				
				if (player == null)
				{
					if (! force)
					{
						sender.sendMessage ("restrict.ur: Player not found online, use -f to assume name is correct.");
						return;
					}
				}
				else
				{
					name = player.Name;
					player.Op = false;
					player.AuthenticatedAs = null;
					
					if (player != sender)
						player.sendMessage ("<Restrict> Your registration has been revoked.");
				}
				
				var pname = NameTransform (name);
				var oname = OldNameTransform (name);
				
				users.setValue (pname, null);
				users.setValue (oname, null);
                users.Save(false);
				
				sender.sendMessage ("restrict.ur: Unregistered user: " + name);
			}
			catch (OptionException)
			{
				throw new CommandError ("");
			}
		}
		
		void OptionsCommand (ISender sender, ArgumentList argz)
		{
			try
			{
				var force = false;
				var ag = allowGuests;
				var rg = restrictGuests;
				var rd = restrictGuestsDoors;
				var si = serverId;
				var changed = false;
				var changed_si = false;
				var reload = false;
				
				var options = new OptionSet ()
				{
					{ "f|force", v => force = true },
					{ "g|allow-guests=", (bool v) => { ag = v; changed = true; } },
					{ "r|restrict-guests=", (bool v) => { rg = v; changed = true; } },
					{ "d|restrict-guests-doors=", (bool v) => { rd = v; changed = true; } },
					{ "L|reload-users", v => reload = true },
					{ "s|server-id=", v =>
						{
							si = v;
							changed = true; changed_si = true;
						}
					},
				};

				var args = options.Parse (argz);
				
				if (args.Count > 0)
					throw new CommandError ("");
					
				if (changed_si && users.Count > 0)
				{
					sender.sendMessage ("restrict.ro: Warning: Changing the server id will invalidate existing password hashes. Use -f to do so anyway.");
					if (! force)
						return;
				}
				
				if (changed)
				{
					properties.setValue ("allow-guests", ag.ToString());
					properties.setValue ("restrict-guests", rg.ToString());
					properties.setValue ("restrict-guests-doors", rd.ToString());
					properties.setValue ("server-id", si.ToString());
                    properties.Save(false);
				}
				
				if (reload)
				{
					sender.sendMessage ("restrict.ro: Reloaded users database, entries: " + users.Count);
					users.Load ();
				}
				
				var msg = string.Concat (
					"Options set: server-id=", si,
					", allow-guests=", ag.ToString(),
					", restrict-guests=", rg.ToString(),
					", restrict-guests-doors=" + rd.ToString());
				
				ProgramLog.Admin.Log ("<Restrict> " + msg);
				sender.sendMessage ("restrict.ro: " + msg);
			}
			catch (OptionException)
			{
				throw new CommandError ("");
			}
		}

		void RequestsCommand (ISender sender, ArgumentList args)
        {
            if (args.TryPop("-all") && args.TryPop("-g"))
            {
                int total = requests.Count;
                for (int i = 0; i < total; i++)
                {
                    RegistrationRequest req = requests.Values.ElementAt(i);
                    RegisterUser(i, req, false);
                }

                Server.notifyOps(
                    String.Format("<Restrict> Registration request granted for {0} user(s).", total)
                , true);
                return;
            }

			int num;
			if (args.TryParseOne ("-g", out num) || args.TryParseOne ("grant", out num))
			{
				RegistrationRequest rq;
				
				if (! requests.TryGetValue (num, out rq))
				{
					sender.sendMessage ("restrict.rr: No such registration request");
					return;
				}

                RegisterUser(num, rq);
			}
			else if (args.TryParseOne ("-d", out num) || args.TryParseOne ("deny", out num))
			{
				RegistrationRequest rq;
				
				if (! requests.TryGetValue (num, out rq))
				{
					sender.sendMessage ("restrict.rr: No such registration request");
					return;
				}
				
				requests.Remove (num);
				
				Server.notifyOps ("<Restrict> Registration request denied for: " + rq.name, true);
				
				var player = FindPlayer (rq.name);
				if (player != null)
					player.sendMessage ("<Restrict> Your registration request has been denied.");
			}
			else
			{
				args.ParseNone ();
				
				sender.sendMessage ("restrict.rr: Pending requests:");
				
				foreach (var kv in requests)
				{
					var rq = kv.Value;
					if (rq == null) continue;
					
					sender.sendMessage (string.Format ("{0,3} : {1} : {2}", kv.Key, rq.address, rq.name));
				}
			}
		}
		
		static HashSet<string> obviousPasswords = new HashSet<string> ()
		{
			"password", "yourpass", "yourpassword", "12345", "123456", "01234", "012345",
			"hello", "mypass", "mypassword", "obama",
		};

		void PlayerPassCommand (ISender sender, string password)
		{
			PlayerCommand ("pass", sender, password);
		}

		void PlayerRegCommand (ISender sender, string password)
		{
			PlayerCommand ("reg", sender, password);
		}

		void PlayerCommand (string command, ISender sender, string password)
		{
			if (! (sender is Player)) return;
			
			var player = (Player) sender;
			
			if (player.AuthenticatedAs != null && command == "reg")
			{
				sender.sendMessage ("<Restrict> Already registered, use /pass to change your password.", 255, 255, 180, 180);
				return;
			}
			else if (player.AuthenticatedAs == null && command == "pass")
			{
				sender.sendMessage ("<Restrict> You are a guest, use /reg to submit a registration request.", 255, 255, 180, 180);
				return;
			}
			
			if (password == null)
			{
				sender.sendMessage ("Error: password cannot be empty.", 255, 255, 180, 180);
				return;
			}
			
			password = password.Trim();
			
			if (password == "")
			{
				sender.sendMessage ("Error: password cannot be empty.", 255, 255, 150, 150);
				return;
			}
			
			if (password.Length < 5)
			{
				sender.sendMessage ("Error: passwords must have at least 5 characters.", 255, 255, 150, 150);
				return;
			}
			
			var name = player.Name;
			var lp = password.ToLower();
			
			if (lp == name.ToLower())
			{
				sender.sendMessage ("Error: passwords cannot be the same as your name.", 255, 255, 150, 150);
				return;
			}
			
			if (obviousPasswords.Contains (lp))
			{
				sender.sendMessage ("Error: password not accepted, too obvious: " + lp, 255, 255, 150, 150);
				return;
			}
			
			if (player.AuthenticatedAs != null)
			{
				var pname = NameTransform (name);
				var oname = OldNameTransform (name);
				var split = (users.getValue(pname) ?? users.getValue(oname)).Split(':');
				var hash = Hash (name, password);
				
				if (hash == split[0])
				{
					sender.sendMessage ("<Restrict> Already registered.");
					return;
				}
				
				if (split.Length > 1 && split[1] == "op")
					hash = hash + ":op";
				
				users.setValue (oname, null);
				users.setValue (pname, hash);
                users.Save(false);
				
				sender.sendMessage ("<Restrict> Your new password is: " + password);
				return;
			}
				
			var address = NetPlay.slots[player.whoAmi].remoteAddress.Split(':')[0];
			
			var previous = requests.Values.Where (r => r != null && r.address == address && r.name == name);
			var cp = previous.Count ();
			if (cp > 0)
			{
				if (cp > 1)
					ProgramLog.Error.Log ("<Restrict> Non-fatal error: more than one identical registration request.");
				
				var rq = previous.First();
				if (password != rq.password)
				{
					rq.password = password;
					sender.sendMessage ("<Restrict> Changed password on pending request to: " + password);
				}
				else
					sender.sendMessage ("<Restrict> Request pending, your password: " + password);
				return;
			}
			
			requests[requestCount] = new RegistrationRequest { name = name, address = address, password = password };
			
			sender.sendMessage ("<Restrict> Request submitted, your password: " + password);
			var msg = string.Concat ("<Restrict> New registration request ", requestCount, " for: ", name);
			Server.notifyOps (msg, false);
			ProgramLog.Users.Log (msg);
			
			requestCount += 1;
		}

        void RegisterUser(int num, RegistrationRequest rq, bool WritetoConsole = true)
        {
            requests.Remove(num);

            var pname = NameTransform(rq.name);
            var hash = Hash(rq.name, rq.password);

            users.setValue(pname, hash);
            users.Save(false);

            var player = FindPlayer(rq.name);
            if (player != null) // TODO: verify IP address
            {
                player.AuthenticatedAs = rq.name;
                player.sendMessage("<Restrict> You are now registered.");
            }

            if(WritetoConsole)
                Server.notifyOps("<Restrict> Registration request granted for: " + rq.name, true);

            var duplicates = requests.Where(kv => kv.Value.name == rq.name).ToArray();
            foreach (var kv in duplicates)
            {
                // deny other requests for the same name
                requests.Remove(kv.Key);
            }
        }
	}
}

