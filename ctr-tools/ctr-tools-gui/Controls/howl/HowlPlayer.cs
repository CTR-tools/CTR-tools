using CTRFramework.Audio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CTRTools.Controls
{
    public class SpuChannel
    {
        public int noteIndex = 0;
        public int nextNoteTimeout = 0;

        WaveOut output;
        AudioFileReader wave;
        SmbPitchShiftingSampleProvider pitchedSample;

        public SpuChannel(string filename, float volume = 1.0f, float pitch = 1.0f)
        {
            SetSample(filename);
            SetVolume(volume);
            SetPitch(pitch);
        }

        public void SetVolume(float volume)
        {
            output.Volume = volume;
        }

        public void SetSample(string filename)
        {
            wave = new AudioFileReader(filename);
            wave.ToSampleProvider();
            pitchedSample = new SmbPitchShiftingSampleProvider(wave);
        }

        public void SetPitch(float pitch)
        {
            if (pitchedSample == null) return;

            pitchedSample.PitchFactor = pitch;
        }

        public void Play(float pitch = 1.0f)
        {
            if (output == null) return;

            if (output.PlaybackState != PlaybackState.Stopped)
                output.Stop();

            SetPitch(pitch);

            output.Play();
        }

        public void Stop()
        {
            if (output == null) return;
            if (output.PlaybackState == PlaybackState.Stopped) return;

            output.Stop();
        }
    }

    public class SpuWannabe
    {
        public SpuChannel[] Channels = new SpuChannel[24];

        public SpuWannabe(Cseq cseq)
        {
            int i = 0;

            foreach (var sample in cseq.Instruments)
            {
                sample.Sample.SaveWav($"channel_{i}.wav", sample.Frequency);
                Channels[i] = new SpuChannel($"channel_{i}.wav");
                i++;
            }
        }

        public void Play(int channel, float pitch = 1.0f)
        {
            Channels[channel].SetPitch(pitch);
            Channels[channel].Play();
        }
    }

    // a standalone NAudio powered wave player.
    public class HowlPlayer
    {
        //howl context to use
        public static HowlContext Context;

        static AudioFileReader wave;
        static WaveOut output;
        static SpuInstrument LastPlayed = null;

        /// <summary>
        /// Plays the selected sound, given the instrument, hit and run style
        /// </summary>
        /// <param name="instrument"></param>
        public static void Play(SpuInstrument instrument)
        {
            if (instrument.Sample == null) return;

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

        public static float GetFreqFactor(int basefreq, int newNote, int octaveShift = 0) => basefreq * (float)Math.Pow(2, (newNote + octaveShift * 12 - 60) / 12f) / basefreq;


        static SpuWannabe player;

        public static void InitSong(Cseq seq)
        {
            player = new SpuWannabe(seq);
        }

        public static void PlaySong(Cseq seq, int songIndex = 0)
        {
            int ticker = 0;

            do
            {
                foreach (var track in seq.Songs[songIndex].Tracks)
                {
                    if (track.isDrumTrack) continue;


                }

                ticker++;
            }
            while (ticker < 10000);
        }

        public static Task PlayTrack(CancellationToken token, Cseq cseq, int songIndex = 0, int trackIndex = 0)
        {
            try
            {
                if (cseq.Songs.Count < songIndex) return null;
                if (cseq.Songs[songIndex].Tracks.Count < trackIndex) return null;

                var task = new Task(() =>
                {
                    var track = cseq.Songs[songIndex].Tracks[trackIndex];

                    if (track.isDrumTrack)
                    {
                        //cant play percussion tracks yet, so dont bother
                        return;
                    }

                    int curEventIndex = 0;

                    var curEvent = track.cseqEventCollection[curEventIndex];

                    int toNextEvent = curEvent.wait;

                    int c5freq = 11025;


                    foreach (var evt in track.cseqEventCollection)
                    {
                        if (evt.eventType == CseqEventType.ChangePatch)
                        {
                            c5freq = cseq.Instruments[evt.pitch].Frequency;

                            cseq.Instruments[evt.pitch].Sample.SaveWav(
                                $"channel_{trackIndex}.wav",
                                c5freq
                                );

                            //only check 1st entry
                            continue;
                        }
                    }


                    var wave = new AudioFileReader($"channel_{trackIndex}.wav");
                    wave.Position = 0;
                    wave.ToSampleProvider();

                    var pitchedSample = new SmbPitchShiftingSampleProvider(wave);
                    pitchedSample.PitchFactor = 1f;
                    pitchedSample.ToMono();

                    var outputevent = new WaveOutEvent();
                    outputevent.DesiredLatency = 150;
                    outputevent.Init(pitchedSample);

                    // var output = new WaveOut();
                    // output.Init(pitchedSample);

                    var tickDuration = new TimeSpan(5000);
                    var sw = new Stopwatch();


                    /*
                    using (var target = new AudioFileReader("render.wav"))
                    {
                        var mixer = new MixingSampleProvider(new[] { target, wave });
                        mixer.ToWaveProvider();

                        WaveFileWriter.CreateWaveFile("render.wav", mixer.ToWaveProvider());
                    }
                    */



                    do
                    {
                        if (token.IsCancellationRequested) return;

                        //wait tick
                        sw.Restart();

                        //process event while next event delay is 0
                        while (toNextEvent == 0)
                        {
                            //maybe current note is a noteon? then trigger a note
                            if (curEvent.eventType == CseqEventType.NoteOn)
                            {
                                wave.Position = 0;

                                pitchedSample.PitchFactor = GetFreqFactor(c5freq, curEvent.pitch, 0);
                                outputevent.Play();
                            }

                            //stop the note if its noteoff event
                            if (curEvent.eventType == CseqEventType.NoteOff)
                            {
                                // output.Stop();
                            }

                            //shift to next event
                            curEventIndex++;
                            curEvent = track.cseqEventCollection[curEventIndex];

                            toNextEvent = curEvent.wait;
                        }

                        while (sw.ElapsedMilliseconds <= 2) { }
                        sw.Stop();

                        //decrease event
                        toNextEvent -= 1;
                    }
                    while (curEventIndex < track.cseqEventCollection.Count);

                    MessageBox.Show("play end!");
                });

                task.Start();

                return task;
            }
            catch (Exception ex)
            {
                MessageBox.Show("asd" + ex.Message);
                return null;
            }

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