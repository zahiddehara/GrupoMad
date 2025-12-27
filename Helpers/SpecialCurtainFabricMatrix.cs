namespace GrupoMad.Helpers
{
    /// <summary>
    /// Static class containing the special curtain fabric usage matrix.
    /// This matrix defines fabric usage for special pricing with 6 height ranges.
    /// </summary>
    public static class SpecialCurtainFabricMatrix
    {
        // Fabric usage matrix [height_index, width_index]
        // Height ranges: 0-1.59, 1.60-1.79, 1.80-1.99, 2.00-2.19, 2.20-2.39, 2.40-2.59
        // Values mapped from CurtainFabricMatrix rows: 0, 6, 8, 10, 12, 14
        public static readonly decimal[,] FabricUsage = new decimal[,]
        {
            // Width: 1.2,  1.4,  1.6,  1.8,  2,    2.2,  2.4,  2.6,  2.8,  3,    3.2,  3.4,  3.6,  3.8,  4,    4.2,  4.4,  4.6,  4.8,  5,    5.2,  5.4,  5.6,  5.8,  6,    6.5,  7,    7.5,  8
            /* 0.00 - 1.59 */ { 2.9m, 3.4m, 3.9m, 4.32m, 4.8m, 5.3m, 5.8m, 6.24m, 6.72m, 7.2m, 7.7m, 8.2m, 8.7m, 9.12m, 9.6m, 10.1m, 10.6m, 11.04m, 11.52m, 12m, 12.5m, 12.96m, 13.44m, 13.92m, 14.4m, 15.6m, 16.8m, 18m, 19.2m },
            /* 1.6 - 1.79 */ { 2.9m, 3.4m, 3.9m, 4.32m, 4.8m, 5.3m, 5.8m, 6.24m, 6.72m, 7.2m, 7.7m, 8.2m, 8.7m, 9.12m, 9.6m, 10.1m, 10.6m, 11.04m, 11.52m, 12m, 12.5m, 12.96m, 13.44m, 13.92m, 14.4m, 15.6m, 16.8m, 18m, 19.2m },
            /* 1.8 - 1.99 */ { 2.9m, 3.4m, 3.9m, 4.32m, 4.8m, 5.3m, 5.8m, 6.24m, 6.72m, 7.2m, 7.7m, 8.2m, 8.7m, 9.12m, 9.6m, 10.1m, 10.6m, 11.04m, 11.52m, 12m, 12.5m, 12.96m, 13.44m, 13.92m, 14.4m, 15.6m, 16.8m, 18m, 19.2m },
            /* 2.0 - 2.19 */ { 2.9m, 3.4m, 3.9m, 4.32m, 4.8m, 5.3m, 5.8m, 6.24m, 6.72m, 7.2m, 7.7m, 8.2m, 8.7m, 9.12m, 9.6m, 10.1m, 10.6m, 11.04m, 11.52m, 12m, 12.5m, 12.96m, 13.44m, 13.92m, 14.4m, 15.6m, 16.8m, 18m, 19.2m },
            /* 2.2 - 2.39 */ { 2.9m, 3.4m, 3.9m, 4.32m, 4.8m, 5.3m, 5.8m, 6.24m, 6.72m, 7.2m, 7.7m, 8.2m, 8.7m, 9.12m, 9.6m, 10.1m, 10.6m, 11.04m, 11.52m, 12m, 12.5m, 12.96m, 13.44m, 13.92m, 14.4m, 15.6m, 16.8m, 18m, 19.2m },
            /* 2.4 */ { 2.9m, 3.4m, 3.9m, 4.32m, 4.8m, 5.3m, 5.8m, 6.24m, 6.72m, 7.2m, 7.7m, 8.2m, 8.7m, 9.12m, 9.6m, 10.1m, 10.6m, 11.04m, 11.52m, 12m, 12.5m, 12.96m, 13.44m, 13.92m, 14.4m, 15.6m, 16.8m, 18m, 19.2m }
        };

        /// <summary>
        /// Gets the fabric usage for given width and height values.
        /// Returns the closest match based on the matrix.
        /// </summary>
        /// <param name="width">Width in meters</param>
        /// <param name="height">Height in meters</param>
        /// <returns>Fabric usage in meters</returns>
        public static decimal GetFabricUsage(decimal width, decimal height)
        {
            int widthIndex = FindWidthRangeIndex(width);
            int heightIndex = FindHeightRangeIndex(height);

            return FabricUsage[heightIndex, widthIndex];
        }

        /// <summary>
        /// Finds the index of the range that contains the given height value.
        /// </summary>
        private static int FindHeightRangeIndex(decimal height)
        {
            for (int i = 0; i < DimensionRanges.SpecialLengthRanges.Count; i++)
            {
                var range = DimensionRanges.SpecialLengthRanges[i];
                if (height >= range.Min && height <= range.Max)
                    return i;
            }
            // Default to last range if exceeds all
            return DimensionRanges.SpecialLengthRanges.Count - 1;
        }

        /// <summary>
        /// Finds the index of the range that contains the given width value.
        /// </summary>
        private static int FindWidthRangeIndex(decimal width)
        {
            for (int i = 0; i < DimensionRanges.WidthRanges.Count; i++)
            {
                var range = DimensionRanges.WidthRanges[i];
                if (width >= range.Min && width <= range.Max)
                    return i;
            }
            // Default to last range if exceeds all
            return DimensionRanges.WidthRanges.Count - 1;
        }

        /// <summary>
        /// Gets fabric usage by dimension range indices (for use with DimensionRanges).
        /// </summary>
        /// <param name="widthRangeIndex">Index in DimensionRanges.WidthRanges</param>
        /// <param name="heightRangeIndex">Index in DimensionRanges.SpecialLengthRanges</param>
        /// <returns>Fabric usage in meters</returns>
        public static decimal GetFabricUsageByRangeIndex(int widthRangeIndex, int heightRangeIndex)
        {
            // Get the min value of each range to look up in the fabric matrix
            var widthRange = DimensionRanges.WidthRanges[widthRangeIndex];
            var heightRange = DimensionRanges.SpecialLengthRanges[heightRangeIndex];

            return GetFabricUsage(widthRange.Min, heightRange.Min);
        }
    }
}
