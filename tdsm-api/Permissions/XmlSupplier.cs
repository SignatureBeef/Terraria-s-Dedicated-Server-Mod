using System;
using System.Linq;
using System.Xml.Serialization;

namespace TDSM.API.Permissions
{
    public class XmlSupplier : IPermissionHandler
    {
        private XmlReflect _store;
        private string _path;

        public XmlSupplier(string file)
        {
            _path = file;
        }

        public bool Load()
        {
            try
            {
                //var slz = new XmlSerializer(typeof(XmlReflect));
                //using (var fs = File.OpenRead(_path))
                //{
                //    _store = (XmlReflect)slz.Deserialize(fs);
                //}

                _store = new XmlReflect();
                using (var reader = new System.Xml.XmlTextReader(_path))
                {
                    var doc = new System.Xml.XmlDocument();
                    doc.Load(reader);
                    var list = doc.SelectNodes("XmlPermissions");
                    if (list.Count > 0)
                    {
                        //Support for multiple documents
                        foreach (System.Xml.XmlNode item in list)
                        {
                            foreach (System.Xml.XmlNode node in item)
                            {
                                switch (node.NodeType)
                                {
                                    case System.Xml.XmlNodeType.Element:
                                        switch (node.Name)
                                        {
                                            case "Groups":
                                                foreach (System.Xml.XmlNode child in node)
                                                    ParseGroup(child);
                                                break;
                                            case "Players":
                                                foreach (System.Xml.XmlNode child in node)
                                                    ParsePlayer(child);
                                                break;
                                            default:
                                                PermissionsManager.RequestParse(this, new XmlNodeEventArgs()
                                                {
                                                    Node = node
                                                });
                                                break;
                                        }
                                        break;
                                    case System.Xml.XmlNodeType.None:
                                        break;
                                }
                            }
                        }
                    }
                }

                if (_store.Players != null)
                    foreach (var user in _store.Players)
                    {
                        if (_store.Groups != null && user.Groups != null)
                            user.MatchedGroups = _store.Groups
                                .Where(x =>
                                    user.Groups.Where(y => x.Name != null && y.ToLower() == x.Name.ToLower()).Count() > 0
                                    ||
                                    x.Attributes
                                        .Where(y => y.Key == "ApplyToGuests" && y.Value.ToLower() == "true")
                                        .Count() > 0
                                 )
                                .Distinct()
                                .ToArray();
                    }
                return true;
            }
            catch (Exception e)
            {
                Tools.WriteLine("Failed to load {0}", _path);
                Tools.WriteLine(e);
            }
            Console.Read();
            return false;
        }

        public void ParseGroup(System.Xml.XmlNode node)
        {
            var group = new XmlGroup()
            {
                Attributes = new System.Collections.Generic.Dictionary<String, String>()
            };

            if (node.Attributes != null && node.Attributes.Count > 0)
            {
                foreach (System.Xml.XmlAttribute attr in node.Attributes)
                {
                    if (!group.Attributes.ContainsKey(attr.Name)) group.Attributes.Add(attr.Name, attr.Value);
                }
            }

            foreach (System.Xml.XmlNode child in node)
            {
                switch (child.Name)
                {
                    case "Name":
                        group.Name = child.InnerText;
                        break;
                    case "Nodes":
                        group.Nodes = ParseNodes(child);
                        break;
                }
            }

            if (_store.Groups == null) _store.Groups = new System.Collections.Generic.List<XmlGroup>() { group };
            else _store.Groups.Add(group);
        }

