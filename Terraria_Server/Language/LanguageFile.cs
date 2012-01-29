using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Misc;
using System.Xml;
using System.Reflection;
using System.Xml.Serialization;
using System.IO;

namespace Terraria_Server.Language
{
	public class LanguageFile
	{
		public static string Disconnected { get; set; }


		public static void LoadClass(string filePath, bool restore = false)
		{
			if (!File.Exists(filePath) || restore)
			{
				if (File.Exists(filePath))
					File.Delete(filePath);

				using (var ctx = Assembly.GetExecutingAssembly().GetManifestResourceStream(Collections.Registries.DEFINITIONS + filePath))
				{
					using (var stream = File.OpenWrite(filePath))
					{
						var buff = new byte[ctx.Length];
						ctx.Read(buff, 0, buff.Length);
						stream.Write(buff, 0, buff.Length);
					}
				}
			}

			var document = new XmlDocument();
			document.Load(File.Open(filePath, FileMode.Open));

			var type = typeof(LanguageFile);

			foreach (XmlNode node in document.ChildNodes)
			{
				try
				{
					var property = node.Name;
					var value = node.InnerText;

					var properties = from x in type.GetProperties() where x.Name == property select x;

					foreach (var prop in properties)
					{
						prop.SetValue(null, value, null);
					}
				}
				catch { }
			}
		}
	}
}