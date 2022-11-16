using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using ctrviewer.Engine.Input;

namespace ctrviewer.Engine.Testing
{
    public struct ColorScheme
    {
        public Color BackgroundColor;
        public Color ForegroundColor;
    }

    public class MenuButton : MenuRootComponent
    {
        public ColorScheme StateNormal = new ColorScheme { BackgroundColor = Color.White, ForegroundColor = Color.Black };
        public ColorScheme StateHover = new ColorScheme { BackgroundColor = Color.White, ForegroundColor = Color.Green };
        public ColorScheme StatePressed = new ColorScheme { BackgroundColor = Color.Gray, ForegroundColor = Color.Red };
        public ColorScheme StateDisabled = new ColorScheme { BackgroundColor = Color.Gray, ForegroundColor = Color.Black };

        public string Text = "Default text";

        public MenuButton() : base()
        {
        }

        public override void DrawComponent(GameTime gameTime, SpriteBatch sb)
        {
            if (MenuRootComponent.Font == null)
            {
                Console.WriteLine("no font assigned.");
                return;
            }

            ColorScheme color = StateNormal;

            switch (buttonState)
            {
                case xButtonState.Normal: color = StateNormal; break;
                case xButtonState.Hover: color = StateHover; break;
                case xButtonState.Pressed: color = StatePressed; break;
                case xButtonState.Disabled: color = StateDisabled; break;
            }

            sb.DrawString(
                MenuRootComponent.Font,
                Text,
                new Vector2(this.Position.X, this.Position.Y),
                color.ForegroundColor
                );
        }
    }

    public enum xButtonState
    {
        Normal,
        Hover,
        Pressed,
        Disabled
    }

    public class MenuRootComponent
    {
        public static SpriteFont Font;

        public bool Visible = true;
        public bool Enabled = true;

        public Point Position = new Point(32, 32);
        public Point Size = new Point(64, 32);

        public List<MenuRootComponent> Children = new List<MenuRootComponent>();

        public xButtonState buttonState = xButtonState.Normal;

        public Rectangle Region
        {
            get => new Rectangle(Position, Size);
        }

        public event EventHandler<EventArgs> OnClick = delegate { };

        public MenuRootComponent()
        {
        }

        public void Update(GameTime gameTime)
        {
            if (Enabled)
            {
                buttonState = xButtonState.Normal;

                if (Intersects(MouseHandler.Position))
                {
                    OnClick(this, new EventArgs());
                    buttonState = xButtonState.Hover;
                }

                foreach (var x in Children)
                    x.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch sb)
        {
            if (Visible)
            {
                DrawComponent(gameTime, sb);

                foreach (var x in Children)
                    x.Draw(gameTime, sb);
            }
        }

        public virtual void DrawComponent(GameTime gameTime, SpriteBatch sb)
        {
        }

        public bool Intersects(Point mousePosition)
        {
            return Region.Intersects(new Rectangle(mousePosition, new Point(1, 1)));
        }
    }
}