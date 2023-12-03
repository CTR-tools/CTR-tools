using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ctrviewer.Engine.Menu
{
    public class FrontendMessage
    {
        public string ID = "msg_id";
        public string Text = "<put your message here>";
        public float X = 0;
        public float Y = 0;
        public double Timer = 0;


        public static List<FrontendMessage> MessageList = new List<FrontendMessage>();

        public static void SendMessage(string ID, string text, float x, float y, float seconds)
        {
            foreach (var message in MessageList.ToList())
            {
                if (message.ID == ID)
                {
                    message.Timer = seconds;
                    message.X = x;
                    message.Y = y;
                    message.Text = text;

                    return;
                }
            }

            //ID not found, add new message
            MessageList.Add(new FrontendMessage { ID = ID, Text = text, X = x, Y = y, Timer = seconds });
        }

        public static void UpdateQueue(GameTime gameTime)
        {
            foreach (var message in MessageList.ToList())
            {
                message.Timer -= gameTime.ElapsedGameTime.TotalSeconds;
                if (message.Timer < 0)
                    MessageList.Remove(message);
            }
        }
    }
}