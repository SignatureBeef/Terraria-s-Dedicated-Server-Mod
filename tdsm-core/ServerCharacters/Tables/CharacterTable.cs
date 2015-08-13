using System;
using TDSM.API.Data;
using TDSM.Core.ServerCharacters;
using TDSM.API;

namespace TDSM.Core.ServerCharacters.Tables
{
    internal class CharacterTable
    {
        public const String TableName = "SSC";

        static class ColumnNames
        {
            public const String Id = "Id";
            public const String UserId = "UserId";
            public const String UUID = "UUID";
            public const String Health = "Health";
            public const String MaxHealth = "MaxHealth";
            public const String Mana = "Mana";
            public const String MaxMana = "MaxMana";
            public const String SpawnX = "SpawnX";
            public const String SpawnY = "SpawnY";
            public const String Hair = "Hair";
            public const String HairDye = "HairDye";
            public const String HideVisual = "HideVisual";
            public const String Difficulty = "Difficulty";
            public const String HairColor = "HairColor";
            public const String SkinColor = "SkinColor";
            public const String EyeColor = "EyeColor";
            public const String ShirtColor = "ShirtColor";
            public const String UnderShirtColor = "UnderShirtColor";
            public const String PantsColor = "PantsColor";
            public const String ShoeColor = "ShoeColor";
            public const String AnglerQuests = "AnglerQuests";
        }

        public static readonly TableColumn[] Columns = new TableColumn[]
        {
            new TableColumn(ColumnNames.Id, typeof(Int32), true, true),
            new TableColumn(ColumnNames.UserId, typeof(Int32), true),
            new TableColumn(ColumnNames.UUID, typeof(String), 36, true),
            new TableColumn(ColumnNames.Health, typeof(Int32)),
            new TableColumn(ColumnNames.MaxHealth, typeof(Int32)),
            new TableColumn(ColumnNames.Mana, typeof(Int32)),
            new TableColumn(ColumnNames.MaxMana, typeof(Int32)),
            new TableColumn(ColumnNames.SpawnX, typeof(Int32)),
            new TableColumn(ColumnNames.SpawnY, typeof(Int32)),
            new TableColumn(ColumnNames.Hair, typeof(Int32)),
            new TableColumn(ColumnNames.HairDye, typeof(Byte)),

            //This could techincally fit in a short, but that would leave only 6 bits left (HideVisual is 10)
            //I would rather use a typical int just for furture additions
            new TableColumn(ColumnNames.HideVisual, typeof(Int32)), 

            new TableColumn(ColumnNames.Difficulty, typeof(Byte)),
            new TableColumn(ColumnNames.HairColor, typeof(UInt32)),
            new TableColumn(ColumnNames.SkinColor, typeof(UInt32)),
            new TableColumn(ColumnNames.EyeColor, typeof(UInt32)),
            new TableColumn(ColumnNames.ShirtColor, typeof(UInt32)),
            new TableColumn(ColumnNames.UnderShirtColor, typeof(UInt32)),
            new TableColumn(ColumnNames.PantsColor, typeof(UInt32)),
            new TableColumn(ColumnNames.ShoeColor, typeof(UInt32)),
            new TableColumn(ColumnNames.AnglerQuests, typeof(Int32))
        };

        public static bool Exists()
        {
            using (var bl = Storage.GetBuilder(CharacterManager.SQLSafeName))
            {
                bl.TableExists(TableName);

                return Storage.Execute(bl);
            }
        }

        public static bool Create()
        {
            using (var bl = Storage.GetBuilder(CharacterManager.SQLSafeName))
            {
                bl.TableCreate(TableName, Columns);

                return Storage.ExecuteNonQuery(bl) > 0;
            }
        }

        public static int NewCharacter
        (
            CharacterMode mode,
            string auth,
            string clientUUID,
            int health,
            int maxHealth,
            int mana,
            int maxMana,
            int spawnX,
            int spawnY,
            int hair,
            byte hairDye,
            bool[] hideVisual,
            byte difficulty,
            Microsoft.Xna.Framework.Color hairColor,
            Microsoft.Xna.Framework.Color skinColor,
            Microsoft.Xna.Framework.Color eyeColor,
            Microsoft.Xna.Framework.Color shirtColor,
            Microsoft.Xna.Framework.Color underShirtColor,
            Microsoft.Xna.Framework.Color pantsColor,
            Microsoft.Xna.Framework.Color shoeColor, 
            int anglerQuests
        )
        {
            return NewCharacter
            (
                mode,
                auth,
                clientUUID,
                health,
                maxHealth,
                mana,
                maxMana,
                spawnX,
                spawnY,
                hair,
                hairDye,
                Tools.EncodeInteger(hideVisual),
                difficulty,
                Tools.EncodeColor(hairColor),
                Tools.EncodeColor(skinColor),
                Tools.EncodeColor(eyeColor),
                Tools.EncodeColor(shirtColor),
                Tools.EncodeColor(underShirtColor),
                Tools.EncodeColor(pantsColor),
                Tools.EncodeColor(shoeColor), 
                anglerQuests
            );
        }

