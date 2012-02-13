using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Utilities
{
	/// <summary>
	/// Very much a hack. Was in a rush.
	/// </summary>
	public static class ConsistencyCheck
	{
		/// <summary>
		/// Check whether TDSM tile sets matches stock.
		/// </summary>
		/// <returns></returns>
		public static bool CheckTileSets(out int err)
		{
			Terraria_Server.Main.Initialize();
			using (var main = new Terraria.Main())
				main.DedServ();

			var merge = Terraria_Server.Main.tileMergeDirt.IsEqualTo(Terraria.Main.tileMergeDirt, out err);
			var tCut = Terraria_Server.Main.tileCut.IsEqualTo(Terraria.Main.tileCut, out err);
			var tAlch = Terraria_Server.Main.tileAlch.IsEqualTo(Terraria.Main.tileAlch, out err);
			var tShine = Terraria_Server.Main.tileShine.IsEqualTo(Terraria.Main.tileShine, out err);
			var wHouse = Terraria_Server.Main.wallHouse.IsEqualTo(Terraria.Main.wallHouse, out err);
			var tStone = Terraria_Server.Main.tileStone.IsEqualTo(Terraria.Main.tileStone, out err);
			var tWater = Terraria_Server.Main.tileWaterDeath.IsEqualTo(Terraria.Main.tileWaterDeath, out err);
			var tLava = Terraria_Server.Main.tileLavaDeath.IsEqualTo(Terraria.Main.tileLavaDeath, out err);
			var table = Terraria_Server.Main.tileTable.IsEqualTo(Terraria.Main.tileTable, out err);
			var bLight = Terraria_Server.Main.tileBlockLight.IsEqualTo(Terraria.Main.tileBlockLight, out err);
			var dung = Terraria_Server.Main.tileDungeon.IsEqualTo(Terraria.Main.tileDungeon, out err);
			var solTop = Terraria_Server.Main.tileSolidTop.IsEqualTo(Terraria.Main.tileSolidTop, out err);
			var tSolid = Terraria_Server.Main.tileSolid.IsEqualTo(Terraria.Main.tileSolid, out err);
			var noAtt = Terraria_Server.Main.tileNoAttach.IsEqualTo(Terraria.Main.tileNoAttach, out err);
			var noFail = Terraria_Server.Main.tileNoFail.IsEqualTo(Terraria.Main.tileNoFail, out err);
			var frameImp = Terraria_Server.Main.tileFrameImportant.IsEqualTo(Terraria.Main.tileFrameImportant, out err);

			return merge && tCut && tAlch && tShine && wHouse && tStone && tWater && tLava && table &&
				bLight && dung && solTop && tSolid && noAtt && noFail && frameImp;
		}

		public static bool IsEqualTo(this Array arr1, Array arr2, out int err)
		{
			err = -1;
			for (var i = 0; i < arr1.Length; i++)
			{
				var current = arr1.GetValue(i);
				var secondary = arr2.GetValue(i);

				if (current != null && secondary != null & current.ToString() != secondary.ToString())
				{
					err = i;
					return false;
				}
			}

			return arr1.Length == arr2.Length;
		}
	}
}
