namespace GrupoMad.Helpers
{
    /// <summary>
    /// Static class containing the curtain fabric usage matrix.
    /// This matrix defines fabric usage based on width and height dimensions.
    /// </summary>
    public static class CurtainFabricMatrix
    {
        // Fabric usage matrix [height_index, width_index]
        public static readonly decimal[,] FabricUsage = new decimal[,]
        {
            // Width: 1.2,  1.4,  1.6,  1.8,  2,    2.2,  2.4,  2.6,  2.8,  3,    3.2,  3.4,  3.6,  3.8,  4,    4.2,  4.4,  4.6,  4.8,  5,    5.2,  5.4,  5.6,  5.8,  6,    6.5,  7,    7.5,  8
            /* 1.0 */ { 1.4m, 2.8m, 2.8m, 2.8m, 2.8m, 2.8m, 2.8m, 4.2m, 4.2m, 4.2m, 4.2m, 4.2m, 4.2m, 5.6m, 5.6m, 5.6m, 5.6m, 5.6m, 5.6m, 5.6m, 7m,   7m,   7m,   7m,   7m,   8.4m, 8.4m, 8.4m, 8.4m },
            /* 1.1 */ { 1.5m, 3m,   3m,   3m,   3m,   3m,   3m,   4.5m, 4.5m, 4.5m, 4.5m, 4.5m, 4.5m, 6m,   6m,   6m,   6m,   6m,   6m,   6m,   7.5m, 7.5m, 7.5m, 7.5m, 7.5m, 9m,   9m,   9m,   9m },
            /* 1.2 */ { 1.6m, 3.2m, 3.2m, 3.2m, 3.2m, 3.2m, 3.2m, 4.8m, 4.8m, 4.8m, 4.8m, 4.8m, 4.8m, 6.4m, 6.4m, 6.4m, 6.4m, 6.4m, 6.4m, 6.4m, 8m,   8m,   8m,   8m,   8m,   9.6m, 9.6m, 9.6m, 9.6m },
            /* 1.3 */ { 1.7m, 3.4m, 3.4m, 3.4m, 3.4m, 3.4m, 3.4m, 5.1m, 5.1m, 5.1m, 5.1m, 5.1m, 5.1m, 6.8m, 6.8m, 6.8m, 6.8m, 6.8m, 6.8m, 6.8m, 8.5m, 8.5m, 8.5m, 8.5m, 8.5m, 10.2m, 10.2m, 10.2m, 10.2m },
            /* 1.4 */ { 2.9m, 3.4m, 3.9m, 4.32m, 4.8m, 5.3m, 5.8m, 6.24m, 6.72m, 7.2m, 7.7m, 8.2m, 8.7m, 9.12m, 9.6m, 10.1m, 10.6m, 11.04m, 11.52m, 12m, 12.5m, 12.96m, 13.44m, 13.92m, 14.4m, 15.6m, 16.8m, 18m, 19.2m },
            /* 1.5 */ { 2.9m, 3.4m, 3.9m, 4.32m, 4.8m, 5.3m, 5.8m, 6.24m, 6.72m, 7.2m, 7.7m, 8.2m, 8.7m, 9.12m, 9.6m, 10.1m, 10.6m, 11.04m, 11.52m, 12m, 12.5m, 12.96m, 13.44m, 13.92m, 14.4m, 15.6m, 16.8m, 18m, 19.2m },
            /* 1.6 */ { 2.9m, 3.4m, 3.9m, 4.32m, 4.8m, 5.3m, 5.8m, 6.24m, 6.72m, 7.2m, 7.7m, 8.2m, 8.7m, 9.12m, 9.6m, 10.1m, 10.6m, 11.04m, 11.52m, 12m, 12.5m, 12.96m, 13.44m, 13.92m, 14.4m, 15.6m, 16.8m, 18m, 19.2m },
            /* 1.7 */ { 2.9m, 3.4m, 3.9m, 4.32m, 4.8m, 5.3m, 5.8m, 6.24m, 6.72m, 7.2m, 7.7m, 8.2m, 8.7m, 9.12m, 9.6m, 10.1m, 10.6m, 11.04m, 11.52m, 12m, 12.5m, 12.96m, 13.44m, 13.92m, 14.4m, 15.6m, 16.8m, 18m, 19.2m },
            /* 1.8 */ { 2.9m, 3.4m, 3.9m, 4.32m, 4.8m, 5.3m, 5.8m, 6.24m, 6.72m, 7.2m, 7.7m, 8.2m, 8.7m, 9.12m, 9.6m, 10.1m, 10.6m, 11.04m, 11.52m, 12m, 12.5m, 12.96m, 13.44m, 13.92m, 14.4m, 15.6m, 16.8m, 18m, 19.2m },
            /* 1.9 */ { 2.9m, 3.4m, 3.9m, 4.32m, 4.8m, 5.3m, 5.8m, 6.24m, 6.72m, 7.2m, 7.7m, 8.2m, 8.7m, 9.12m, 9.6m, 10.1m, 10.6m, 11.04m, 11.52m, 12m, 12.5m, 12.96m, 13.44m, 13.92m, 14.4m, 15.6m, 16.8m, 18m, 19.2m },
            /* 2.0 */ { 2.9m, 3.4m, 3.9m, 4.32m, 4.8m, 5.3m, 5.8m, 6.24m, 6.72m, 7.2m, 7.7m, 8.2m, 8.7m, 9.12m, 9.6m, 10.1m, 10.6m, 11.04m, 11.52m, 12m, 12.5m, 12.96m, 13.44m, 13.92m, 14.4m, 15.6m, 16.8m, 18m, 19.2m },
            /* 2.1 */ { 2.9m, 3.4m, 3.9m, 4.32m, 4.8m, 5.3m, 5.8m, 6.24m, 6.72m, 7.2m, 7.7m, 8.2m, 8.7m, 9.12m, 9.6m, 10.1m, 10.6m, 11.04m, 11.52m, 12m, 12.5m, 12.96m, 13.44m, 13.92m, 14.4m, 15.6m, 16.8m, 18m, 19.2m },
            /* 2.2 */ { 2.9m, 3.4m, 3.9m, 4.32m, 4.8m, 5.3m, 5.8m, 6.24m, 6.72m, 7.2m, 7.7m, 8.2m, 8.7m, 9.12m, 9.6m, 10.1m, 10.6m, 11.04m, 11.52m, 12m, 12.5m, 12.96m, 13.44m, 13.92m, 14.4m, 15.6m, 16.8m, 18m, 19.2m },
            /* 2.3 */ { 2.9m, 3.4m, 3.9m, 4.32m, 4.8m, 5.3m, 5.8m, 6.24m, 6.72m, 7.2m, 7.7m, 8.2m, 8.7m, 9.12m, 9.6m, 10.1m, 10.6m, 11.04m, 11.52m, 12m, 12.5m, 12.96m, 13.44m, 13.92m, 14.4m, 15.6m, 16.8m, 18m, 19.2m },
            /* 2.4 */ { 2.9m, 3.4m, 3.9m, 4.32m, 4.8m, 5.3m, 5.8m, 6.24m, 6.72m, 7.2m, 7.7m, 8.2m, 8.7m, 9.12m, 9.6m, 10.1m, 10.6m, 11.04m, 11.52m, 12m, 12.5m, 12.96m, 13.44m, 13.92m, 14.4m, 15.6m, 16.8m, 18m, 19.2m },
            /* 2.5 */ { 2.9m, 3.4m, 3.9m, 4.32m, 4.8m, 5.3m, 5.8m, 6.24m, 6.72m, 7.2m, 7.7m, 8.2m, 8.7m, 9.12m, 9.6m, 10.1m, 10.6m, 11.04m, 11.52m, 12m, 12.5m, 12.96m, 13.44m, 13.92m, 14.4m, 15.6m, 16.8m, 18m, 19.2m },
            /* 2.6 */ { 3m, 4.5m, 5.2m, 5.4m, 5.6m, 5.8m, 6m, 9m, 9m, 9m, 9m, 9m, 9m, 12m, 12m, 12m, 12m, 12m, 12m, 12m, 15m, 15m, 15m, 15m, 15m, 18m, 18m, 18m, 18m },
            /* 2.7 */ { 3.1m, 5.1m, 6.2m, 6.2m, 6.2m, 6.2m, 6.2m, 9.3m, 9.3m, 9.3m, 9.3m, 9.3m, 9.3m, 12.4m, 12.4m, 12.4m, 12.4m, 12.4m, 12.4m, 12.4m, 15.5m, 15.5m, 15.5m, 15.5m, 15.5m, 18.6m, 18.6m, 18.6m, 18.6m },
            /* 2.8 */ { 3.2m, 5.2m, 6.4m, 6.4m, 6.4m, 6.4m, 6.4m, 9.6m, 9.6m, 9.6m, 9.6m, 9.6m, 9.6m, 12.8m, 12.8m, 12.8m, 12.8m, 12.8m, 12.8m, 12.8m, 16m, 16m, 16m, 16m, 16m, 19.2m, 19.2m, 19.2m, 19.2m },
            /* 2.9 */ { 3.3m, 5.3m, 6.6m, 6.6m, 6.6m, 6.6m, 6.6m, 9.9m, 9.9m, 9.9m, 9.9m, 9.9m, 9.9m, 13.2m, 13.2m, 13.2m, 13.2m, 13.2m, 13.2m, 13.2m, 16.5m, 16.5m, 16.5m, 16.5m, 16.5m, 19.8m, 19.8m, 19.8m, 19.8m },
            /* 3.0 */ { 3.4m, 5.4m, 6.8m, 6.8m, 6.8m, 6.8m, 6.8m, 10.2m, 10.2m, 10.2m, 10.2m, 10.2m, 10.2m, 13.6m, 13.6m, 13.6m, 13.6m, 13.6m, 13.6m, 13.6m, 17m, 17m, 17m, 17m, 17m, 20.4m, 20.4m, 20.4m, 20.4m },
            /* 3.1 */ { 3.5m, 5.5m, 7m, 7m, 7m, 7m, 7m, 10.5m, 10.5m, 10.5m, 10.5m, 10.5m, 10.5m, 14m, 14m, 14m, 14m, 14m, 14m, 14m, 17.5m, 17.5m, 17.5m, 17.5m, 17.5m, 21m, 21m, 21m, 21m },
            /* 3.2 */ { 3.6m, 6m, 7.2m, 7.2m, 7.2m, 7.2m, 7.2m, 10.8m, 10.8m, 10.8m, 10.8m, 10.8m, 10.8m, 14.4m, 14.4m, 14.4m, 14.4m, 14.4m, 14.4m, 14.4m, 18m, 18m, 18m, 18m, 18m, 21.6m, 21.6m, 21.6m, 21.6m },
            /* 3.3 */ { 3.7m, 6.1m, 7.4m, 7.4m, 7.4m, 7.4m, 7.4m, 11.1m, 11.1m, 11.1m, 11.1m, 11.1m, 11.1m, 14.8m, 14.8m, 14.8m, 14.8m, 14.8m, 14.8m, 14.8m, 18.5m, 18.5m, 18.5m, 18.5m, 18.5m, 22.2m, 22.2m, 22.2m, 22.2m },
            /* 3.4 */ { 3.8m, 6.2m, 7.6m, 7.6m, 7.6m, 7.6m, 7.6m, 11.4m, 11.4m, 11.4m, 11.4m, 11.4m, 11.4m, 15.2m, 15.2m, 15.2m, 15.2m, 15.2m, 15.2m, 15.2m, 19m, 19m, 19m, 19m, 19m, 22.8m, 22.8m, 22.8m, 22.8m },
            /* 3.5 */ { 3.9m, 6.3m, 7.8m, 7.8m, 7.8m, 7.8m, 7.8m, 11.7m, 11.7m, 11.7m, 11.7m, 11.7m, 11.7m, 15.6m, 15.6m, 15.6m, 15.6m, 15.6m, 15.6m, 15.6m, 19.5m, 19.5m, 19.5m, 19.5m, 19.5m, 23.4m, 23.4m, 23.4m, 23.4m },
            /* 3.6 */ { 4m, 7m, 8m, 8m, 8m, 8m, 8m, 12m, 12m, 12m, 12m, 12m, 12m, 16m, 16m, 16m, 16m, 16m, 16m, 16m, 20m, 20m, 20m, 20m, 20m, 24m, 24m, 24m, 24m },
            /* 3.7 */ { 4.1m, 7.2m, 8.2m, 8.2m, 8.2m, 8.2m, 8.2m, 12.3m, 12.3m, 12.3m, 12.3m, 12.3m, 12.3m, 16.4m, 16.4m, 16.4m, 16.4m, 16.4m, 16.4m, 16.4m, 20.5m, 20.5m, 20.5m, 20.5m, 20.5m, 24.6m, 24.6m, 24.6m, 24.6m },
            /* 3.8 */ { 4.2m, 7.6m, 8.4m, 8.4m, 8.4m, 8.4m, 8.4m, 12.6m, 12.6m, 12.6m, 12.6m, 12.6m, 12.6m, 16.8m, 16.8m, 16.8m, 16.8m, 16.8m, 16.8m, 16.8m, 21m, 21m, 21m, 21m, 21m, 25.2m, 25.2m, 25.2m, 25.2m },
            /* 3.9 */ { 4.3m, 7.8m, 8.6m, 8.6m, 8.6m, 8.6m, 8.6m, 12.9m, 12.9m, 12.9m, 12.9m, 12.9m, 12.9m, 17.2m, 17.2m, 17.2m, 17.2m, 17.2m, 17.2m, 17.2m, 21.5m, 21.5m, 21.5m, 21.5m, 21.5m, 25.8m, 25.8m, 25.8m, 25.8m },
            /* 4.0 */ { 4.4m, 8m, 8.8m, 8.8m, 8.8m, 8.8m, 8.8m, 13.2m, 13.2m, 13.2m, 13.2m, 13.2m, 13.2m, 17.6m, 17.6m, 17.6m, 17.6m, 17.6m, 17.6m, 17.6m, 22m, 22m, 22m, 22m, 22m, 26.4m, 26.4m, 26.4m, 26.4m },
            /* 4.1 */ { 4.5m, 8.2m, 9m, 9m, 9m, 9m, 9m, 13.5m, 13.5m, 13.5m, 13.5m, 13.5m, 13.5m, 18m, 18m, 18m, 18m, 18m, 18m, 18m, 22.5m, 22.5m, 22.5m, 22.5m, 22.5m, 27m, 27m, 27m, 27m },
            /* 4.2 */ { 4.6m, 8.4m, 9.2m, 9.2m, 9.2m, 9.2m, 9.2m, 13.8m, 13.8m, 13.8m, 13.8m, 13.8m, 13.8m, 18.4m, 18.4m, 18.4m, 18.4m, 18.4m, 18.4m, 18.4m, 23m, 23m, 23m, 23m, 23m, 27.6m, 27.6m, 27.6m, 27.6m },
            /* 4.3 */ { 4.7m, 8.6m, 9.4m, 9.4m, 9.4m, 9.4m, 9.4m, 14.1m, 14.1m, 14.1m, 14.1m, 14.1m, 14.1m, 18.8m, 18.8m, 18.8m, 18.8m, 18.8m, 18.8m, 18.8m, 23.5m, 23.5m, 23.5m, 23.5m, 23.5m, 28.2m, 28.2m, 28.2m, 28.2m },
            /* 4.4 */ { 4.8m, 8.8m, 9.6m, 9.6m, 9.6m, 9.6m, 9.6m, 14.4m, 14.4m, 14.4m, 14.4m, 14.4m, 14.4m, 19.2m, 19.2m, 19.2m, 19.2m, 19.2m, 19.2m, 19.2m, 24m, 24m, 24m, 24m, 24m, 28.8m, 28.8m, 28.8m, 28.8m },
            /* 4.5 */ { 4.9m, 9m, 9.8m, 9.8m, 9.8m, 9.8m, 9.8m, 14.7m, 14.7m, 14.7m, 14.7m, 14.7m, 14.7m, 19.6m, 19.6m, 19.6m, 19.6m, 19.6m, 19.6m, 19.6m, 24.5m, 24.5m, 24.5m, 24.5m, 24.5m, 29.4m, 29.4m, 29.4m, 29.4m },
            /* 5.0 */ { 5.4m, 10.8m, 10.8m, 10.8m, 10.8m, 10.8m, 10.8m, 16.2m, 16.2m, 16.2m, 16.2m, 16.2m, 16.2m, 21.6m, 21.6m, 21.6m, 21.6m, 21.6m, 21.6m, 21.6m, 27m, 27m, 27m, 27m, 27m, 32.4m, 32.4m, 32.4m, 32.4m },
            /* 5.5 */ { 5.9m, 11.8m, 11.8m, 11.8m, 11.8m, 11.8m, 11.8m, 17.2m, 17.2m, 17.2m, 17.2m, 17.2m, 17.2m, 23.6m, 23.6m, 23.6m, 23.6m, 23.6m, 23.6m, 23.6m, 29.5m, 29.5m, 29.5m, 29.5m, 29.5m, 35.4m, 35.4m, 35.4m, 35.4m },
            /* 6.0 */ { 6.4m, 12.8m, 12.8m, 12.8m, 12.8m, 12.8m, 12.8m, 19.2m, 19.2m, 19.2m, 19.2m, 19.2m, 19.2m, 25.6m, 25.6m, 25.6m, 25.6m, 25.6m, 25.6m, 25.6m, 32m, 32m, 32m, 32m, 32m, 38.4m, 38.4m, 38.4m, 38.4m },
            /* 6.5 */ { 6.9m, 13.8m, 13.8m, 13.8m, 13.8m, 13.8m, 13.8m, 20.7m, 20.7m, 20.7m, 20.7m, 20.7m, 20.7m, 27.6m, 27.6m, 27.6m, 27.6m, 27.6m, 27.6m, 27.6m, 34.5m, 34.5m, 34.5m, 34.5m, 34.5m, 41.4m, 41.4m, 41.4m, 41.4m },
            /* 7.0 */ { 7.4m, 14.8m, 14.8m, 14.8m, 14.8m, 14.8m, 14.8m, 22.2m, 22.2m, 22.2m, 22.2m, 22.2m, 22.2m, 29.6m, 29.6m, 29.6m, 29.6m, 29.6m, 29.6m, 29.6m, 37m, 37m, 37m, 37m, 37m, 44.4m, 44.4m, 44.4m, 44.4m },
            /* 7.5 */ { 7.9m, 15.8m, 15.8m, 15.8m, 15.8m, 15.8m, 15.8m, 23.7m, 23.7m, 23.7m, 23.7m, 23.7m, 23.7m, 31.6m, 31.6m, 31.6m, 31.6m, 31.6m, 31.6m, 31.6m, 39.5m, 39.5m, 39.5m, 39.5m, 39.5m, 47.4m, 47.4m, 47.4m, 47.4m },
            /* 8.0 */ { 8.4m, 16.8m, 16.8m, 16.8m, 16.8m, 16.8m, 16.8m, 25.2m, 25.2m, 25.2m, 25.2m, 25.2m, 25.2m, 35.6m, 35.6m, 35.6m, 35.6m, 35.6m, 35.6m, 35.6m, 42m, 42m, 42m, 42m, 42m, 50.4m, 50.4m, 50.4m, 50.4m },
            /* 8.5 */ { 8.9m, 17.8m, 17.8m, 17.8m, 17.8m, 17.8m, 17.8m, 26.7m, 26.7m, 26.7m, 26.7m, 26.7m, 26.7m, 33.6m, 33.6m, 33.6m, 33.6m, 33.6m, 33.6m, 33.6m, 44.5m, 44.5m, 44.5m, 44.5m, 44.5m, 53.4m, 53.4m, 53.4m, 53.4m },
            /* 9.0 */ { 9.4m, 18.8m, 18.8m, 18.8m, 18.8m, 18.8m, 18.8m, 28.2m, 28.2m, 28.2m, 28.2m, 28.2m, 28.2m, 37.6m, 37.6m, 37.6m, 37.6m, 37.6m, 37.6m, 37.6m, 47m, 47m, 47m, 47m, 47m, 56.4m, 56.4m, 56.4m, 56.4m }
        };

        /// <summary>
        /// Gets the fabric usage for given width and height values.
        /// Returns the fabric usage based on the range the values fall into.
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
            for (int i = 0; i < DimensionRanges.LengthRanges.Count; i++)
            {
                var range = DimensionRanges.LengthRanges[i];
                if (height >= range.Min && height <= range.Max)
                    return i;
            }
            // Default to last range if exceeds all
            return DimensionRanges.LengthRanges.Count - 1;
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
        /// <param name="heightRangeIndex">Index in DimensionRanges.LengthRanges</param>
        /// <returns>Fabric usage in meters</returns>
        public static decimal GetFabricUsageByRangeIndex(int widthRangeIndex, int heightRangeIndex)
        {
            // Get the min value of each range to look up in the fabric matrix
            var widthRange = DimensionRanges.WidthRanges[widthRangeIndex];
            var heightRange = DimensionRanges.LengthRanges[heightRangeIndex];

            return GetFabricUsage(widthRange.Min, heightRange.Min);
        }
    }
}
