using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFlat.Utility
{
    public class Activity
    {
        public PlayerBase player { get; set; }
        public PlaygroundBase playground { get; set; }

        public void act()
        {
            player.play(playground);
        }
    }
}
