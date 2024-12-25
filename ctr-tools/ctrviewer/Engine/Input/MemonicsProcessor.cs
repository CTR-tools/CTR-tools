using ctrviewer.Engine.Menu;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace ctrviewer.Engine.Input
{
    //TODO: match enum to actual game values
    public enum Cheat
    {
        CharRoo = 1 << 0,
        CharPapu = 1 << 1,
        CharJoe = 1 << 2,
        CharPinstripe = 1 << 3,
        CharTropy = 1 << 4,
        CharPenta = 1 << 5,
        CharFake = 1 << 6,
        CharOxide = 1 << 7,

        InfiniteBombs = 1 << 8,
        InfiniteRockets = 1 << 9,
        InfiniteMasks = 1 << 10,
        TurboCounter = 1 << 11,
        IcyTracks = 1 << 12,
        Invisibility = 1 << 13,
        SuperTurboPads = 1 << 14,
        Scrapbook = 1 << 15,
        SpyroDemo = 1 << 16,
        MaxWumpa = 1 << 17,
        TrackRewards = 1 << 18,
        SuperHard = 1 << 19,
        UnlimitedTurbos = 1 << 20,
        NoMissiles = 1 << 21,
        OneLapRaces = 1 << 22,
        SuperEngine = 1 << 23
    }

    /// <summary>
    /// Intended to process cheat input string and toggle cheat flag if it matches the checksum.
    /// Original mnemonics derives from Gex 2 by Danny Chan, but also modified a bit, since original one used nintendo ABXY notation + start.
    /// 
    /// Up = U or N for north
    /// Down = D or S for south
    /// Left = L or W for west
    /// Right = R or E for east
    /// X for X, O for circle, A for triangle
    /// 
    /// Available letters: ADELNORSUWX
    /// can also also seamlessly throw in C for circle, but it wasnt in the original game
    /// </summary>
    public class MemonicsProcessor
    {

        /// <summary>
        /// This is a list of buttons allowed for the mnemonics system.
        /// As a side effect, pressing other buttons won't affect the proper sequence, maybe better treat it as reset?
        /// </summary>
        private readonly List<Buttons> AllowedCheatButtons = new List<Buttons>() {
            Buttons.DPadUp, //UN
            Buttons.DPadDown, //DS
            Buttons.DPadLeft, //LW
            Buttons.DPadRight, //RE
            Buttons.A, //A
            Buttons.B, //O
            //Buttons.X,
            Buttons.Y //A
        };

        private const uint defaultState = 0xFFFFFFFF;

        /// <summary>
        /// A single uint to hold states for 32 cheats.
        /// </summary>
        private uint CheatState = 0;

        /// <summary>
        /// Input sequence checksum, tested against precalculated mnemonic value.
        /// </summary>
        private uint Value = defaultState;

        /// <summary>
        /// Returns current cheat state. Exposed for the outside users.
        /// </summary>
        /// <param name="cheat"></param>
        /// <returns></returns>
        public bool CheatUnlocked(Cheat cheat) => (CheatState & (uint)cheat) > 0;

        /// <summary>
        /// Toggles current cheat state. Only can toggle within the class.
        /// </summary>
        /// <param name="cheat"></param>
        private void ToggleCheat(Cheat cheat)
        {
            CheatState ^= (uint)cheat;
            //GameConsole.Write($"{cheat} state now: {CheatUnlocked(cheat)}");

            //send a message to the UI, since we got a state change
            FrontendMessage.SendMessage("cheat_unlock", $"Cheat {(CheatUnlocked(cheat) ? "enabled" : "disabled")}: {cheat}", 10, 10, 5);

            //and reset the input
            Reset();
        }

        /// <summary>
        /// Processes the next input in the sequence and updates the checksum. Formula is just something.
        /// Maybe add a calculation func so can avoid hardcoded checksums at all?
        /// </summary>
        /// <param name="button"></param>
        private void NextInput(Buttons button)
        {
            //absolutely arbitrary formula to scramble the checksum value
            //the way if works now, highest digit is always F
            Value = (Value >> 1) ^ (uint)((0x76281493 ^ ((((int)button + 100) * 32) ^ Value)));

            //copy checksum to clipboard
            //Clipboard.SetText(Value.ToString("X8"));

            //GameConsole.Write($"mnemonic value now: {Value.ToString("X8")}\tpressed: {button}");

            //check if we can unlock something
            MaybeUnlockStuff(Value);
        }

        /// <summary>
        /// Resets the checksum to the initial state. Public since might want to reset by outside events.
        /// </summary>
        public void Reset()
        {
            if (Value != defaultState)
            {
                Value = defaultState;
                //GameConsole.Write("reset mnemonic input!");
            }
        }

        /// <summary>
        /// Checks whether any of allowed cheat buttons is pressed. To be called in the parent update class.
        /// </summary>
        public void Update()
        {
            foreach (var button in AllowedCheatButtons)
            {
                if (GamePadHandler.IsPressed(button))
                {
                    NextInput(button);
                    break;
                }
            }
        }

        /// <summary>
        /// Unlocks stuff if mnemonic sequence resulted in valid checksum.
        /// </summary>
        private void MaybeUnlockStuff(uint value)
        {
            //might be a good idea to refactor it to a dictionary<uint, cheat>. less messy.
            switch (value)
            {
                //ROODUDE:      Right, Circle, Circle, Down, Up, Down, Right
                case 0xF17A5059: ToggleCheat(Cheat.CharRoo); break;
                //LARDROLLS:    Left, Triangle, Right, Down, Right, Circle, Left, Left, Down
                case 0xF62A0178: ToggleCheat(Cheat.CharPapu); break;
                //DOLLARS:      Down, Circle, Left, Left, Triangle, Right, Down
                case 0xF16A1120: ToggleCheat(Cheat.CharJoe); break;
                //WEASES (LEASED or LEADED also matches): Left, Right, Triangle, Down, Right, Down
                case 0xFA7E4E1F: ToggleCheat(Cheat.CharPinstripe); break;
                //ODDNOODLE - fake crash: Circle, Down, Down, Up, Circle, Circle, Down, Left, Right
                case 0xF635A432: ToggleCheat(Cheat.CharFake); break;
                //SLENDER - tropy: Down, Left, Right, Up, Down, Right, Right
                case 0xF17870C0: ToggleCheat(Cheat.CharTropy); break;
                //SEASWAN - penta: Down, Right, Triangle, Down, Left, Triangle, Up
                case 0xF1717048: ToggleCheat(Cheat.CharPenta); break;
                //UNDERWEAR:    Up, Up, Down, Right, Right, Left, Right, Triangle, Right
                case 0xF62A61A9: ToggleCheat(Cheat.Scrapbook); break;
                //REWARDS:  Right, Right, Left, Triangle, Right, Down, Down
                case 0xF1667009: ToggleCheat(Cheat.TrackRewards); break;
                //SOAR: Down, Circle, Triangle, Right
                case 0xFED8027A: ToggleCheat(Cheat.SpyroDemo); break;
                //SEEDS:    Down Right Right Down Down
                case 0xF78F9A54: ToggleCheat(Cheat.MaxWumpa); break;
                //WARLORDS:	Left, Triangle, Right, Left, Circle, Right, Down, Down 
                case 0xFFFE9051: ToggleCheat(Cheat.InfiniteMasks); break;
                //ARSENAL:  Triangle, Right, Down, Right, Up, Triangle, Left
                case 0xF17531A9: ToggleCheat(Cheat.InfiniteBombs); break;
                //ADDON:    Triangle, Down, Down, Circle, Up
                case 0xF7989A98: ToggleCheat(Cheat.TurboCounter); break;
                //ARROW:    Triangle, Right, Right, Circle, Left
                case 0xF7989BB0: ToggleCheat(Cheat.SuperTurboPads); break;
                //UNSEEN:   Up, Up, Down, Right, Right, Up 
                case 0xFA604F42: ToggleCheat(Cheat.Invisibility); break;
                //AXELWAX:   Triangle, X, Right, Left, Left, Triangle, X
                case 0xF17401DD: ToggleCheat(Cheat.UnlimitedTurbos); break;
                //SLEDROAD: Down, Left, Right, Down, Right, Circle, Triangle, Down 
                case 0xFFF151CD: ToggleCheat(Cheat.IcyTracks); break;
                //DELUXE:   Down, Right, Left, Up, X, Right
                case 0xFA634E85: ToggleCheat(Cheat.SuperHard); break;
                //ENDURO/ENDNEO:   Right, Up, Down, Up, Right, Circle
                case 0xFA644FA1: ToggleCheat(Cheat.NoMissiles); break;
                //SUDDENDEAD:  Down, Up, Down, Down, Right, Up, Down, Right, Triangle, Down
                case 0xFB1B489D: ToggleCheat(Cheat.OneLapRaces); break;
                //UNLEADED: Up, Up, Left, Right, Triangle, Down, Right, Down 
                case 0xFFF251F1: ToggleCheat(Cheat.SuperEngine); break;
            }
        }
    }
}
