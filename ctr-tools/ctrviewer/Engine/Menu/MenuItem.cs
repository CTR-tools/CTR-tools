using Microsoft.Xna.Framework.Graphics;
using System;

namespace ctrviewer.Engine.Gui
{
    public class MenuItem
    {
        public event EventHandler Click;
        public virtual void OnClicked(object sender, EventArgs args = null) => Click?.Invoke(this, args);

        public string Name;
        public string Text;
        public string Action;
        public string Param;
        public int Value;
        public bool Enabled = true;
        public float Width;
        public SwitchType sType;

        public int rangeval;
        public int rangemax;

        public MenuItem()
        {
        }

        public MenuItem(string t, string a, string p, bool e = true, SwitchType st = SwitchType.None, int rmax = 0, int intValue = 0)
        {
            Text = t;
            Action = a;
            Param = p;
            Enabled = e;
            sType = st;
            rangeval = 0;
            rangemax = rmax;
            Value = intValue;
        }

        public void CalcWidth()
        {
            Width = Menu.Font.MeasureString(ToString()).X;
        }

        public override string ToString()
        {
            return Text.ToUpper();
        }

        public void DoClick()
        {
            OnClicked(this);
        }
    }
}
