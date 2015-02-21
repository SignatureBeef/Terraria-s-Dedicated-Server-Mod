using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace tdsm.api.ID
{
    public static class Lookup
    {
        class FieldInfoComparer<T> : IEqualityComparer<FieldInfo>
        {
            public bool Equals(FieldInfo x, FieldInfo y)
            {
                return ((T)(object)x.GetValue(null)).Equals((T)(object)y.GetValue(null));
            }

            public int GetHashCode(FieldInfo obj)
            {
                return ((T)(object)obj.GetValue(null)).GetHashCode();
            }
        }

        static Dictionary<Int32, String> _buff;
        static Dictionary<Int16, String> _item;
        static Dictionary<Int16, String> _message;
        static Dictionary<Int32, String> _status;
        static Dictionary<UInt16, String> _tile;
        static Dictionary<Byte, String> _wall;

        internal static void Initialise()
        {
            _buff = MapFromType<Int32>(typeof(Terraria.ID.BuffID));
            _item = MapFromType<Int16>(typeof(Terraria.ID.ItemID));
            _message = MapFromType<Int16>(typeof(Terraria.ID.MessageID));
            _status = MapFromType<Int32>(typeof(Terraria.ID.StatusID));
            _tile = MapFromType<UInt16>(typeof(Terraria.ID.TileID));
            _wall = MapFromType<Byte>(typeof(Terraria.ID.WallID));
        }

        static Dictionary<T, String> MapFromType<T>(Type type)
        {
            return type
                .GetFields()
                .Distinct(new FieldInfoComparer<T>())
                .ToDictionary(x => (T)x.GetValue(null), x => x.Name);
        }

        /// <summary>
        /// Gets the name of the buff by the given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string FindBuffById(int id)
        {
            if (_buff.ContainsKey(id)) return _buff[id];
            return null;
        }

        /// <summary>
        /// Gets the name of the item by the given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string FindItemById(short id)
        {
            if (_item.ContainsKey(id)) return _item[id];
            return null;
        }

        /// <summary>
        /// Gets the text of the message by the given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string FindMessageById(short id)
        {
            if (_message.ContainsKey(id)) return _message[id];
            return null;
        }

        /// <summary>
        /// Gets the text representation for the given status id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string FindStatusById(int id)
        {
            if (_status.ContainsKey(id)) return _status[id];
            return null;
        }

        /// <summary>
        /// Gets the name of the tile by the given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string FindTileById(ushort id)
        {
            if (_tile.ContainsKey(id)) return _tile[id];
            return null;
        }

        /// <summary>
        /// Gets the name of the wall by the given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string FindWallById(byte id)
        {
            if (_wall.ContainsKey(id)) return _wall[id];
            return null;
        }
    }
}
