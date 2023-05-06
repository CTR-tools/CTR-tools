using System;
using System.Numerics;

namespace CTRFramework.Shared
{
    /// <summary>
    /// The idea of rotation detector is to represent quad UV as a 2x2 matrix, then compare to texture layout UV.
    /// First we rotate it to the right 4 times, then flip, then rotate 4 more times.
    /// When match is found, return respective rotatefliptype flag.
    /// </summary>
    public class RotationDetector
    {
        private static void FlipX(ref Vector2[,] a)
        {
            Vector2 buf;

            buf = a[0, 0];
            a[0, 0] = a[1, 0];
            a[1, 0] = buf;

            buf = a[0, 1];
            a[0, 1] = a[1, 1];
            a[1, 1] = buf;
        }

        private static void FlipY(ref Vector2[,] a)
        {
            Vector2 buf;

            buf = a[0, 0];
            a[0, 0] = a[0, 1];
            a[0, 1] = buf;

            buf = a[1, 0];
            a[1, 0] = a[1, 1];
            a[1, 1] = buf;
        }

        private static void RotateRight(ref Vector2[,] a)
        {
            Vector2 buf;

            buf = a[0, 0];
            a[0, 0] = a[1, 0];
            a[1, 0] = a[1, 1];
            a[1, 1] = a[0, 1];
            a[0, 1] = buf;
        }

        private static void RotateLeft(ref Vector2[,] a)
        {
            Vector2 buf;

            buf = a[0, 0];
            a[0, 0] = a[0, 1];
            a[0, 1] = a[1, 1];
            a[1, 1] = a[1, 0];
            a[1, 0] = buf;
        }

        private static bool Equals(Vector2[,] a, Vector2[,] b)
        {
            return a[0, 0] == b[0, 0] && a[0, 1] == b[0, 1] && a[1, 0] == b[1, 0] && a[1, 1] == b[1, 1];
        }

        public static RotateFlipType Test(Vector2[,] a, Vector2[,] b)
        {
            if (Equals(a, b)) return RotateFlipType.None;
            RotateRight(ref a); if (Equals(a, b)) return RotateFlipType.Rotate90;
            RotateRight(ref a); if (Equals(a, b)) return RotateFlipType.Rotate180;
            RotateRight(ref a); if (Equals(a, b)) return RotateFlipType.Rotate270;
            RotateRight(ref a); //now we're back to the original
            FlipX(ref a); if (Equals(a, b)) return RotateFlipType.Flip;
            RotateLeft(ref a); if (Equals(a, b)) return RotateFlipType.FlipRotate90;
            RotateLeft(ref a); if (Equals(a, b)) return RotateFlipType.FlipRotate180;
            RotateLeft(ref a); if (Equals(a, b)) return RotateFlipType.FlipRotate270;

            //bummer
            return RotateFlipType.NoMatch;
        }
    }
}