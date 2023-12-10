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
        public BoolType DisplayType { get; set; } = BoolType.YesNo;

        public string TrueText = "True";
        public string FalseText = "False";

        public new bool Value { get; set; } = false;

        public BoolMenuItem()
        {
            SetType(DisplayType);
        }

        private void SetType(BoolType boolType)
        {
            DisplayType = boolType;

            switch (boolType)
            {
                case BoolType.OnOff:
                    TrueText = Locale.Bool_On;
                    FalseText = Locale.Bool_Off;
                    break;

                case BoolType.TrueFalse:
                    TrueText = "True";
                    FalseText = "False";
                    break;

                case BoolType.EnabledDisabled:
                    TrueText = "Enabled";
                    FalseText = "Disabled";
                    break;

                case BoolType.YesNo:
                    TrueText = Locale.Bool_Yes;
                    FalseText = Locale.Bool_No;
                    break;

                case BoolType.Numeric:
                    TrueText = "1";
                    FalseText = "0";
                    break;

                default: throw new NotImplementedException("BoolMenuItem: Unimplemented BoolType.");
            }
        }

        private string BoolDisplayValue => Value ? TrueText : FalseText;

        public override void OnClick(object sender, EventArgs args = null)
        {
            //toggle value
            Value ^= true;

            //calculate new menu item width
            CalcWidth();

            //execute parent event
            base.OnClick(args);
        }

        public override string ToString() => $"{Text}: {BoolDisplayValue}";
    }
}