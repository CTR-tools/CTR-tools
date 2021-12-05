using System;

namespace ctrviewer.Engine.Gui
{
    public enum BoolType
    {
        OnOff,
        YesNo,
        EnabledDisabled,
        Number,
        TrueFalse
    }

    public class BoolMenuItem : MenuItem
    {
        public override void OnClicked(object sender, EventArgs args = null)
        {
            Value ^= true;
            CalcWidth();
            base.OnClicked(args);
        }

        private bool _value = false;

        public bool Value
        {
            get { return _value; }
            set
            {
                _value = value;
            }
        }

        public BoolType DisplayType = BoolType.OnOff;

        private string BoolDisplayValue()
        {
            switch (DisplayType)
            {
                case BoolType.OnOff: return Value ? "On" : "Off";
                case BoolType.TrueFalse: return Value ? "True" : "False";
                case BoolType.EnabledDisabled: return Value ? "Enabled" : "Disabled";
                case BoolType.YesNo: return Value ? "Yes" : "No";
                case BoolType.Number: return Value ? "1" : "0";
                default: throw new NotImplementedException("BoolMenuItem: Unimplemented DisplayType.");
            }
        }

        public BoolMenuItem(bool InitValue)
        {
            _value = InitValue;
        }

        public override string ToString()
        {
            return $"{Text}: {BoolDisplayValue()}".ToUpper();
        }
    }
}