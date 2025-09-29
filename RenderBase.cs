
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
            CreateRenderAreas(Buffer, out chunks);

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

        public static void CreateRenderAreas(Bitmap buffer, out List<(Point start, Point end)>? renderAreas)
        {
            if (buffer == null)
            {
                renderAreas = null;
                return;
            }

            int procCount = Environment.ProcessorCount;

            // Compute rows based on processors
            int rows = (int)Math.Ceiling(procCount / (double)_bufferColumns);

            int cellWidth = buffer.Width / _bufferColumns;
            int cellHeight = buffer.Height/ rows;

            renderAreas = new();

            //  Create chunks
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < _bufferColumns; col++)
                {
                    int x1 = col * cellWidth;
                    int y1 = row * cellHeight;

                    int x2 = (col == _bufferColumns - 1) ? buffer.Width : (x1 + cellWidth);
                    int y2 = (row == rows - 1) ? buffer.Height : (y1 + cellHeight);

                    Point start = new Point(x1, y1);
                    Point end = new Point(x2, y2);

                    renderAreas.Add((start, end));

                    if (renderAreas.Count == procCount)
                        break; // stop if we've scheduled enough
                }
                if (renderAreas.Count == procCount)
                    break;
            }
        }

    }
}
