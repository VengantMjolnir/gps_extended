using VRage.Game.Components;
using Sandbox.ModAPI;
using System.Text;
using Sandbox.ModAPI.Interfaces.Terminal;
using System.Collections.Generic;
using System;
using VRage.Game.ModAPI;
using VRage.Game;
using Sandbox.Game.Entities;
using System.Linq;
using VRage.Utils;
using Sandbox.Game;
using Sandbox.Game.Screens.Terminal.Controls;
using Sandbox.Game.EntityComponents;
using VRage.ModAPI;
using VRage.Game.Entity;
using VRageMath;

namespace GPS_ExtendedMod
{
    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
    public class GPSExtendedSession : MySessionComponentBase
    {
        private bool consoleMode = false;
        string ON = "ON";
        string OFF = "OFF";

        public override void BeforeStart()
        {
            base.BeforeStart();

            MyAPIGateway.Utilities.MessageEnteredSender += OnMessageEntered;
        }

        
        protected override void UnloadData()
        {
            MyAPIGateway.Utilities.MessageEnteredSender -= OnMessageEntered;

            base.UnloadData();
        }

        private void OnMessageEntered(ulong sender, string messageText, ref bool sendToOthers)
        {
            try
            {
                if (messageText == null)
                {
                    return;
                }
                if (messageText.StartsWith("/xgps"))
                {
                    IMyPlayer player = MyAPIGateway.Session.LocalHumanPlayer;
                    if (player != null)
                    {
                        string[] args = messageText.Split(' ');
                        if (args.Length <= 2)
                        {
                            Echo("GPS Extended requires 2 arguments <distance> and <name>. Distance is in KM");
                            sendToOthers = false;
                            return;
                        }
                        float distance = float.Parse(args[1]) * 1000f;
                        string[] message_bits = new string[args.Length - 2];
                        Array.Copy(args, 2, message_bits, 0, message_bits.Length);
                        string name = string.Join(" ", message_bits);
                        if (name == "")
                        {
                            name = player.DisplayName;
                        }
                        Vector3D position = player.GetPosition();
                        position += MyAPIGateway.Session.Camera.WorldMatrix.Forward * distance;
                        IMyGps gps = MyAPIGateway.Session.GPS.Create(name, "", position, true, false);
                        MyAPIGateway.Session.GPS.AddGps(player.IdentityId, gps);

                        sendToOthers = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Echo("An error occurred, check SE logs for details. (Please ensure syntax is: /xgps <distance> <name>)");
                MyLog.Default.WriteLineAndConsole($"GPS_EXTENDED [ERROR]: \nMessage:{ex.Message}\n{ex.StackTrace}");
            }
        }

        private void Echo(string message)
        {
            MyAPIGateway.Utilities.ShowMessage("GPS_Extended", message);
        }

    }
}
