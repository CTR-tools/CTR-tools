using CTRFramework.Shared;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;

namespace ctrviewer.Engine.Render
{
    public class AnimationKey
    {
        public float Time;
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;

        public static AnimationKey Lerp(AnimationKey from, AnimationKey to, float amount)
        {
            return new AnimationKey
            {
                Position = Vector3.Lerp(from.Position, to.Position, amount),
                Rotation = Vector3.Lerp(from.Rotation, to.Rotation, amount),
                Scale = Vector3.Lerp(from.Scale, to.Scale, amount),
                Time = MathHelper.Lerp(from.Time, to.Time, amount)
            };
        }

        public AnimationKey()
        {
        }

        public AnimationKey(BinaryReaderEx br) => Read(br);

        public static AnimationKey FromReader(BinaryReaderEx br) => new AnimationKey(br);

        public void Read(BinaryReaderEx br)
        {
            Time = br.ReadSingle();
            Position = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            Rotation = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            Scale = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
        }

        public void Write(BinaryWriterEx bw)
        {
            bw.Write(Time);

            bw.Write(Position.X);
            bw.Write(Position.Y);
            bw.Write(Position.Z);

            bw.Write(Rotation.X);
            bw.Write(Rotation.Y);
            bw.Write(Rotation.Z);

            bw.Write(Scale.X);
            bw.Write(Scale.Y);
            bw.Write(Scale.Z);
        }
    }

    public class SimpleAnimation : List<AnimationKey>
    {
        public SimpleAnimation()
        {
        }

        public SimpleAnimation(BinaryReaderEx br) => Read(br);

        public static SimpleAnimation FromFile(string filename)
        {
            using (var br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                return FromReader(br);
            }
        }

        public static SimpleAnimation FromReader(BinaryReaderEx br) => new SimpleAnimation(br);

        public void Read(BinaryReaderEx br)
        {
            if (new string(br.ReadChars(4)) != "ANIM")
            {
                Helpers.Panic(this, PanicType.Error, "not an animation file");
                return;
            }

            int numKeys = br.ReadInt32();

            for (int i = 0; i < numKeys; i++)
                Add(AnimationKey.FromReader(br));
        }

        public void Write(BinaryWriterEx bw)
        {
            bw.Write("ANIM".ToCharArray());
            bw.Write(Count);

            foreach (var key in this)
                key.Write(bw);
        }

        public void Save(string filename)
        {
            using (var bw = new BinaryWriterEx(File.OpenWrite(filename)))
            {
                Write(bw);
            }
        }
    }

    public class AnimationPlayer
    {
        public bool IsPlaying = false;

        public SimpleAnimation Animation;

        public int NumFrames => Animation is null ? 0 : Animation.Count;

        public float Speed = 2f;

        public float Timer = 0f;

        public AnimationKey State;

        public AnimationPlayer(SimpleAnimation anim)
        {
            if (anim is null)
            {
                GameConsole.Write("null anim passed to player");
                return;
            }

            Animation = anim;
            State = anim[0];
        }

        public AnimationPlayer()
        {
            Animation.Add(new AnimationKey() { Position = new Vector3(0, 0, 0), Rotation = new Vector3(0, 0, 0), Scale = new Vector3(1), Time = 0 });
            Animation.Add(new AnimationKey() { Position = new Vector3(0, 0, 0), Rotation = new Vector3(0, 0, 0), Scale = new Vector3(1), Time = 2000 });

            State = Animation[0];
        }

        private int _thisframe;
        private int _nextframe => _thisframe + 1;

        private float NextFrameTime => NextFrame.Time * Speed;
        private float ThisFrameTime => ThisFrame.Time * Speed;

        private float FrameDuration => (NextFrame.Time - ThisFrame.Time) / Speed;

        private AnimationKey ThisFrame => Animation[_thisframe];
        private AnimationKey NextFrame => _nextframe < Animation.Count ? Animation[_nextframe] : null ;

        public static AnimationPlayer Create(string animName, bool run = false, float speed = 1f) => new AnimationPlayer(ContentVault.GetVectorAnim(animName)) { Speed = speed, IsPlaying = run };

        public void Load(string animName)
        {
            Animation = ContentVault.GetVectorAnim(animName);
        }

        public void Run() => IsPlaying = true;

        public void Pause() => IsPlaying = false;

        public void Advance(GameTime gameTime)
        {
            if (!IsPlaying) return;
            if (Animation is null) return;
            if (NumFrames == 0) return;

            Timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            
            if (Timer >= FrameDuration)
            {
                Timer -= FrameDuration;
                _thisframe++;

                if (NextFrame is null)
                    _thisframe = 0;
            }

            State = AnimationKey.Lerp(ThisFrame, NextFrame, Timer / FrameDuration);
        }

        public void Animate(InstancedModel model)
        {
            if (model is null) return;

            model.Scale *= State.Scale;
            model.Position += State.Position;
            model.Rotation = State.Rotation;
        }
    }
}