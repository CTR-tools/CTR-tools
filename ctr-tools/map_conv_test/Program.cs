using CTRFramework;
using CTRFramework.Shared;
using System;

namespace map_conv_test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            OBJ.FixCulture();

            if (args.Length == 0)
            {
                Console.WriteLine("Expecting OBJ file.");
                Console.WriteLine("Must be built in quads.");
                return;
            }

            var obj = OBJ.FromFile(args[0]);

            var scn = new CtrScene()
            {
                header = new SceneHeader()
                {
                    glowGradients = new Gradient[3] { new Gradient(), new Gradient(), new Gradient() },
                    startGrid = new Pose[8]
                    {
                        new Pose(), new Pose(), new Pose(), new Pose(),
                        new Pose(), new Pose(), new Pose(), new Pose()
                    },
                    ptrMeshInfo = new PsxPtr(0x800 - 8 * 4)
                },
                verts = obj.CtrVerts,
                quads = obj.CtrQuads,

                mesh = new MeshInfo()
                {
                    numQuadBlocks = (uint)obj.CtrQuads.Count,
                    numVertices = (uint)obj.CtrVerts.Count,
                    ptrVertices = (UIntPtr)0x800,
                    ptrQuadBlocks = (UIntPtr)(0x800 + Vertex.SizeOf * obj.CtrVerts.Count),
                }
            };

            Console.WriteLine(scn.mesh.numVertices);
            Console.WriteLine(scn.verts[0].Position.ToString());


            scn.Save("test.lev");

            Console.WriteLine("done.");
        }
    }
}