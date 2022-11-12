using System;

namespace ctrviewer.Engine.Gui
{
    public class MenuItem
    {
        public string Name;
        public string Text;
        public string Action;
        public string Param;
        public int Value;
        public bool Enabled = true;
        public float Width;

        #region [events]
        public event EventHandler Click;
        public virtual void OnClick(object sender, EventArgs args = null) => Click?.Invoke(this, args);
        public void DoClick() { if (Enabled) OnClick(this); }

        public event EventHandler PressedLeft;
        public virtual void OnPressedLeft(object sender, EventArgs args = null) => PressedLeft?.Invoke(this, args);
        public void PressLeft() { if (Enabled) OnPressedLeft(this); }

        public event EventHandler PressedRight;
        public virtual void OnPressedRight(object sender, EventArgs args = null) => PressedRight?.Invoke(this, args);
        public void PressRight() { if (Enabled) OnPressedRight(this); }
        #endregion

        public MenuItem()
        {
        }

        public MenuItem(string t, string a, string p, bool e = true, int intValue = 0)
        {
            Text = t;
            Action = a;
            Param = p;
            Enabled = e;
            Value = intValue;
        }

        public void CalcWidth() => Width = Menu.Font.MeasureString(ToString()).X;

        public override string ToString() => Text.ToUpper();
    }
}