using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tdsm.api.Misc;

namespace tdsm.api
{
    public class ScheduledNotification : Task
    {
        private new Action<Task> Method { get; set; }

        private string _message;
        private Color _colour;

        public bool ConsoleOnly { get; set; }

        public ScheduledNotification(string message, Color colour, int seconds)
        {
            _message = message;
            _colour = colour;
            base.Trigger = seconds;
            base.Method = (tsk) =>
            {
                if (ConsoleOnly) Tools.WriteLine(_message);
                else Tools.NotifyAllPlayers(_message, _colour);
            };
            Tasks.Schedule(this);
        }
    }
}
