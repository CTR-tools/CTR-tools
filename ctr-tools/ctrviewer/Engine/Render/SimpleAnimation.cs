using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ctrviewer.Engine.Render
{
    public class AnimationKey
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;
        public float TimeValue;
    }
    public class SimpleAnimation
    {
        public bool forcepos = true;

        public float Timer = 0;

        public List<AnimationKey> Keys = new List<AnimationKey>();
        public int TotalKeys => Keys.Count;

        public AnimationKey State;

        public SimpleAnimation()
        {
            Keys.Add(new AnimationKey() { Position = new Vector3(0, 0, 0), Rotation = new Vector3(0, 0, 0), Scale = new Vector3(1f), TimeValue = 0 });
            Keys.Add(new AnimationKey() { Position = new Vector3(0, 0, 0), Rotation = new Vector3(0, 0, 0), Scale = new Vector3(1), TimeValue = 2000 });
            //Keys.Add(new AnimationKey() { Position = new Vector3(0, 0, 0), Rotation = new Vector3(6.28f, 0, 0), Scale = new Vector3(1), TimeValue = 3000 });
            //Keys.Add(new AnimationKey() { Position = new Vector3(0, 0, 0), Rotation = new Vector3(6.28f, 0, 0), Scale = new Vector3(1), TimeValue = 4500 });

            State = Keys[0];
        }

        public int frame;

        public void Update(GameTime gameTime)
        {
            if (TotalKeys == 0) return;

            frame = TotalKeys - 1;

            Timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (Timer >= Keys[frame].TimeValue)
            {
                Timer = 0;
                return;
            }

            while (Keys[frame].TimeValue > Timer)
                frame--;

            if (frame < 0)
                frame = 0;

            float timeScale = (Timer - Keys[frame].TimeValue) / (Keys[frame + 1].TimeValue - Keys[frame].TimeValue);

            State = new AnimationKey()
            {
                Position = Vector3.Lerp(Keys[frame].Position, Keys[frame + 1].Position, timeScale),
                Rotation = Vector3.Lerp(Keys[frame].Rotation, Keys[frame + 1].Rotation, timeScale),
                Scale = Vector3.Lerp(Keys[frame].Scale, Keys[frame + 1].Scale, timeScale)
            };
        }
    }
}