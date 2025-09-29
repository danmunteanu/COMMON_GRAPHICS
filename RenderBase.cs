namespace CommonGraphics
{
    public class RenderBase
    {
        public delegate void ProgressCallback(int percent);
        public delegate void ChunkRenderer(Point start, Point end, byte[] pixels, int stride);

        public Bitmap? Buffer { get; set; } = null;

        protected int _plottedPixels = 0;
        protected int _pixelsToPlotCount = 0;
        protected int _lastReportedPercent = 0;

        public TraversalOrder TraversalOrder { get; set; } = TraversalOrder.LeftToRight;

        //  Callback for update
        public ProgressCallback? Callback { get; set; } = null;
        public ChunkRenderer? RenderChunk { get; set; } = null;

        private static int _bufferColumns = 3;

        public virtual void Render()
        {
            if (Buffer == null)
                return;

            if (RenderChunk == null)
                return;

            //  Split render area in multiple chunks
            List<(Point start, Point end)>? chunks = null;
            SplitRenderArea(Buffer, out chunks);

            //  Something went wrong while chunking
            if (chunks == null)
                return;

            // Lock the bitmap once
            var rect = new Rectangle(0, 0, Buffer.Width, Buffer.Height);
            var bmpData = Buffer.LockBits(rect,
                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
            );

            IntPtr ptr = bmpData.Scan0;
            int stride = bmpData.Stride;
            byte[] pixels = new byte[stride * Buffer.Height];

            _plottedPixels = 0;
            _pixelsToPlotCount = Buffer.Width * Buffer.Height;
            _lastReportedPercent = 0;
            Callback?.Invoke(0);

            // Render all chunks in parallel
            Parallel.ForEach(chunks, chunk =>
            {
                RenderChunk(chunk.start, chunk.end, pixels, stride);
            });

            // Copy back into bitmap
            System.Runtime.InteropServices.Marshal.Copy(pixels, 0, ptr, pixels.Length);
            Buffer.UnlockBits(bmpData);

            // 100% progress
            Callback?.Invoke(100);
        }

        /*
         * Divides the bitmap buffer into rectangular chunks for parallel rendering.
         *
         * The image is split into a grid with a fixed number of columns and
         * as many rows as needed based on the processor count.
         *
         * Any leftover pixels (from integer division) are absorbed into the
         * last chunk in each row and column, ensuring full coverage of the
         * render area without gaps or overlaps.
         *
         * Example layout with _bufferColumns = 3 and procCount = 8:
         *
         *  +---------+---------+---------+
         *  | Chunk 0 | Chunk 1 | Chunk 2 |
         *  +---------+---------+---------+
         *  | Chunk 3 | Chunk 4 | Chunk 5 |
         *  +---------+---------+---------+
         *  | Chunk 6 | Chunk 7 | Chunk 8 |
         *  +---------+---------+---------+
         *
         * Each chunk is represented by its top-left (start) and
         * bottom-right (end) coordinates.
         */
        public static void SplitRenderArea(Bitmap buffer, out List<(Point start, Point end)>? renderAreas)
        {
            if (buffer == null)
            {
                renderAreas = null;
                return;
            }

            int procCount = Environment.ProcessorCount;

            // Compute rows and cols for a near-square tiling
            int cols = _bufferColumns;
            int rows = (int)Math.Ceiling(procCount / (double)cols);

            // Base chunk sizes
            int baseCellWidth = buffer.Width / cols;
            int baseCellHeight = buffer.Height / rows;

            renderAreas = new();

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    int x1 = col * baseCellWidth;
                    int y1 = row * baseCellHeight;

                    // Expand the last chunk in each row/col to absorb leftovers
                    int x2 = (col == cols - 1) ? buffer.Width : (x1 + baseCellWidth);
                    int y2 = (row == rows - 1) ? buffer.Height : (y1 + baseCellHeight);

                    Point start = new Point(x1, y1);
                    Point end = new Point(x2, y2);

                    renderAreas.Add((start, end));
                }
            }
        }


    }
}
