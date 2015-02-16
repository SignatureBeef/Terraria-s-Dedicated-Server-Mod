using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace tdsm.api.Permissions
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
                var slz = new XmlSerializer(typeof(XmlReflect));
                using (var fs = File.OpenRead(_path))
                {
                    _store = (XmlReflect)slz.Deserialize(fs);
                }

                foreach (var user in _store.Players)
                {
                    if (_store.Groups != null && user.Groups != null)
                        user.MatchedGroups = _store.Groups
                            .Where(x =>
                                user.Groups.Where(y => x.Name != null && y.ToLower() == x.Name.ToLower()).Count() > 0
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

        public bool IsRestricted(string node, BasePlayer player)
        {
            if (player.Op) return false;

            if (player.AuthenticatedAs != null)
            {
                var lowered = player.AuthenticatedAs.ToLower();
                var matches = _store.Players.Where(x => x.Name.ToLower() == lowered).ToArray();
                if (matches.Length > 0)
                {
                    var loweredNode = node.ToLower();
                    var allowed = false;
                    foreach (var match in matches)
                    {
                        //If any denied then the player is restricted
                        if (match.MatchedGroups
                            .SelectMany(x => x.Nodes)
                            .Where(x => x.Key.ToLower() == loweredNode && x.Deny)
                            .Count() > 0) return true;
                        if (_store.Groups
                            .Where(x => x.AppyToGuests)
                            .SelectMany(x => x.Nodes)
                            .Where(x => x.Key.ToLower() == loweredNode && x.Deny)
                            .Count() > 0) return true;

                        if (match.MatchedGroups
                            .SelectMany(x => x.Nodes)
                            .Where(x => x.Key.ToLower() == loweredNode && !x.Deny)
                            .Count() > 0) allowed = true;
                        if (_store.Groups
                            .Where(x => x.AppyToGuests)
                            .SelectMany(x => x.Nodes)
                            .Where(x => x.Key.ToLower() == loweredNode && !x.Deny)
                            .Count() > 0) allowed = true;
                    }

                    return !allowed;
                }
            }

            var groups = _store.Groups.Where(x => x.AppyToGuests).ToArray();
            if (groups.Length > 0)
            {
                var loweredNode = node.ToLower();
                var allowed = false;
                foreach (var match in groups)
                {
                    if (match.Nodes
                        .Where(x => x.Key.ToLower() == loweredNode && x.Deny)
                        .Count() > 0) return true;

                    if (match.Nodes
                        .Where(x => x.Key.ToLower() == loweredNode && !x.Deny)
                        .Count() > 0) allowed = true;
                }

                return !allowed;
            }

            return !player.Op;
        }
    }

    [Serializable]
    public class XmlReflect
    {
        public XmlGroup[] Groups { get; set; }
        public XmlPlayer[] Players { get; set; }
    }

    [Serializable]
    public class XmlGroup
    {
        public string Name { get; set; }
        [XmlAttribute]
        public bool AppyToGuests { get; set; }
        public XmlNode[] Nodes { get; set; }
    }

    [Serializable]
    public class XmlPlayer
    {
        public string Name { get; set; }
        public XmlNode[] Nodes { get; set; }

        [XmlIgnore]
        public XmlGroup[] MatchedGroups { get; set; }
        public string[] Groups { get; set; }
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
