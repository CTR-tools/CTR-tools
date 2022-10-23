using System;
using System.Collections.Generic;

namespace ctrviewer.Engine.Gui
{
    public class IntRangeMenuItem : MenuItem
    {
        public List<(int value, string label)> Values = new List<(int value, string label)>();

        public int SelectedValue = 0;

        public new int Value => Values[SelectedValue].value;
        public string Label => Values[SelectedValue].label;

        public IntRangeMenuItem()
        {
        }

        public void ChangeValue(int change)
        {
            SelectedValue += change;

            if (SelectedValue < 0)
                SelectedValue = Values.Count - 1;

            if (SelectedValue > Values.Count - 1)
                SelectedValue = 0;
        }

        public override void OnClick(object sender, EventArgs args = null)
        {
            ChangeValue(1);
            CalcWidth();
            base.OnClick(args);
        }

        public override string ToString() => $"{Text}: << {Label} >>".ToUpper();
    }
}