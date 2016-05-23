using System;
using OTA;
using OTA.Command;
using Terraria;

namespace TDSM.Core.Command.Commands
{
    public class VarCommand : CoreCommand
    {
        public override void Initialise()
        {
            AddCommand("var")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Experimental variable manipulation")
                .WithHelpText("<field|exec|prop> <namespace.classname> <fieldname|methodname>")
                .WithHelpText("field Terraria.Main eclipse          #Get the value")
                .WithHelpText("field Terraria.Main eclipse false    #Set the value")
                .WithPermissionNode("tdsm.var")
                .Calls(this.VariableMan);
        }

        private static object GetDataValue(Type dataType, string val)
        {
            switch (dataType.Name)
            {
                case "Boolean":
                    return Boolean.Parse(val);
                case "Int16":
                    return Int16.Parse(val);
                case "Int32":
                    return Int32.Parse(val);
                case "Int64":
                    return Int64.Parse(val);
                case "Byte":
                    return Byte.Parse(val);
                case "Double":
                    return Double.Parse(val);
                case "Single":
                    return Single.Parse(val);
                case "String":
                    return val;
                default:
                    throw new CommandError("Unsupported datatype");
            }
        }

        /// <summary>
        /// Allows on the fly variable modifications
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="args">Arguments.</param>
        public void VariableMan(ISender sender, ArgumentList args)
        {
            // var <exec|field>
            // var field <namespace.type> <fieldname>
            // var field <namespace.type> <fieldname> <valuetobeset>

            // var exec <namespace.type> <methodname>
            //No arguments supported yet
            var cmd = args.GetString(0);

            if (cmd == "field" || cmd == "arr")
            {
                var type = args.GetString(1);
                var mem = args.GetString(2);

                //Find the type
                var at = Type.GetType(type);
                if (at == null)
                    at = System.Reflection.Assembly.GetEntryAssembly().GetType(type);
                if (at == null)
                    at = System.Reflection.Assembly.GetCallingAssembly().GetType(type);
                if (at == null)
                    at = System.Reflection.Assembly.GetExecutingAssembly().GetType(type);
                if (at == null)
                    at = typeof(Terraria.Main).Assembly.GetType(type);
                if (at == null)
                    throw new CommandError("Invalid type: " + type);

                //Find the field
                var am = at.GetField(mem);
                if (am == null)
                    throw new CommandError("Invalid field: " + mem);

                string val = null;
                if (cmd != "arr" && args.TryGetString(3, out val))
                {
                    object data = GetDataValue(am.FieldType, val);
                    am.SetValue(null, data);

                    var v = am.GetValue(null);
                    if (v != null)
                        val = v.ToString();
                    else
                        val = "null";
                    sender.Message("Value is now: " + val);
                }
                else
                {
                    var v = am.GetValue(null);
                    if (v != null)
                    {
                        if (v.GetType().IsArray)
                        {
                            var index = args.GetInt(3);
                            var obj = (v as Array).GetValue(index);
                            if (obj is Terraria.RemoteClient)
                            {
                                val = "";
                                var rc = obj as Terraria.RemoteClient;

                                val += $"Id: {rc.Id}\n";
                                val += $"IsActive: {rc.IsActive}\n";
                                val += $"IsAnnouncementCompleted: {rc.IsAnnouncementCompleted}\n";
                                val += $"IsConnected(): {rc.IsConnected()}\n";
                                val += $"IsReading: {rc.IsReading}\n";
                                val += $"Name: {rc.Name}\n";
                                val += $"PendingTermination: {rc.PendingTermination}\n";
                                val += $"SpamAddBlock: {rc.SpamAddBlock}\n";
                                val += $"SpamAddBlockMax: {rc.SpamAddBlockMax}\n";
                                val += $"SpamDeleteBlock: {rc.SpamDeleteBlock}\n";
                                val += $"SpamDeleteBlockMax: {rc.SpamDeleteBlockMax}\n";
                                val += $"SpamProjectile: {rc.SpamProjectile}\n";
                                val += $"SpamProjectileMax: {rc.SpamProjectileMax}\n";
                                val += $"SpamWater: {rc.SpamWater}\n";
                                val += $"SpamWaterMax: {rc.SpamWaterMax}\n";
                                val += $"State: {rc.State}\n";
                                val += $"StatusCount: {rc.StatusCount}\n";
                                val += $"StatusMax: {rc.StatusMax}\n";
                                val += $"StatusText: {rc.StatusText}\n";
                                val += $"StatusText2: {rc.StatusText2}\n";
                                val += $"TimeOutTimer: {rc.TimeOutTimer}\n";

#if CUSTOM_SOCKETS
                                if (rc.Socket is OTA.Sockets.ClientConnection)
                                {
                                    val += "\n=======================\nClientConnection\n";
                                    var cc = rc.Socket as OTA.Sockets.ClientConnection;
                                    val += $"Active: {cc.Active}\n";
                                    val += $"BytesReceived: {cc.BytesReceived}\n";
                                    val += $"BytesSent: {cc.BytesSent}\n";
                                    val += $"IsOTAClient: {cc.IsOTAClient}\n";
                                    val += $"QueueLength: {cc.QueueLength}\n";
                                    val += $"QueueSize: {cc.QueueSize}\n";
                                    //val += $"RemoteAddress: {cc.RemoteAddress}\n";
                                    val += $"SlotId: {cc.SlotId}\n";
                                }
#endif
                            }
                            else val = obj.ToString();
                        }
                        else val = v.ToString();
                    }
                    else
                        val = "null";
                    sender.Message("Value: " + val);
                }
            }
            else if (cmd == "prop")
            {
                var type = args.GetString(1);
                var prop = args.GetString(2);

                //Find the type
                var at = Type.GetType(type);
                if (at == null)
                    at = System.Reflection.Assembly.GetEntryAssembly().GetType(type);
                if (at == null)
                    at = System.Reflection.Assembly.GetCallingAssembly().GetType(type);
                if (at == null)
                    at = System.Reflection.Assembly.GetExecutingAssembly().GetType(type);
                if (at == null)
                    throw new CommandError("Invalid type: " + type);

                //Find the field
                var am = at.GetProperty(prop);
                if (am == null)
                    throw new CommandError("Invalid property: " + prop);

                string val = null;
                if (args.TryGetString(3, out val))
                {
                    object data = GetDataValue(am.PropertyType, val);
                    am.SetValue(null, data, null);

                    var v = am.GetValue(null, null);
                    if (v != null)
                        val = v.ToString();
                    else
                        val = "null";
                    sender.Message("Value is now: " + val);
                }
                else
                {
                    var v = am.GetValue(null, null);
                    if (v != null)
                        val = v.ToString();
                    else
                        val = "null";
                    sender.Message("Value: " + val);
                }
            }
            else if (cmd == "exec")
            {
                var type = args.GetString(1);
                var mthd = args.GetString(2);

                //Find the type
                var at = Type.GetType(type);
                if (at == null)
                    at = System.Reflection.Assembly.GetEntryAssembly().GetType(type);
                if (at == null)
                    at = System.Reflection.Assembly.GetCallingAssembly().GetType(type);
                if (at == null)
                    at = System.Reflection.Assembly.GetExecutingAssembly().GetType(type);
                if (at == null)
                    throw new CommandError("Invalid type: " + type);

                //Find the field
                var am = at.GetMethod(mthd);
                if (am == null)
                    throw new CommandError("Invalid method: " + mthd);

                var prms = am.GetParameters();
                if (prms.Length == 0)
                {
                    var res = am.Invoke(null, null);
                    var result = res == null ? "null" : res.ToString();
                    sender.Message("Result: " + result);
                }
                else
                    sender.Message("Arguments are not yet supported for exec");
            }
            else
                sender.Message("Unsupported var command: " + cmd);
        }
    }
}

