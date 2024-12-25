using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ctrviewer.Engine.Render
{
    public class BillboardTire
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Rotation = Vector3.Zero;
        public Vector3 Scale = Vector3.One;

        public VertexPositionColorTexture[] verts_sealed;
        /*
        public void Draw()
        {
            foreach (var pass in alpha.CurrentTechnique.Passes)
            {
                pass.Apply();

                foreach (var mat in indexBuffers)
                {
                    graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                        PrimitiveType.TriangleList,
                        anim == null ? verts_sealed : anim.verts_sealed, anim == null ? 0 : anim.frameSize * (int)Game1.frame, anim == null ? numVerts : anim.frameSize,
                        mat.indices_sealed, 0, mat.numFaces,
                        VertexPositionColorTexture.VertexDeclaration
                    );
                }
            }     
        }

        */
    }
}