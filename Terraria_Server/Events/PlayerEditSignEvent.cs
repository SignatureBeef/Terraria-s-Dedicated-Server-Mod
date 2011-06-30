using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Events
{
    public class PlayerEditSignEvent : PlayerDestroySignEvent
    {
        private string text;

        public string getText()
        {
            return text;
        }

        public void setText(string Text)
        {
            text = Text;
        }
    }
}
