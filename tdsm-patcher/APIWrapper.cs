using System;
using System.Reflection;
using System.IO;
using Mono.Cecil;

namespace tdsm.patcher
{
	/// <summary>
	/// This class is to isolate and manage the tdsm.exe referenced by the api dll.
	/// Previously windows would lock the tdsm.exe that was referenced by tdsm.api.dll, which itself was loaded by the patcher.
	/// The locking would cause the patcher (if ran a second time) to fail when saving tdsm.exe
	/// </summary>
	public static class APIWrapper
	{
		static Assembly _api;
		static APIWrapper ()
		{
			//Resolve the tdsm.exe to memory [TODO]
			AppDomain.CurrentDomain.AssemblyResolve += (s, a) =>
			{
				Console.WriteLine("API wants: {0}", a.Name);
				return null;
			};

			using (var ms = new MemoryStream())
			{
				using (var fs = File.OpenRead("tdsm.api.dll"))
				{
					var buff = new byte[256];
					while (fs.Position < fs.Length)
					{
						var task = fs.Read(buff, 0, buff.Length);
						ms.Write(buff, 0, task);
					}
				}

				ms.Seek(0L, SeekOrigin.Begin);
				_api = System.Reflection.Assembly.Load(ms.ToArray());
			}
		}

		public static int Build
		{
			get { 
				return (int)_api.GetType ("tdsm.api.Globals").GetField ("Build").GetValue(null);
			}
		}

		public static string TerrariaVersion
		{
			get { 
				return (string)_api.GetType ("tdsm.api.Globals").GetField ("TerrariaVersion").GetValue(null);
			}
		}

		public static string LibrariesPath
		{
			get { 
				return (string)_api.GetType ("tdsm.api.Globals").GetProperty("LibrariesPath").GetValue(null, null);
			}
		}

		public static string PluginPath
		{
			get { 
				return (string)_api.GetType ("tdsm.api.Globals").GetProperty("PluginPath").GetValue(null, null);
			}
		}

		public static bool IsPatching
		{
			get { 
				return (bool)_api.GetType ("tdsm.api.Globals").GetProperty("IsPatching").GetValue(null, null);
			}
			set {
				_api.GetType ("tdsm.api.Globals").GetProperty("IsPatching").SetValue(null, value, null);
			}
		}

		public static void Initialise()
		{
			IsPatching = true;

			_api.GetType ("tdsm.api.Globals").GetMethod ("Touch").Invoke (null, null);

			var pm = _api.GetType ("tdsm.api.PluginManager");

			pm.GetMethod ("SetHookSource").Invoke (null, new object[] { _api.GetType ("tdsm.api.Plugin.HookPoints") });
			pm.GetMethod ("Initialize").Invoke (null, new object[] { PluginPath, LibrariesPath });
			pm.GetMethod ("LoadPlugins").Invoke (null, null);
		}

		public static void InvokeEvent(AssemblyDefinition terraria, bool isServer)
		{
			var hct = _api.GetType ("tdsm.api.Plugin.HookContext");
			var hap = _api.GetType ("tdsm.api.Plugin.HookArgs").GetNestedType("PatchServer");

			var ctx = Activator.CreateInstance (hct);
			var args = Activator.CreateInstance (hap);

			hap.GetProperty ("Terraria").SetValue (args, (object)terraria, null);
			hap.GetProperty ("IsServer").SetValue (args, isServer, null);
			hap.GetProperty ("IsClient").SetValue (args, !isServer, null);

			var pst = _api.GetType ("tdsm.api.Plugin.HookPoints")
				.GetField ("PatchServer");
			var pse = pst.GetValue(null);

			var arguments = new object [] { ctx, args };
			pst.FieldType.GetMethod ("Invoke").Invoke (pse, arguments);
		}

		public static bool IsDotNet()
		{
			return false;
		}
	}
}

