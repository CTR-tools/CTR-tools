using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ctrviewer.Engine.Render
{
    public class AnimationKey
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;

        public float Time;
        public float TimeValue => Time / (Parent != null ? Parent.Speed : 1.0f);

        public SimpleAnimation Parent;
    }

    public class SimpleAnimation : List<AnimationKey>
    {
        public float Speed = 1.0f;

        public float Timer = 0;

        public AnimationKey State;

        public SimpleAnimation()
        {
            Add(new AnimationKey() { Parent = this, Position = new Vector3(0, 0, 0), Rotation = new Vector3(0, 0, 0), Scale = new Vector3(1f), Time = 0 });
            Add(new AnimationKey() { Parent = this, Position = new Vector3(0, 0, 0), Rotation = new Vector3(0, 0, 0), Scale = new Vector3(1), Time = 2000 });
            //Keys.Add(new AnimationKey() { Position = new Vector3(0, 0, 0), Rotation = new Vector3(6.28f, 0, 0), Scale = new Vector3(1), TimeValue = 3000 });
            //Keys.Add(new AnimationKey() { Position = new Vector3(0, 0, 0), Rotation = new Vector3(6.28f, 0, 0), Scale = new Vector3(1), TimeValue = 4500 });

            State = this[0];
        }

        public int frame;

        public void Update(GameTime gameTime)
        {
            if (Count == 0) return;

            frame = Count - 1;

            Timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (Timer >= this[frame].TimeValue)
            {
                Timer = 0;
                State = this[0];
                return;
            }

            while (this[frame].TimeValue > Timer)
                frame--;

            if (frame < 0)
                frame = 0;

            float timeScale = (Timer - this[frame].TimeValue) / (this[frame + 1].TimeValue - this[frame].TimeValue);

            State = new AnimationKey()
            {
                Position = Vector3.Lerp(this[frame].Position, this[frame + 1].Position, timeScale),
                Rotation = Vector3.Lerp(this[frame].Rotation, this[frame + 1].Rotation, timeScale),
                Scale = Vector3.Lerp(this[frame].Scale, this[frame + 1].Scale, timeScale)
            };
        }
    }
}