        public static int NewCharacter
        (
            CharacterMode mode,
            string auth,
            string clientUUID,
            int health,
            int maxHealth,
            int mana,
            int maxMana,
            int spawnX,
            int spawnY,
            int hair,
            byte hairDye,
            int hideVisual,
            byte difficulty,
            uint hairColor,
            uint skinColor,
            uint eyeColor,
            uint shirtColor,
            uint underShirtColor,
            uint pantsColor,
            uint shoeColor, 
            int anglerQuests
        )
        {
            int? userId = null;
            if (mode == CharacterMode.AUTH)
            {
                var user = AuthenticatedUsers.GetUser(auth);
                userId = user.Value.Id;
            }
            else if (mode != CharacterMode.UUID)
                return 0;

            using (var bl = Storage.GetBuilder(CharacterManager.SQLSafeName))
            {
                bl.InsertInto(TableName, 
                    new DataParameter(ColumnNames.UserId, userId),
                    new DataParameter(ColumnNames.UUID, clientUUID),
                    new DataParameter(ColumnNames.Health, health),
                    new DataParameter(ColumnNames.MaxHealth, maxHealth),
                    new DataParameter(ColumnNames.Mana, mana),
                    new DataParameter(ColumnNames.MaxMana, maxMana),
                    new DataParameter(ColumnNames.SpawnX, spawnX),
                    new DataParameter(ColumnNames.SpawnY, spawnY),
                    new DataParameter(ColumnNames.Hair, hair),
                    new DataParameter(ColumnNames.HairDye, hairDye),
                    new DataParameter(ColumnNames.HideVisual, hideVisual),
                    new DataParameter(ColumnNames.Difficulty, difficulty),
                    new DataParameter(ColumnNames.HairColor, hairColor),
                    new DataParameter(ColumnNames.SkinColor, skinColor),
                    new DataParameter(ColumnNames.EyeColor, eyeColor),
                    new DataParameter(ColumnNames.ShirtColor, shirtColor),
                    new DataParameter(ColumnNames.UnderShirtColor, underShirtColor),
                    new DataParameter(ColumnNames.PantsColor, pantsColor),
                    new DataParameter(ColumnNames.ShoeColor, shoeColor),
                    new DataParameter(ColumnNames.AnglerQuests, anglerQuests)
                );

                return (int)Storage.ExecuteInsert(bl); //Get the new ID
            }
        }

        public static bool UpdateCharacter
        (
            CharacterMode mode,
            string auth,
            string clientUUID,
            int health,
            int maxHealth,
            int mana,
            int maxMana,
            int spawnX,
            int spawnY,
            int hair,
            byte hairDye,
            bool[] hideVisual,
            byte difficulty,
            Microsoft.Xna.Framework.Color hairColor,
            Microsoft.Xna.Framework.Color skinColor,
            Microsoft.Xna.Framework.Color eyeColor,
            Microsoft.Xna.Framework.Color shirtColor,
            Microsoft.Xna.Framework.Color underShirtColor,
            Microsoft.Xna.Framework.Color pantsColor,
            Microsoft.Xna.Framework.Color shoeColor, 
            int anglerQuests
        )
        {
            return UpdateCharacter
            (
                mode,
                auth,
                clientUUID,
                health,
                maxHealth,
                mana,
                maxMana,
                spawnX,
                spawnY,
                hair,
                hairDye,
                Tools.EncodeInteger(hideVisual),
                difficulty,
                Tools.EncodeColor(hairColor),
                Tools.EncodeColor(skinColor),
                Tools.EncodeColor(eyeColor),
                Tools.EncodeColor(shirtColor),
                Tools.EncodeColor(underShirtColor),
                Tools.EncodeColor(pantsColor),
                Tools.EncodeColor(shoeColor), 
                anglerQuests
            );
        }

