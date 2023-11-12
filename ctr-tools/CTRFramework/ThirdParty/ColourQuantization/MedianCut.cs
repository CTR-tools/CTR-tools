using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColourQuantization
{
    class MedianCut
    {
        public static Bitmap Quantize(Bitmap bitmap, int colourCount)
        {
            var colours = Enumerable.Range(0, bitmap.Height)
                .SelectMany(y => Enumerable.Range(0, bitmap.Width)
                    .Select(x => bitmap.GetPixel(x, y)));
            var buckets = new List<Bucket> { new Bucket(colours) };

            while (buckets.Count < colourCount)
            {
                var newBuckets = new List<Bucket>();
                for (var i = 0; i < buckets.Count; i++)
                {
                    if (newBuckets.Count + (buckets.Count - i) < colourCount)
                    {
                        var split = buckets[i].Split();
                        newBuckets.Add(split.Item1);
                        newBuckets.Add(split.Item2);
                    }
                    else
                    {
                        newBuckets.AddRange(buckets.GetRange(i, buckets.Count - i));
                        break;
                    }
                }
                buckets = newBuckets;
            }

            var ret = new Bitmap(bitmap.Width, bitmap.Height);
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    var bucket = buckets.First(b => b.HasColour(bitmap.GetPixel(x, y)));
                    ret.SetPixel(x, y, bucket.Colour);
                }
            }
            return ret;
        }

        private class Bucket
        {
            private readonly IDictionary<Color, int> colours;

            public Color Colour { get; }

            public Bucket(IEnumerable<Color> colours)
            {
                this.colours = colours.ToLookup(c => c)
                    .ToDictionary(c => c.Key, c => c.Count());
                this.Colour = Average(this.colours);
            }

            public Bucket(IEnumerable<KeyValuePair<Color, int>> enumerable)
            {
                this.colours = enumerable.ToDictionary(c => c.Key, c => c.Value);
                this.Colour = Average(this.colours);
            }

            private static Color Average(IEnumerable<KeyValuePair<Color, int>> colours)
            {
                var totals = colours.Sum(c => c.Value);
                return Color.FromArgb(
                    alpha: 255,
                    red: colours.Sum(c => c.Key.R * c.Value) / totals,
                    green: colours.Sum(c => c.Key.G * c.Value) / totals,
                    blue: colours.Sum(c => c.Key.B * c.Value) / totals);
            }

            public bool HasColour(Color colour)
            {
                return colours.ContainsKey(colour);
            }

            public Tuple<Bucket, Bucket> Split()
            {
                var redRange = colours.Keys.Max(c => c.R) - colours.Keys.Min(c => c.R);
                var greenRange = colours.Keys.Max(c => c.G) - colours.Keys.Min(c => c.G);
                var blueRange = colours.Keys.Max(c => c.B) - colours.Keys.Min(c => c.B);

                Func<Color, int> sorter;
                if (redRange > greenRange)
                {
                    if (redRange > blueRange)
                    {
                        sorter = c => c.R;
                    }
                    else
                    {
                        sorter = c => c.B;
                    }
                }
                else
                {
                    if (greenRange > blueRange)
                    {
                        sorter = c => c.G;
                    }
                    else
                    {
                        sorter = c => c.B;
                    }
                }

                var sorted = colours.OrderBy(c => sorter(c.Key));

                var firstBucketCount = sorted.Count() / 2;

                var bucket1 = new Bucket(sorted.Take(firstBucketCount));
                var bucket2 = new Bucket(sorted.Skip(firstBucketCount));
                return new Tuple<Bucket, Bucket>(bucket1, bucket2);
            }
        }
    }
}
