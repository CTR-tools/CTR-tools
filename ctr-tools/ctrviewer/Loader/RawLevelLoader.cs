using Assimp;
using ctrviewer.Engine;
using ctrviewer.Engine.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ctrviewer.Loaders
{
    public class RawLevelLoader : MGLevel
    {
        public RawLevelLoader(string filename)
        {
            Load(filename);
        }

        public static MGLevel FromFile(string filename)
        {
            return new RawLevelLoader(filename);
        }

        private void Load(string filename)
        {
            using (AssimpContext importer = new AssimpContext())
            {
                ImportAssimpData(importer.ImportFile(filename));
            }
        }

        public void ImportAssimpData(Scene scene)
        {
            Random random = new Random();

            List<VertexPositionColorTexture> monolist = new List<VertexPositionColorTexture>();

            //if (!scene.HasMeshes)
            //    return;

            Trilists.Add("test", new TriList());

            List<Vector2> uv = new List<Vector2>();
            uv.Add(new Vector2(0, 0));
            uv.Add(new Vector2(0, 1));
            uv.Add(new Vector2(1, 0));
            uv.Add(new Vector2(1, 1));

            foreach (var mesh in scene.Meshes)
            {
                foreach (var face in mesh.Faces)
                {
                    monolist.Clear();

                    int val = random.Next(128 - 32) + 32 * 2;

                    foreach (var i in face.Indices)
                    {
                        monolist.Add(
                            new VertexPositionColorTexture(
                                Convert(mesh.Vertices[i]),
                                new Color(val, val, val) * (1.25f - mesh.Vertices[i].Y / 10f),
                                new Vector2(0, 0)
                                )
                            );

                        //VertexPositionColorTexture v = monolist[0];
                        // monolist[0] = monolist[1];
                        //monolist[1] = v;
                    }

                    switch (face.IndexCount)
                    {
                        case 3: Trilists["test"].PushTri(monolist); break;
                        case 4: Trilists["test"].PushQuad(monolist); break;
                        default: GameConsole.Write("Unsupported face count."); break;
                    }
                }
            }

            Seal();

            Trilists["test"].textureEnabled = true;

            GameConsole.Write($"numverts: {Trilists["test"].numVerts}");
            GameConsole.Write($"numfaces: {Trilists["test"].numFaces}");
        }

        public Vector3 Convert(Vector3D input) => new Vector3(input.X, input.Y, input.Z);
        public Vector3D Convert(Vector3 input) => new Vector3D(input.X, input.Y, input.Z);

        public Color4D Convert(Color color) => new Color4D(color.R, color.G, color.B, color.A);
        public Color Convert(Color4D color) => new Color(color.R, color.G, color.B, color.A);
    }
}