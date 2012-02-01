using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Net;
using System.Text.RegularExpressions;
using Languages.Translation;
using Terraria_Server.Language;
using Terraria_Server.Collections;

namespace Languages
{
	public partial class Main : Form
	{
		public const String BING_APP_ID = "6BD88479580D052DE4298DA81829C0E8099A07B2";
		public const Int32 TDSM_BUILD = 37;
		public static FileStream ImageStream = new FileStream("bg.bmp", FileMode.OpenOrCreate);

		public Main()
		{
			InitializeComponent();
		}

		private void Main_Load(object sender, EventArgs e)
		{
			Text += " for TDSM #" + TDSM_BUILD;
			GenerateLoginBG(Color.LightBlue, Size);
			BackgroundImage = Image.FromStream(ImageStream);

			GB_Main.Location = Methods.CenterControl(sender as Main, GB_Main);

			if (!Methods.TryGetTranslations())
			{
				MessageBox.Show("Unable to retrieve Translation data.");
				return;
			}

			Cb_Languages.Items.AddRange(Methods.Languages.Keys.ToArray());

			if (Cb_Languages.Items.Count > 0)
				Cb_Languages.SelectedIndex = 0;
		}

		public static void GenerateLoginBG(Color BaseColour, Size Dimensions, int ColumnDepth = -3, int ColumnTrigger = 3)
		{
			var maxX = Dimensions.Width;
			var maxY = Dimensions.Height;
			var image = new Bitmap(maxX, maxY);
			var initialColour = BaseColour;

			var skip = 0.0;
			var column = new Random();
			for (int y = 0; y < maxY; y++)
			{
				for (int x = 0; x < maxX; x++)
				{
					if (column.Next(ColumnTrigger) == 1)
					{
						var col = initialColour.Downgrade(ColumnDepth);
						image.SetPixel(x, y, col);
					}
					else
						image.SetPixel(x, y, initialColour);
				}

				if (skip == 0.30)
				{
					initialColour = initialColour.Downgrade(1);
					skip = 0;
				}
				else
					skip += 0.30;
			}

			image.Save(ImageStream, ImageFormat.Bmp);
		}

		private void GB_Main_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.FillRectangle(Brushes.LightGray, new Rectangle(new Point(2, 8), GB_Main.Size.Decrease(4, 10)));
		}

		private void Btn_Generate_Click(object sender, EventArgs e)
		{
			LanguageData.RestoreXML();
			var nodes = LanguageData.GetNodes();
			var failed = 0;

			foreach (var keyPair in nodes.FieldwiseClone())
			{
				string translated;
				if (Methods.TryTranslateText(keyPair.Value, Methods.Languages[Cb_Languages.Text], out translated))
					nodes.UpdateProperty(keyPair.Key, translated);
				else
					failed++;
			}

			LanguageData.SaveXML(nodes);
			MessageBox.Show(
				String.Format("{0} node(s) converted!", nodes.Count - failed)
			);
		}
	}

	public static class LanguageData
	{
		public static void RestoreXML()
		{
			Terraria_Server.Language.Languages.LoadClass(Registries.LANGUAGE_FILE, true);
		}

		public static void SaveXML(Dictionary<String, String> data)
		{
			var filePath = Registries.LANGUAGE_FILE;
			if (File.Exists(filePath))
				File.Delete(filePath);

			var xml = new string[data.Count + 2];
			var index = 0;

			xml[index++] = "<Languages>";

			foreach (var keyPair in data)
				xml[index++] = String.Format("\t<{0}>{1}</{2}>", keyPair.Key, keyPair.Value, keyPair.Key);

			xml[index++] = "</Languages>";

			File.WriteAllLines(filePath, xml);
		}

		public static Dictionary<String, String> GetNodes()
		{
			var type = typeof(Terraria_Server.Language.Languages);
			var list = new Dictionary<String, String>();

			var properties = type.GetProperties();

			foreach (var prop in properties)
			{
				var val = prop.GetValue(null, null);

				list.Add(prop.Name, val as String);
			}

			return list;
		}

		public static void UpdateProperty(this Dictionary<String, String> list, string Key, string Value)
		{
			if (list.ContainsKey(Key))
				list[Key] = Value;
		}
	}

	public static class Methods
	{
		public static Color Downgrade(this Color col1, int val)
		{
			var r = col1.R - val;
			var g = col1.G - val;
			var b = col1.B - val;

			if (r < 0)
				r = 0;
			if (g < 0)
				g = 0;
			if (b < 0)
				b = 0;

			if (r > 255)
				r = 255;
			if (g > 255)
				g = 255;
			if (b > 255)
				b = 255;

			return Color.FromArgb(col1.A, r, g, b);
		}

		public static Point CenterControl(Control Parent, Control Child)
		{
			var p = Parent.Size.Divide(2);
			var c = Child.Size.Divide(2);
			var size = p - c;

			return new Point(size.Width, (int)(size.Height / 1.35));
		}

		public static Size Divide(this Size pnt1, int val)
		{
			var x = pnt1.Width / val;
			var y = pnt1.Height / val;

			return new Size(x, y);
		}

		public static Size Decrease(this Size pnt1, int X, int Y)
		{
			var x = pnt1.Width - X;
			var y = pnt1.Height - Y;

			return new Size(x, y);
		}

		public static Dictionary<String, String> Languages = new Dictionary<String, String>();

		public static bool TryGetTranslations()
		{
			try
			{
				Languages.Clear();

				using (var ctx = new LanguageServiceClient())
				{
					var codes = ctx.GetLanguagesForSpeak(Main.BING_APP_ID);
					var langs = ctx.GetLanguageNames(Main.BING_APP_ID, "en", codes);

					for (var i = 0; i < codes.Length; i++)
						Languages.Add(langs[i], codes[i]);
				}

				return true;
			}
			catch { }

			return false;
		}

		public static bool TryTranslateText(string input, string to, out string Translated)
		{
			Translated = String.Empty;

			try
			{
				using (var ctx = new LanguageServiceClient())
				{
					Translated = ctx.Translate(Main.BING_APP_ID, input, "en", to, "text/html", "general");
				}

				return Translated != String.Empty;
			}
			catch { }

			return false;
		}
	}
}
