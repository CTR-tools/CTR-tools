using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ctrviewer.Engine
{
    public class MenuItem
    {
        public string Title;
        public string Action;
        public string Param;
        public int Value;
        public bool Enabled;
        public float Width;
        public SwitchType sType;

        public int rangeval;
        public int rangemax;

        public MenuItem(string t, string a, string p, bool e, SwitchType st = SwitchType.None, int rmax = 0, int intValue = 0)
        {
            Title = t;
            Action = a;
            Param = p;
            Enabled = e;
            sType = st;
            rangeval = 0;
            rangemax = rmax;
            Value = intValue;
        }

        public void CalcWidth(SpriteFont font)
        {
            Width = font.MeasureString(Title).X;
        }
    }

}
