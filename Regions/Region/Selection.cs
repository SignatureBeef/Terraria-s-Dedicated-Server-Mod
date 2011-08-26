using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server;
using Terraria_Server.Misc;

namespace Regions.Region
{
    public class Selection
    {
        public static Dictionary<String, Vector2[]> selectionPlayers = new Dictionary<String, Vector2[]>();

        public static Boolean isInSelectionlist(Player player)
        {
            return selectionPlayers.ContainsKey(player.Name);
        }

        public static Vector2[] GetSelection(Player player)
        {
            Vector2[] selections = null;
            if (isInSelectionlist(player) && selectionPlayers.TryGetValue(player.Name, out selections))
                return selections;
            else
                return null;
        }

        public static void SetSelection(Player player, Vector2[] selection)
        {
            if (isInSelectionlist(player))
                selectionPlayers.Remove(player.Name);

            selectionPlayers.Add(player.Name, selection);
        }

    }
}