        public void ParsePlayer(System.Xml.XmlNode node)
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
                        player.Nodes = ParseNodes(child);
                        break;
                    case "Groups":
                        player.Groups = ParseArray(child);
                        break;
                }
            }

            if (_store.Players == null) _store.Players = new System.Collections.Generic.List<XmlPlayer>() { player };
            else _store.Players.Add(player);
        }

        public XmlNode[] ParseNodes(System.Xml.XmlNode node)
        {
            var nodes = new System.Collections.Generic.List<XmlNode>();

            foreach (System.Xml.XmlNode child in node)
            {
                if (child.Name == "Node")
                {
                    var nd = new XmlNode()
                    {
                        Key = child.Attributes["Key"].Value
                    };
                    if (child.Attributes["Deny"] != null)
                    {
                        var state = false;
                        Boolean.TryParse(child.Attributes["Deny"].Value, out state);
                        nd.Deny = state;
                    }
                    nodes.Add(nd);
                }
            }

            return nodes.ToArray();
        }

        public string[] ParseArray(System.Xml.XmlNode node)
        {
            var nodes = new System.Collections.Generic.List<String>();

            foreach (System.Xml.XmlNode child in node)
            {
                if (child.Name == "string")
                {
                    nodes.Add(child.InnerText);
                }
            }

            return nodes.ToArray();
        }

        public Permission IsPermitted(string node, BasePlayer player)
        {
            if (player != null)
            {
                if (!player.Op) //Operators should never be restricted
                {
                    var match = _store.Players.Where(x => x.Name == player.AuthenticatedAs).ToArray();
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

                    //Check for DEFAULT group permissions second
                    if (player.AuthenticatedAs == null)
                    {
                        nodesForUser = _store.Groups
                                .Where(y => y.Attributes
                                    .Where(z => z.Key == "ApplyToGuests" && z.Value.ToLower() == "true")
                                    .Count() > 0)
                                .SelectMany(y => y.Nodes)
                                .Where(x => x.Key == node)
                                .ToArray();
                        foreach (var nd in nodesForUser)
                        {
                            if (!found) found = true;
                            if (nd.Deny) return Permission.Denied;
                            if (nd.Deny == false) allowed = true;
                        }
                    }

                    if (!found) return Permission.NodeNonExistent;

                    return allowed ? Permission.Permitted : Permission.Denied;
                }
                else return Permission.Permitted;
            }

            return Permission.Denied;
        }

        public Permission IsPermittedForGroup(string node, Func<System.Collections.Generic.Dictionary<String, String>, Boolean> whereHas = null)
        {
            var groups = _store.Groups
                .Where(x => whereHas(x.Attributes))
                .ToArray();

            var allowed = false;
            var found = false;
            foreach (var group in groups)
            {
                foreach (var nd in group.Nodes.Where(x => x.Key == node))
                {
                    if (!found) found = true;
                    if (nd.Deny) return Permission.Denied;
                    if (nd.Deny == false) allowed = true;
                }
            }

            if (!found) return Permission.NodeNonExistent;

            return allowed ? Permission.Permitted : Permission.Denied;
        }
    }

    [Serializable]
    public class XmlReflect
    {
        public System.Collections.Generic.List<XmlGroup> Groups { get; set; }
        public System.Collections.Generic.List<XmlPlayer> Players { get; set; }
    }

    [Serializable]
    public class XmlGroup
    {
        public string Name { get; set; }
        public XmlNode[] Nodes { get; set; }

        public System.Collections.Generic.Dictionary<String, String> Attributes { get; set; }
    }

    [Serializable]
    public class XmlPlayer
    {
        public string Name { get; set; }
        public XmlNode[] Nodes { get; set; }

        [XmlIgnore]
        public XmlGroup[] MatchedGroups { get; set; }

        public string[] Groups { get; set; }

        public System.Collections.Generic.Dictionary<String, String> Attributes { get; set; }
    }

    [Serializable]
    public class XmlNode
    {
        [XmlAttribute]
        public string Key;

        /// <summary>
        /// Optional deny parameter
        /// </summary>
        [XmlAttribute]
        public bool Deny { get; set; }
    }

    /*
     * Layout
     * 
     * [Defined groups]
     *      - List of group nodes
     * 
     * [Players]
     *      - List of assigned groups
     *      - List player nodes
     */
}
