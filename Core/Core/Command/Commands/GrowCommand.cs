using System;
using OTA.Command;
using Terraria;

namespace TDSM.Core.Command.Commands
{
    public class GrowCommand : CoreCommand
    {
        public override void Initialise()
        {
            Core.AddCommand("grow")
                .WithAccessLevel(AccessLevel.OP)
                .WithPermissionNode("tdsm.grow")
                .Calls((ISender sender, ArgumentList args) =>
                {
                    if (null == WorldGen.genRand) WorldGen.genRand = new Random();
                    var ply = sender as Player;
                    int tileX = (int)(ply.position.X / 16f), tileY = (int)((ply.position.Y + ply.height) / 16f);

                    if (args.TryPop("-alch")) WorldGen.GrowAlch(tileX, tileY);
                        //                    else if (args.TryPop("-cactus")) WorldGen.GrowCactus(tileX, tileY);
                        else if (args.TryPop("-epictree")) WorldGen.GrowEpicTree(tileX, tileY);
                        //                    else if (args.TryPop("-livingtree")) WorldGen.GrowLivingTree(tileX, tileY);
                        else if (args.TryPop("-palmtree")) WorldGen.GrowPalmTree(tileX, tileY);
                        //                    else if (args.TryPop("-pumpkin")) WorldGen.GrowPumpkin(tileX, tileY);
                        else if (args.TryPop("-shroom")) WorldGen.GrowShroom(tileX, tileY);
                        //                    else if (args.TryPop("-spike")) WorldGen.GrowSpike(tileX, tileY);
                        else if (args.TryPop("-tree") || args.TryPop("-t")) WorldGen.GrowTree(tileX, tileY);
                    else if (args.TryPop("-tree")) WorldGen.GrowTree(tileX, tileY);
                    else if (args.TryPop("-grass") || args.TryPop("-g")) WorldGen.SpreadGrass(tileX, tileY);
                    else if (args.TryPop("-undergroundtree")) WorldGen.GrowUndergroundTree(tileX, tileY);
                    else throw new CommandError("Element not supported");

                    NetMessage.SendTileSquare(ply.whoAmI, (int)(ply.position.X / 16), (int)(ply.position.Y / 16), 32);
                });
        }
    }
}

