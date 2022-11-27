using System;
using Locale = ctrviewer.Resources.Localization;

namespace ctrviewer.Engine.Gui
{
    public enum BoolType
    {
        OnOff,
        YesNo,
        EnabledDisabled,
        Numeric,
        TrueFalse
    }

    public class BoolMenuItem : MenuItem
    {
        public BoolType DisplayType = BoolType.YesNo;

        public new bool Value { get; set; } = false;

        private string BoolDisplayValue()
        {
            switch (DisplayType)
            {
                case BoolType.OnOff: return Value ? Locale.Bool_On : Locale.Bool_Off;
                case BoolType.TrueFalse: return Value ? "True" : "False";
                case BoolType.EnabledDisabled: return Value ? "Enabled" : "Disabled";
                case BoolType.YesNo: return Value ? Locale.Bool_Yes : Locale.Bool_No;
                case BoolType.Numeric: return Value ? "1" : "0";
                default: throw new NotImplementedException("BoolMenuItem: Unimplemented BoolType.");
            }
        }

        public override void OnClick(object sender, EventArgs args = null)
        {
            Value ^= true;
            CalcWidth();
            base.OnClick(args);
        }

        public override string ToString() => $"{Text}: {BoolDisplayValue()}";
    }
}