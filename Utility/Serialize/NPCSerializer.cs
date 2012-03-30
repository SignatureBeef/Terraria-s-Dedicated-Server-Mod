using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Terraria_Utilities.Serialize;

namespace Terraria_Utilities.Serialize
{
	/// <summary>
	/// This class is designed just for NPC's.
	/// Due to how Relogic deals with NPC's by overriding a default NPC using SetDefaults
	/// we then need to merge them together and replace the type of the ByName NPC's with Inherits.
	/// By using this class we should then be able to specify which feilds we want rather than excluding.
	/// 
	/// This class will become out of date very fast. 
	/// </summary>
	public static class NPCSerializer
	{
		public static List<String> Feilds = new List<String>()
		{
			"Name", "Type", "aiStyle", "scale",
			"damage", "defense", "lifeMax", "value",
			"knockBackResist", "Width", "Height", "PoisonImmunity",
			"ConfusionImmunity", "BurningImmunity", "CurseImmunity", "slots",
			"noTileCollide", "noGravity", "behindTiles", "NetAlways",
			"DisplayName", "netSkip", "NetID"
		};

		public static List<String> NPCNames = new List<String>()
		{
			"Green Slime", "Pinky", "Baby Slime", "Black Slime",
			"Purple Slime", "Red Slime", "Yellow Slime", "Jungle Slime",
			"Little Eater", "Big Eater", "Short Bones", "Big Boned",
			"Little Stinger", "Big Stinger", "Slimeling", "Slimer2",
			"Heavy Skeleton"
		};

		public static void Serialize(int objects = 1001)
		{
			var type = typeof(Terraria.NPC);
			FileStream fs = new FileStream("DEBUG_NPC.xml", FileMode.OpenOrCreate);
			XmlDictionaryWriter.Create(fs, new XmlWriterSettings()
				{
					Indent = true
				}
			);
			XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(fs);
			writer.WriteStartElement("ArrayOf" + type.Name);

			/* Set this to true to avoid the main textures....I guess in a way, it's a truer examination :P */
			Terraria.Main.dedServ = true;

			for (var npcId = 0; npcId < objects; npcId++)
			{
				var npc = Activator.CreateInstance(type) as Terraria.NPC;
				npc.SetDefaults(npcId);

				if (npc.name == String.Empty)
					continue;

				SerializeNPC(npc, writer);
			}

			foreach (var npcName in NPCNames)
			{
				var npc = Activator.CreateInstance(type) as Terraria.NPC;
				npc.SetDefaults(npcName);

				SerializeNPC(npc, writer, false);
			}

			writer.WriteEndElement();
			writer.Flush();
			writer.Close();
		}

		public static void SerializeNPC(Terraria.NPC npc, XmlDictionaryWriter writer, bool SetDefaults_Int32 = true)
		{
			writer.WriteString("\n\t");
			writer.WriteStartElement("NPC");

			foreach (var feild in Feilds)
			{
				switch (feild)
				{
					case "ConfusionImmunity":
						writer.WriteCustomObject(npc.buffImmune[31], feild);
						break;
					case "PoisonImmunity":
						writer.WriteCustomObject(npc.buffImmune[20], feild);
						break;
					case "BurningImmunity":
						writer.WriteCustomObject(npc.buffImmune[24], feild);
						break;
					case "CurseImmunity":
						writer.WriteCustomObject(npc.buffImmune[39], feild);
						break;
					case "Name":
						writer.WriteCustomObject(npc.name, feild);
						break;
					case "Type":
						if (SetDefaults_Int32)
							writer.WriteCustomObject(npc.type, feild);
						else
							writer.WriteCustomObject(npc.type, "Inherits");
						break;
					case "aiStyle":
						writer.WriteCustomObject(npc.aiStyle, feild);
						break;
					case "scale":
						writer.WriteCustomObject(npc.scale, feild);
						break;
					case "damage":
						writer.WriteCustomObject(npc.damage, feild);
						break;
					case "defense":
						writer.WriteCustomObject(npc.defense, feild);
						break;
					case "lifeMax":
						writer.WriteCustomObject(npc.lifeMax, feild);
						break;
					case "value":
						writer.WriteCustomObject(npc.value, feild);
						break;
					case "knockBackResist":
						writer.WriteCustomObject(npc.knockBackResist, feild);
						break;
					case "Width":
						writer.WriteCustomObject(npc.width, feild);
						break;
					case "Height":
						writer.WriteCustomObject(npc.height, feild);
						break;
					case "slots":
						writer.WriteCustomObject(npc.npcSlots, feild);
						break;
					case "noTileCollide":
						writer.WriteCustomObject(npc.noTileCollide, feild);
						break;
					case "noGravity":
						writer.WriteCustomObject(npc.noGravity, feild);
						break;
					case "behindTiles":
						writer.WriteCustomObject(npc.behindTiles, feild);
						break;
					case "DisplayName":
						if(npc.displayName != npc.name)
							writer.WriteCustomObject(npc.displayName, feild);
						break;
					case "NetAlways":
						writer.WriteCustomObject(npc.netAlways, feild);
						break;
					case "netSkip":
						writer.WriteCustomObject(npc.netSkip, feild);
						break;
					case "NetID":
						writer.WriteCustomObject(npc.netID, feild);
						break;
					default:
						throw new Exception("This class is feild specific! Please add the relative feild above [" + feild + "]");
				}
			}

			writer.WriteString("\n\t");
			writer.WriteEndElement();
		}

		public static void WriteCustomObject(this XmlDictionaryWriter Writer, Object obj, String Feild, String Prefix = "\n\t\t")
		{
			if (obj is Boolean && (Boolean)obj == false ||
				obj is String && (String)obj == String.Empty ||
				obj is Single && (Single)obj == default(Single) ||
				obj is Int32 && (Int32)obj == default(Int32))
				return;


			var data = obj.ToString();

			Writer.WriteString(Prefix);
			Writer.WriteStartElement(Feild);
			Writer.WriteString((obj is Boolean) ? data.ToLower() : data);
			Writer.WriteEndElement();
		}
	}
}
