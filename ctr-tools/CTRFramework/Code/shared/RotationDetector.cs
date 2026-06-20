using System.Numerics;

namespace CTRFramework.Shared
{
    /// <summary>
    /// The idea of rotation detector is to represent quad UV as a 2x2 matrix, then compare to texture layout UV.
    /// First we rotate it to the right 4 times, then flip, then rotate 4 more times.
    /// When a match is found, return a corresponding rotatefliptype flag.
    /// It's just a math helpers, so the class itself is static: parameters are passed by reference and modified directly.
    /// Usage example: var result = RotationDetector.Test(matrixA, matrixB);
    /// </summary>
    public static class RotationDetector
    {
        /// <summary>
        /// Flips 2x2 matrix along the X axis.
        /// </summary>
        /// <param name="m"></param>
        private static void FlipX(ref Vector2[,] m)
        {
            Vector2 buf;

            buf = m[0, 0];
            m[0, 0] = m[1, 0];
            m[1, 0] = buf;

            buf = m[0, 1];
            m[0, 1] = m[1, 1];
            m[1, 1] = buf;
        }

        /// <summary>
        /// Flips 2x2 matrix along the Y axis.
        /// </summary>
        /// <param name="m"></param>
        private static void FlipY(ref Vector2[,] m)
        {
            Vector2 buf;

            buf = m[0, 0];
            m[0, 0] = m[0, 1];
            m[0, 1] = buf;

            buf = m[1, 0];
            m[1, 0] = m[1, 1];
            m[1, 1] = buf;
        }

        /// <summary>
        /// Rotates 2x2 matrix clockwise.
        /// </summary>
        /// <param name="m"></param>
        private static void RotateClockwise(ref Vector2[,] m)
        {
            var buf = m[0, 0];
            m[0, 0] = m[1, 0];
            m[1, 0] = m[1, 1];
            m[1, 1] = m[0, 1];
            m[0, 1] = buf;
        }

        /// <summary>
        /// Rotates 2x2 matrix counterclockwise.
        /// </summary>
        /// <param name="m"></param>
        private static void RotateCounterClockwise(ref Vector2[,] m)
        {
            var buf = m[0, 0];
            m[0, 0] = m[0, 1];
            m[0, 1] = m[1, 1];
            m[1, 1] = m[1, 0];
            m[1, 0] = buf;
        }

        /// <summary>
        /// Determines whether two 2x2 matrices coincide.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>True if matrices coincide, false otherwise.</returns>
        private static bool Equals(Vector2[,] a, Vector2[,] b)
        {
            return a[0, 0] == b[0, 0] && a[0, 1] == b[0, 1] && a[1, 0] == b[1, 0] && a[1, 1] == b[1, 1];
        }

        /// <summary>
        /// 2x2 matrix comparison function.
        /// Performs a series of transforms and compares the intermediate result to the target matrix.
        /// If conicides, return the detected match.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static RotateFlipType Test(Vector2[,] a, Vector2[,] b)
        {
            // maybe we already have a matching matrices
            if (Equals(a, b)) return RotateFlipType.None;

            // otherwise keep rotating
            RotateClockwise(ref a); if (Equals(a, b)) return RotateFlipType.Rotate90;
            RotateClockwise(ref a); if (Equals(a, b)) return RotateFlipType.Rotate180;
            RotateClockwise(ref a); if (Equals(a, b)) return RotateFlipType.Rotate270;
            RotateClockwise(ref a);

            // now we're back to the original rotation, so flip it
            FlipX(ref a); if (Equals(a, b)) return RotateFlipType.Flip;

            // and rotate the other way around
            RotateCounterClockwise(ref a); if (Equals(a, b)) return RotateFlipType.FlipRotate90;
            RotateCounterClockwise(ref a); if (Equals(a, b)) return RotateFlipType.FlipRotate180;
            RotateCounterClockwise(ref a); if (Equals(a, b)) return RotateFlipType.FlipRotate270;

            // bummer
            // may only happen if B is not a transformed A.
            return RotateFlipType.NoMatch;
        }
    }
}