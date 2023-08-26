using System;
using System.IO;
using NAudio.Wave;
using CTRFramework.Sound;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace CTRTools.Controls
{
    public class HowlPlayer
    {
        //howl context to use
        public static HowlContext Context;

        static AudioFileReader wave;
        static WaveOut output;
        static Instrument LastPlayed = null;

        /// <summary>
        /// Plays the selected sound, given the instrument, hit and run style
        /// </summary>
        /// <param name="instrument"></param>
        public static void Play(Instrument instrument)
        {
            //little caching if trying to play the same sample in a row
            if (LastPlayed == instrument)
            {
                LastPlayed = instrument;
                output.Stop();
                output.Play();
                return;
            }

            Reset();

            var sample = instrument.GetVagSample(Context);

            //fix this to use in-mem
            File.Delete("temp.wav");
            sample.ExportWav("temp.wav");

            wave = new AudioFileReader("temp.wav");
            output = new WaveOut();

            output.Volume = instrument.Volume;
            output.Init(wave);
            output.Play();
        }

        /// <summary>
        /// Resets the player state to be used by the next sample
        /// </summary>
        public static void Reset()
        {
            if (output != null)
            {
                if (output.PlaybackState != PlaybackState.Stopped) output.Stop();
                wave.Close();
                wave = null;
            }
        }

    }
}