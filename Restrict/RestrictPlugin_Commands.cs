using Microsoft.Xna.Framework;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using tdsm.api;
using tdsm.api.Command;
using Terraria;

namespace RestrictPlugin
{
    public partial class RestrictPlugin
    {
        void RegisterCommand(ISender sender, ArgumentList argz)
        {
            try
            {
                var op = false;
                var force = false;
                string password = null;
                var options = new OptionSet()
				{
					{ "o|op", v => op = true },
					{ "f|force", v => force = true },
					{ "p|password=", v => password = v },
				};

                var args = options.Parse(argz);

                if (args.Count == 0 || args.Count > 2)
                    throw new CommandError("");

                var name = args[0];
                var player = Tools.GetPlayerByName(name);

                if (player == null)
                {
                    if (!force)
                    {
                        sender.SendMessage("restrict.ru: Player not found online, use -f to assume name is correct.");
                        return;
                    }
                }
                else
                    name = player.Name;

                var pname = NameTransform(name);
                var oname = OldNameTransform(name);

                string hash = null;
                if (password != null)
                    hash = Hash(name, password);
                else if (args.Count == 2)
                    hash = Hash(name, args[1]);
                //hash = args[1];

                String.Format("User: {0}, Pass: {1}, Hash: {2}", name, password, hash);

                if (hash != null)
                {
                    var old = users.Find(pname) ?? users.Find(oname);

                    var val = hash;
                    if (op) val += ":op";

                    users.Update(oname, null);
                    users.Update(pname, val);
                    users.Save();

                    if (player != null)
                    {
                        player.SetAuthentication(name, this.Name);

                        if (player != sender)
                        {
                            if (op)
                                player.SendMessage("<Restrict> You have been registered as an operator.");
                            else
                                player.SendMessage("<Restrict> You have been registered.");
                        }
                        player.Op = op;
                    }

                    if (old != null)
                        sender.SendMessage("restrict.ru: Changed password for: " + name);
                    else if (op)
                    {
                        sender.SendMessage("restrict.ru: Registered operator: " + name);
                        Tools.WriteLine("<Restrict> Manually registered new operator: " + name);
                    }
                    else
                    {
                        sender.SendMessage("restrict.ru: Registered user: " + name);
                        Tools.WriteLine("<Restrict> Manually registered new user: " + name);
                    }

                }
                else if (args.Count == 1)
                {
                    var entry = users.Find(pname) ?? users.Find(oname);

                    if (entry == null)
                    {
                        sender.SendMessage("restrict.ru: No such user in database: " + name);
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
                                player.SendMessage("<Restrict> You have been registered as an operator.");
                            else if (oldop && !op)
                                player.SendMessage("<Restrict> You have been unregistered as an operator.");
                        }
                    }

                    if (oldop != op)
                    {
                        var val = split[0];
                        if (op) val += ":op";

                        users.Update(oname, null);
                        users.Update(pname, val);
                        users.Save();

                        if (oldop && !op)
                        {
                            sender.SendMessage("restrict.ru: De-opped: " + name);
                            Tools.WriteLine("<Restrict> De-opped: " + name);
                        }
                        else if (op && !oldop)
                        {
                            sender.SendMessage("restrict.ru: Opped: " + name);
                            Tools.WriteLine("<Restrict> Opped: " + name);
                        }
                    }
                }
            }
            catch (OptionException)
            {
                throw new CommandError("");
            }
        }

        void UnregisterCommand(ISender sender, ArgumentList argz)
        {
            try
            {
                var force = false;
                var options = new OptionSet()
				{
					{ "f|force", v => force = true },
				};

                var args = options.Parse(argz);

                if (args.Count == 0 || args.Count > 1)
                    throw new CommandError("");

                var name = args[0];
                var player = Tools.GetPlayerByName(name);

                if (player == null)
                {
                    if (!force)
                    {
                        sender.SendMessage("restrict.ur: Player not found online, use -f to assume name is correct.");
                        return;
                    }
                }
                else
                {
                    name = player.Name;
                    player.Op = false;
                    player.AuthenticatedAs = null;

                    if (player != sender)
                        player.SendMessage("<Restrict> Your registration has been revoked.");
                }

                var pname = NameTransform(name);
                var oname = OldNameTransform(name);

                users.Update(pname, null);
                users.Update(oname, null);
                users.Save();

                sender.SendMessage("restrict.ur: Unregistered user: " + name);
            }
            catch (OptionException)
            {
                throw new CommandError("");
            }
        }

