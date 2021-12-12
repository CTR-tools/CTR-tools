namespace ctrviewer.Engine.Gui
{
    public class IntRangeMenuItem : MenuItem
    {
        public int Value { get; set; } = 0;

        public int MinValue = 0;
        public int MaxValue = 10;

        public IntRangeMenuItem(int InitValue)
        {
            Value = InitValue;
        }

        public void ChangeValue(int change)
        {
            Value += change;

            if (Value < MinValue)
                Value = MaxValue;

            if (Value > MaxValue)
                Value = MinValue;
        }

        public override string ToString()
        {
            return $"{Text}: << {Value} >>";
        }
    }
}