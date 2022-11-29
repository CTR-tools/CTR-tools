using Assimp;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace ctrviewer.Engine.Render
{
    public class AnimationKey
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;
        public float Time;

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
    }

    public class SimpleAnimation : List<AnimationKey>
    {
        
    }

    public class AnimationPlayer
    {
        public bool Paused = true;

        public SimpleAnimation Animation;

        public int NumFrames => Animation.Count;

        public float Speed = 1f;

        public float Timer = 0;

        public AnimationKey State;

        public AnimationPlayer(SimpleAnimation anim)
        {
            Animation = anim;
            State = anim[0];
        }

        public AnimationPlayer()
        {
            Animation.Add(new AnimationKey() { Position = new Vector3(0, 0, 0), Rotation = new Vector3(0, 0, 0), Scale = new Vector3(1), Time = 0 });
            Animation.Add(new AnimationKey() { Position = new Vector3(0, 0, 0), Rotation = new Vector3(0, 0, 0), Scale = new Vector3(1), Time = 2000 });

            State = Animation[0];
        }

        public void Run()
        {
            Paused = false;
        }

        public void Pause()
        {
            Paused = true;
        }

        private int frame;

        private float NextFrameTime => NextFrame.Time * Speed;
        private float ThisFrameTime => ThisFrame.Time * Speed;

        private float FrameDuration => (NextFrame.Time - ThisFrame.Time) / Speed;

        private AnimationKey ThisFrame => Animation[frame];
        private AnimationKey NextFrame => frame + 1 < Animation.Count ? Animation[frame + 1] : null ;

        public static AnimationPlayer Create(string animName) => new AnimationPlayer(ContentVault.GetVectorAnim(animName));

        public void Load(string animName)
        {
            Animation = ContentVault.GetVectorAnim(animName);
        }

        public void Advance(GameTime gameTime)
        {
            if (Paused) return;
            if (NumFrames == 0) return;

            Timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            //reset
            if (NextFrame is null)
            {
                Timer = 0;
                frame = 0;
                return;
            }

            if (Timer >= FrameDuration)
            {
                Timer -= FrameDuration;
                frame++;
                return;
            }

            State = AnimationKey.Lerp(ThisFrame, NextFrame, Timer / FrameDuration);
        }
    }
}