        void OptionsCommand(ISender sender, ArgumentList argz)
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

                var options = new OptionSet()
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

                var args = options.Parse(argz);

                if (args.Count > 0)
                    throw new CommandError("");

                if (changed_si && users.Count > 0)
                {
                    sender.SendMessage("restrict.ro: Warning: Changing the server id will invalidate existing password hashes. Use -f to do so anyway.");
                    if (!force)
                        return;
                }

                if (changed)
                {
                    properties.Update("allow-guests", ag.ToString());
                    properties.Update("restrict-guests", rg.ToString());
                    properties.Update("restrict-guests-doors", rd.ToString());
                    properties.Update("server-id", si.ToString());
                    properties.Save();
                }

                if (reload)
                {
                    sender.SendMessage("restrict.ro: Reloaded users database, entries: " + users.Count);
                    users.Load();
                }

                var msg = string.Concat(
                    "Options set: server-id=", si,
                    ", allow-guests=", ag.ToString(),
                    ", restrict-guests=", rg.ToString(),
                    ", restrict-guests-doors=" + rd.ToString());

                Tools.WriteLine("<Restrict> " + msg);
                sender.SendMessage("restrict.ro: " + msg);
            }
            catch (OptionException)
            {
                throw new CommandError("");
            }
        }

        void RequestsCommand(ISender sender, ArgumentList args)
        {
            if (args.TryPop("-all") && args.TryPop("-g"))
            {
                int total = requests.Count;
                for (int i = 0; i < total; i++)
                {
                    RegistrationRequest req = requests.Values.ElementAt(i);
                    RegisterUser(i, req, false);
                }

                Tools.NotifyAllOps(
                    String.Format("<Restrict> Registration request granted for {0} user(s).", total)
                , true);
                return;
            }

            int num;
            if (args.TryParseOne("-g", out num) || args.TryParseOne("grant", out num))
            {
                RegistrationRequest rq;

                if (!requests.TryGetValue(num, out rq))
                {
                    sender.SendMessage("restrict.rr: No such registration request");
                    return;
                }

                RegisterUser(num, rq);
            }
            else if (args.TryParseOne("-d", out num) || args.TryParseOne("deny", out num))
            {
                RegistrationRequest rq;

                if (!requests.TryGetValue(num, out rq))
                {
                    sender.SendMessage("restrict.rr: No such registration request");
                    return;
                }

                requests.Remove(num);

                Tools.NotifyAllOps("<Restrict> Registration request denied for: " + rq.name, true);

                var player = Tools.GetPlayerByName(rq.name);
                if (player != null)
                    player.SendMessage("<Restrict> Your registration request has been denied.");
            }
            else
            {
                args.ParseNone();

                sender.SendMessage("restrict.rr: Pending requests:");

                foreach (var kv in requests)
                {
                    var rq = kv.Value;
                    if (rq == null) continue;

                    sender.SendMessage(string.Format("{0,3} : {1} : {2}", kv.Key, rq.address, rq.name));
                }
            }
        }

        static HashSet<string> obviousPasswords = new HashSet<string>()
		{
			"password", "yourpass", "yourpassword", "12345", "123456", "01234", "012345",
			"hello", "mypass", "mypassword", "obama",
		};

        void PlayerPassCommand(ISender sender, string password)
        {
            PlayerCommand("pass", sender, password);
        }

        void PlayerRegCommand(ISender sender, string password)
        {
            PlayerCommand("reg", sender, password);
        }

        void PlayerLoginCommand(ISender sender, string password)
        {
            if (!(sender is Player)) return;

            var player = (Player)sender;

            if (player.AuthenticatedAs == null)
            {
                var name = player.Name;
                var pname = NameTransform(name);
                var oname = OldNameTransform(name);
                string entry = null;

                lock (users)
                {
                    entry = users.Find(pname) ?? users.Find(oname);
                }

                if (entry != null)
                {
                    var split = entry.Split(':');
                    var hash = split[0];
                    var hash2 = Hash(name, password);

                    if (hash != hash2)
                    {
                        sender.SendMessage("Authentication failed.", 255, 255, 180, 180);
                        return;
                    }

                    if (split.Length > 1 && split[1] == "op")
                    {
                        player.Op = true;
                    }

                    player.SetAuthentication(name, this.Name);

                    if (player.Op)
                        player.Message(255, new Color(128, 128, 255), "This humble server welcomes back Their Operating Highness.");
                    else
                        player.Message(255, new Color(128, 255, 128), "Welcome back, registered user.");
                }
                else sender.SendMessage("Authentication failed.", 255, 255, 180, 180);
            }
            else sender.SendMessage("You are already authenticated.", 255, 255, 180, 180);
        }

        void PlayerCommand(string command, ISender sender, string password)
        {
            if (!(sender is Player)) return;

            var player = (Player)sender;

            if (player.AuthenticatedAs != null && command == "reg")
            {
                sender.SendMessage("<Restrict> Already registered, use /pass to change your password.", 255, 255, 180, 180);
                return;
            }
            else if (player.AuthenticatedAs == null && command == "pass")
            {
                sender.SendMessage("<Restrict> You are a guest, use /reg to submit a registration request.", 255, 255, 180, 180);
                return;
            }

            if (password == null)
            {
                sender.SendMessage("Error: password cannot be empty.", 255, 255, 180, 180);
                return;
            }

            password = password.Trim();

            if (password == "")
            {
                sender.SendMessage("Error: password cannot be empty.", 255, 255, 150, 150);
                return;
            }

            if (password.Length < 5 || password.ToCharArray().Distinct().Count() < 5)
            {
                sender.SendMessage("Error: passwords must have at least 5 unique characters.", 255, 255, 150, 150);
                return;
            }

            var name = player.Name;
            var lp = password.ToLower();

            if (lp == name.ToLower())
            {
                sender.SendMessage("Error: passwords cannot be the same as your name.", 255, 255, 150, 150);
                return;
            }

            if (obviousPasswords.Contains(lp))
            {
                sender.SendMessage("Error: password not accepted, too obvious: " + lp, 255, 255, 150, 150);
                return;
            }

            if (player.AuthenticatedAs != null)
            {
                var pname = NameTransform(name);
                var oname = OldNameTransform(name);
                var split = (users.Find(pname) ?? users.Find(oname)).Split(':');
                var hash = Hash(name, password);

                if (hash == split[0])
                {
                    sender.SendMessage("<Restrict> Already registered.");
                    return;
                }

                if (split.Length > 1 && split[1] == "op")
                    hash = hash + ":op";

                users.Update(oname, null);
                users.Update(pname, hash);
                users.Save();

                sender.SendMessage("<Restrict> Your new password is: " + password);
                return;
            }

            var address = tdsm.api.Callbacks.NetplayCallback.slots[player.whoAmi].remoteAddress.Split(':')[0];

            var previous = requests.Values.Where(r => r != null && r.address == address && r.name == name);
            var cp = previous.Count();
            if (cp > 0)
            {
                if (cp > 1)
                    Tools.WriteLine("<Restrict> Non-fatal error: more than one identical registration request.");

                var rq = previous.First();
                if (password != rq.password)
                {
                    rq.password = password;
                    sender.SendMessage("<Restrict> Changed password on pending request to: " + password);
                }
                else
                    sender.SendMessage("<Restrict> Request pending, your password: " + password);
                return;
            }

            requests[requestCount] = new RegistrationRequest { name = name, address = address, password = password };

            sender.SendMessage("<Restrict> Request submitted, your password: " + password);
            var msg = string.Concat("<Restrict> New registration request ", requestCount, " for: ", name);
            Tools.NotifyAllOps(msg, false);
            Tools.WriteLine(msg);

            requestCount += 1;
        }

        void RegisterUser(int num, RegistrationRequest rq, bool WritetoConsole = true)
        {
            requests.Remove(num);

            var pname = NameTransform(rq.name);
            var hash = Hash(rq.name, rq.password);

            users.Update(pname, hash);
            users.Save();

            var player = Tools.GetPlayerByName(rq.name);
            if (player != null) // TODO: verify IP address
            {
                player.SetAuthentication(rq.name, this.Name);
                player.SendMessage("<Restrict> You are now registered.");
            }

            if (WritetoConsole)
                Tools.NotifyAllOps("<Restrict> Registration request granted for: " + rq.name, true);

            var duplicates = requests.Where(kv => kv.Value.name == rq.name).ToArray();
            foreach (var kv in duplicates)
            {
                // deny other requests for the same name
                requests.Remove(kv.Key);
            }
        }
    }
}

