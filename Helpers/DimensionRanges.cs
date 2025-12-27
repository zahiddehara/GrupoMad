namespace GrupoMad.Helpers
{
    /// <summary>
    /// Predefined dimension ranges for curtain pricing matrix
    /// </summary>
    public static class DimensionRanges
    {
        /// <summary>
        /// Width ranges (29 ranges from 0.00m to 20.00m)
        /// </summary>
        public static readonly List<(decimal Min, decimal Max)> WidthRanges = new()
        {
            (0.00m, 1.39m), (1.40m, 1.59m), (1.60m, 1.79m), (1.80m, 1.99m),
            (2.00m, 2.19m), (2.20m, 2.39m), (2.40m, 2.59m), (2.60m, 2.79m),
            (2.80m, 2.99m), (3.00m, 3.19m), (3.20m, 3.39m), (3.40m, 3.59m),
            (3.60m, 3.79m), (3.80m, 3.99m), (4.00m, 4.19m), (4.20m, 4.39m),
            (4.40m, 4.59m), (4.60m, 4.79m), (4.80m, 4.99m), (5.00m, 5.19m),
            (5.20m, 5.39m), (5.40m, 5.59m), (5.60m, 5.79m), (5.80m, 5.99m),
            (6.00m, 6.49m), (6.50m, 6.99m), (7.00m, 7.49m), (7.50m, 7.99m),
            (8.00m, 20.00m)
        };

        /// <summary>
        /// Length ranges (45 ranges from 1.00m to 9.49m)
        /// </summary>
        public static readonly List<(decimal Min, decimal Max)> LengthRanges = new()
        {
            // 0.10m increments from 1.00 to 4.49
            (1.00m, 1.09m), (1.10m, 1.19m), (1.20m, 1.29m), (1.30m, 1.39m),
            (1.40m, 1.49m), (1.50m, 1.59m), (1.60m, 1.69m), (1.70m, 1.79m),
            (1.80m, 1.89m), (1.90m, 1.99m), (2.00m, 2.09m), (2.10m, 2.19m),
            (2.20m, 2.29m), (2.30m, 2.39m), (2.40m, 2.49m), (2.50m, 2.59m),
            (2.60m, 2.69m), (2.70m, 2.79m), (2.80m, 2.89m), (2.90m, 2.99m),
            (3.00m, 3.09m), (3.10m, 3.19m), (3.20m, 3.29m), (3.30m, 3.39m),
            (3.40m, 3.49m), (3.50m, 3.59m), (3.60m, 3.69m), (3.70m, 3.79m),
            (3.80m, 3.89m), (3.90m, 3.99m), (4.00m, 4.09m), (4.10m, 4.19m),
            (4.20m, 4.29m), (4.30m, 4.39m), (4.40m, 4.49m),
            // 0.50m increments from 4.50 to 9.49
            (4.50m, 4.99m), (5.00m, 5.49m), (5.50m, 5.99m), (6.00m, 6.49m),
            (6.50m, 6.99m), (7.00m, 7.49m), (7.50m, 7.99m), (8.00m, 8.49m),
            (8.50m, 8.99m), (9.00m, 9.49m)
        };

        /// <summary>
        /// Special length ranges (6 ranges for special curtain pricing)
        /// </summary>
        public static readonly List<(decimal Min, decimal Max)> SpecialLengthRanges = new()
        {
            (0m, 1.59m),
            (1.60m, 1.79m),
            (1.80m, 1.99m),
            (2.00m, 2.19m),
            (2.20m, 2.39m),
            (2.40m, 2.59m)
        };

        /// <summary>
        /// Formats a range as display string
        /// </summary>
        public static string FormatRange(decimal min, decimal max)
        {
            return $"{min:0.00} - {max:0.00}";
        }

        /// <summary>
        /// Creates a unique key for a width/length range combination
        /// </summary>
        public static string CreateRangeKey(int widthIndex, int lengthIndex)
        {
            return $"{widthIndex}_{lengthIndex}";
        }

        /// <summary>
        /// Parses a range key back to indices
        /// </summary>
        public static (int widthIndex, int lengthIndex) ParseRangeKey(string key)
        {
            var parts = key.Split('_');
            return (int.Parse(parts[0]), int.Parse(parts[1]));
        }
    }
}
