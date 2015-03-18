using System;
using System.Linq;
using tdsm.api.Permissions;

namespace tdsm.core.WebInterface
{
    public static class WebPermissions
    {
        private static System.Collections.Generic.List<XmlPlayer> _store;

        public static void Load()
        {
            PermissionsManager.AttatchOnParse((sender, args) =>
            {
                switch (args.Node.Name)
                {
                    case "WebUsers":
                        foreach (System.Xml.XmlNode child in args.Node)
                            ParseWebUser(sender as IPermissionHandler, child);
                        break;
                }
            });
        }

        public static void ParseWebUser(IPermissionHandler handler, System.Xml.XmlNode node)
        {
            var player = new XmlPlayer()
            {
                Attributes = new System.Collections.Generic.Dictionary<String, String>()
            };

            if (node.Attributes != null && node.Attributes.Count > 0)
            {
                foreach (System.Xml.XmlAttribute attr in node.Attributes)
                {
                    if (!player.Attributes.ContainsKey(attr.Name)) player.Attributes.Add(attr.Name, attr.Value);
                }
            }

            foreach (System.Xml.XmlNode child in node)
            {
                switch (child.Name)
                {
                    case "Name":
                        player.Name = child.InnerText;
                        break;
                    case "Nodes":
                        player.Nodes = handler.ParseNodes(child);
                        break;
                    case "Groups":
                        player.Groups = handler.ParseArray(child);
                        break;
                }
            }

            if (_store == null) _store = new System.Collections.Generic.List<XmlPlayer>() { player };
            else _store.Add(player);
        }

        public static Permission IsPermitted(string node, WebRequest req)
        {
            if (req != null)
            {
                var match = _store.Where(x => x.Name == req.AuthenticatedAs).ToArray();
                var allowed = false;
                var found = false;

                var nodesForUser = match
                    .SelectMany(x => x.Nodes)
                    .Where(x => x.Key == node)
                    .ToArray();
                foreach (var nd in nodesForUser)
                {
                    if (!found) found = true;
                    if (nd.Deny) return Permission.Denied;
                    if (nd.Deny == false) allowed = true;
                }

                //Check for group permissions second
                nodesForUser = match.SelectMany(x => x.MatchedGroups)
                        .SelectMany(x => x.Nodes)
                        .Where(x => x.Key == node)
                        .ToArray();
                foreach (var nd in nodesForUser)
                {
                    if (!found) found = true;
                    if (nd.Deny) return Permission.Denied;
                    if (nd.Deny == false) allowed = true;
                }

                ////Check for DEFAULT group permissions second
                //if (!String.IsNullOrEmpty(req.AuthenticatedAs))
                //{
                //    nodesForUser = _store.Groups
                //            .Where(y => y.Attributes
                //                .Where(z => z.Key == "ApplyToGuests" && z.Value.ToLower() == "true")
                //                .Count() > 0)
                //            .SelectMany(y => y.Nodes)
                //            .Where(x => x.Key == node)
                //            .ToArray();
                //    foreach (var nd in nodesForUser)
                //    {
                //        if (!found) found = true;
                //        if (nd.Deny) return Permission.Denied;
                //        if (nd.Deny == false) allowed = true;
                //    }
                //}

                if (!found) return Permission.NodeNonExistent;

                return allowed ? Permission.Permitted : Permission.Denied;
            }

            return Permission.Denied;
        }
    }
}