        public static int GetCharacterId(CharacterMode mode, string auth, string clientUUID)
        {
            if (mode == CharacterMode.AUTH)
            {
                var user = AuthenticatedUsers.GetUser(auth);
                return  user.Value.Id;
            }
            else if (mode != CharacterMode.UUID)
                return 0;
            
            using (var bl = Storage.GetBuilder(CharacterManager.SQLSafeName))
            {
                bl.SelectFrom(TableName, new string[] { ColumnNames.Id }, 
                    new WhereFilter(ColumnNames.UUID, clientUUID)
                );

                return Storage.ExecuteScalar<Int32>(bl);
            }
        }

        public static bool UpdateCharacter
        (
            CharacterMode mode,
            string auth,
            string clientUUID,
            int health,
            int maxHealth,
            int mana,
            int maxMana,
            int spawnX,
            int spawnY,
            int hair,
            byte hairDye,
            int hideVisual,
            byte difficulty,
            uint hairColor,
            uint skinColor,
            uint eyeColor,
            uint shirtColor,
            uint underShirtColor,
            uint pantsColor,
            uint shoeColor, 
            int anglerQuests
        )
        {
            int? userId = null;
            if (mode == CharacterMode.AUTH)
            {
                var user = AuthenticatedUsers.GetUser(auth);
                userId = user.Value.Id;
            }
            else if (mode != CharacterMode.UUID)
                return false;
            
            using (var bl = Storage.GetBuilder(CharacterManager.SQLSafeName))
            {
                if (mode == CharacterMode.AUTH)
                {
                    bl.Update(TableName, new DataParameter[]
                        {
                            new DataParameter(ColumnNames.Health, health),
                            new DataParameter(ColumnNames.MaxHealth, maxHealth),
                            new DataParameter(ColumnNames.Mana, mana),
                            new DataParameter(ColumnNames.MaxMana, maxMana),
                            new DataParameter(ColumnNames.SpawnX, spawnX),
                            new DataParameter(ColumnNames.SpawnY, spawnY),
                            new DataParameter(ColumnNames.Hair, hair),
                            new DataParameter(ColumnNames.HairDye, hairDye),
                            new DataParameter(ColumnNames.HideVisual, hideVisual),
                            new DataParameter(ColumnNames.Difficulty, difficulty),
                            new DataParameter(ColumnNames.HairColor, hairColor),
                            new DataParameter(ColumnNames.SkinColor, skinColor),
                            new DataParameter(ColumnNames.EyeColor, eyeColor),
                            new DataParameter(ColumnNames.ShirtColor, shirtColor),
                            new DataParameter(ColumnNames.UnderShirtColor, underShirtColor),
                            new DataParameter(ColumnNames.PantsColor, pantsColor),
                            new DataParameter(ColumnNames.ShoeColor, shoeColor),
                            new DataParameter(ColumnNames.AnglerQuests, anglerQuests)
                        },
                        new WhereFilter(ColumnNames.UserId, userId.Value)
                    );
                }
                else
                {
                    bl.Update(TableName, new DataParameter[]
                        {
                            new DataParameter(ColumnNames.Health, health),
                            new DataParameter(ColumnNames.MaxHealth, maxHealth),
                            new DataParameter(ColumnNames.Mana, mana),
                            new DataParameter(ColumnNames.MaxMana, maxMana),
                            new DataParameter(ColumnNames.SpawnX, spawnX),
                            new DataParameter(ColumnNames.SpawnY, spawnY),
                            new DataParameter(ColumnNames.Hair, hair),
                            new DataParameter(ColumnNames.HairDye, hairDye),
                            new DataParameter(ColumnNames.HideVisual, hideVisual),
                            new DataParameter(ColumnNames.Difficulty, difficulty),
                            new DataParameter(ColumnNames.HairColor, hairColor),
                            new DataParameter(ColumnNames.SkinColor, skinColor),
                            new DataParameter(ColumnNames.EyeColor, eyeColor),
                            new DataParameter(ColumnNames.ShirtColor, shirtColor),
                            new DataParameter(ColumnNames.UnderShirtColor, underShirtColor),
                            new DataParameter(ColumnNames.PantsColor, pantsColor),
                            new DataParameter(ColumnNames.ShoeColor, shoeColor),
                            new DataParameter(ColumnNames.AnglerQuests, anglerQuests)
                        },
                        new WhereFilter(ColumnNames.UUID, clientUUID)
                    );
                }

                return Storage.ExecuteNonQuery(bl) > 0;
            }
        }

    }
}