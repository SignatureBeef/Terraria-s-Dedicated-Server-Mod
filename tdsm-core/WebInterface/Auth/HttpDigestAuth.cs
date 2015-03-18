
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using tdsm.api.Command;
using tdsm.api.Misc;
namespace tdsm.core.WebInterface.Auth
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="http://talks.codegram.com/http-authentication-methods"/>
    class HttpDigestAuth : IHttpAuth
    {
        private PropertiesFile _database;
        private readonly MD5 _hasher = MD5.Create();

        private Dictionary<String, String> _lastNonceLookup;

        public HttpDigestAuth(string databasePath)
        {
            _database = new PropertiesFile(databasePath);
            _lastNonceLookup = new Dictionary<String, String>();
        }

        public bool CreateLogin(string username, string password, params string[] options)
        {
            if (options.Length != 1) throw new InvalidOperationException("Expected realm as an option.");
            var realm = options[0];

            var hash = ComputeHash(username + ':' + realm + ':' + password);
            return _database.Update(username, hash);
        }

        string ComputeHash(string input)
        {
            var data = _hasher.ComputeHash(Encoding.UTF8.GetBytes(input));

            var builder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
                builder.Append(data[i].ToString("x2"));

            return builder.ToString();
        }

        public void WebAuthCommand(ISender sender, ArgumentList args)
        {
            if (args.TryPop("clear"))
            {
                _lastNonceLookup.Clear();
            }
        }

        public string Authenticate(WebRequest request)
        {
            //username=MD5(HA1:nonce:HA2),realm
            //Initially I was thinking we could simply use the hash as the KEY, and the username as the VALUE. 
            //But i rather have the user being asked for and then compare, rather that matching on ALL user hashs and using whatever username was found.
            //The odds are small, but I have no justification for it as of yet so I am sticking with the username key.
            if (request.Headers.ContainsKey("Auth"))
            {
                var auth = request.Headers["Auth"];
                var split = auth.Split('=');
                if (split.Length == 2)
                {
                    var username = split[0];
                    split = split[1].Split(',');
                    if (split.Length == 2 && split[0].Length == 32)
                    {
                        var hash = split[0];
                        var realm = split[1];

                        if (realm == WebServer.ProviderName)
                        {
                            var key = request.IPAddress + '+' + username;
                            var ha1 = _database.Find(username);
                            if (_lastNonceLookup.ContainsKey(key) && ha1 != null)
                            {
                                var servernonce = _lastNonceLookup[key];
                                var ha2 = ComputeHash("auth:" + request.Path);
                                var serverHash = ComputeHash(ha1 + ':' + servernonce + ':' + ha2);

                                if (serverHash == hash)
                                {
                                    //Push out the new nonce
                                    var nextnonce = Guid.NewGuid().ToString();
                                    _lastNonceLookup[key] = nextnonce;
                                    request.ResponseHeaders.Add("next-nonce", nextnonce);

                                    return username;
                                }
                            }
                            else
                            {
                                //No user exists, so give the request a new nonce and let them revalidate
                                var nextnonce = Guid.NewGuid().ToString();
                                _lastNonceLookup.Add(key, nextnonce);
                                request.ResponseHeaders.Add("next-nonce", nextnonce);

                                request.RepsondHeader(403, "OK", "text/hml", 0);
                                request.End();
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
