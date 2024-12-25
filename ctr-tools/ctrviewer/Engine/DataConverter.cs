using CTRFramework;
using CTRFramework.Models;
using CTRFramework.Shared;
using ctrviewer.Engine.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ctrviewer.Engine
{
    class DataConverter
    {
        public static Vector3 ToVector3(Vector3s vector, double scale = 1.0f) => new Vector3((float)(vector.X * scale), (float)(vector.Y * scale), (float)(vector.Z * scale));
        //public static Vector3 ToVector3(Color color) => new Vector3(color.R / 255f, color.G / 255f, color.B / 255f);

        public static Vector3 ToVector3(System.Numerics.Vector3 vector, double scale = 1.0f) => new Vector3((float)(vector.X * scale), (float)(vector.Y * scale), (float)(vector.Z * scale));

        public static Vector3 ToVector3(Vector4s s, float scale = 1.0f) => new Vector3(s.X * scale, s.Y * scale, s.Z * scale);

        public static Color ToColor(Vector4b s) => new Color(s.X / 255f, s.Y / 255f, s.Z / 255f, 1f);// s.W);

        public static VertexPositionColorTexture ToVptc(Vertex v, System.Numerics.Vector2 uv, float scale = 1.0f)
        {
            return ToVptc(v, uv, Color.Gray, false, scale);
        }

        public static VertexPositionColorTexture ToVptc(Vertex v, System.Numerics.Vector2 uv, Color color, bool lerp = false, double scale = 1.0f)
        {
            return new VertexPositionColorTexture()
            {
                Position = ToVector3(v.Position, scale),
                Color = lerp ? Color.Lerp(color, ToColor(v.Color), 0.5f) : ToColor(v.Color),
                TextureCoordinate = new Vector2(uv.X / 255.0f, uv.Y / 255.0f)
            };
        }

        /// <summary>
        /// Converts CTR mesh to internal monogame viewer format.
        /// </summary>
        public static TriList ToTriList(CtrMesh mesh, Color color, bool lerp = false, float scale = 1f, int animIndex = 0, int frameIndex = 0)
        {
            //GameConsole.Write(model.Name);

            var trilist = new TriList();

            if (!mesh.IsAnimated)
            {
                //first, push all vertices to vertex buffer
                for (int i = 0; i < mesh.verts.Count / 3; i++)
                {
                    var li = new List<VertexPositionColorTexture>();

                    for (int j = i * 3; j < i * 3 + 3; j++)
                    {
                        var vert = mesh.verts[j];
                        li.Add(DataConverter.ToVptc(vert, vert.uv, color, lerp, scale * Helpers.GteScaleSmall)); //todo: what, why, shouldnt use gte scale here
                    }

                    trilist.PushTri(li);
                }
            }
            else
            {
                foreach (var anim in mesh.anims)
                {
                    var buf = new AnimatedVertexBuffer();

                    buf.totalFrames = anim.numFrames;
                    buf.frameSize = mesh.verts.Count;

                    foreach (var frame in anim.Frames)
                    {
                        mesh.frame = frame;
                        mesh.GetVertexBuffer();

                        for (int i = 0; i < mesh.verts.Count / 3; i++)
                        {
                            var li = new List<VertexPositionColorTexture>();

                            for (int j = i * 3; j < i * 3 + 3; j++)
                            {
                                var vert = mesh.verts[j];
                                li.Add(DataConverter.ToVptc(vert, vert.uv, color, lerp, scale * Helpers.GteScaleSmall)); //todo: what, why, shouldnt use gte scale here
                            }

                            buf.PushTri(li);
                        }
                    }

                    trilist.animsList.Add(buf);
                }

                trilist.anim = trilist.animsList[0];
            }

            //next create index buffers for all textures
            for (int i = 0; i < mesh.verts.Count / 3; i++)
            {
                //load textures
                var tex = mesh.matIndices[i];

                if (tex is not null)
                    if (tex.ParentLayout is not null)
                        tex = tex.ParentLayout;

                string texture = tex is null ? "default" : tex.Tag;

                // ptr intex buffer
                var buf = trilist.GetIndexBuffer(texture);
                buf.PushTri(i * 3 + 0, i * 3 + 1, i * 3 + 2);

                //todo: implement later
                /*
                t.textureName = texture;
                t.textureEnabled = t.textureName == "test" ? false : true;
                t.ScrollingEnabled = false;

                if (t.textureEnabled)
                    if (model[0].matIndices[i].blendingMode == CTRFramework.Vram.BlendingMode.Additive)
                        t.blendState = BlendState.Additive;
                */
            }

            trilist.Seal();

            return trilist;
        }

        /*
        public static AnimatedTriListCollection ToAnimatedTriListCollection(CtrModel model, int animIndex = 0)
        {
            var coll = new AnimatedTriListCollection();

            if (model[0].IsAnimated)
            {
                foreach (var frame in model[0].anims[animIndex].Frames)
                {
                    model[0].frame = frame;
                    model[0].GetVertexBuffer();

                    coll.Add(ToTriListCollection(model));
                }
            }

            return coll;
        }
        */

        public static SimpleAnimation ToSimpleAnimation(NavPath path)
        {
            var anim = new SimpleAnimation();
            anim.Clear();

            int time = 0;

            foreach (var point in path.Frames)
            {
                anim.Add(new AnimationKey() { Position = ToVector3(point.position), Rotation = Vector3.One, Scale = Vector3.One, Time = time });
                time += 200;
            }

            return anim;
        }

        public static SimpleAnimation ToSimpleAnimation(List<Pose> poses)
        {
            var anim = new SimpleAnimation();
            anim.Clear();

            int time = 0;

            foreach (var point in poses)
            {
                anim.Add(new AnimationKey()
                {
                    Position = ToVector3(point.Position),
                    Rotation = new Vector3(ToVector3(point.Rotation).X, ToVector3(point.Rotation).Y, ToVector3(point.Rotation).Z),
                    Scale = new Vector3(1),
                    Time = time
                });

                time += 1000;
            }

            anim.Add(anim[0]);

            return anim;
        }

        public static SimpleAnimation ToSimpleAnimation(List<RespawnPoint> respawns)
        {
            if (respawns.Count == 0)
                return null;

            var anim = new SimpleAnimation();
            anim.Clear();

            int time = 0;


            var zerospawn = respawns[0];
            var spawn = zerospawn;

            do
            {
                var rot = ToVector3(spawn.Pose.Rotation);

                anim.Add(new AnimationKey()
                {
                    Position = ToVector3(spawn.Pose.Position),
                    Rotation = new Vector3(0),
                    Scale = new Vector3(1f),
                    Time = time
                });

                time += 500;
                spawn = spawn.Next;
            }
            while (spawn != zerospawn && spawn.Next != null);

            anim.Add(new AnimationKey()
            {
                Position = anim[0].Position,
                Rotation = new Vector3(0),//anim.Keys[0].Rotation,
                Scale = new Vector3(1f),
                Time = time + 500
            });

            return anim;
        }
    }
}