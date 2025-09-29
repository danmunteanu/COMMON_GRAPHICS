
namespace CommonGraphics
{
    public class Renderer
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

        public virtual void Render()
        {
            if (Buffer == null)
                return;

            if (RenderChunk == null)
                return;

            //  Split render area in multiple chunks
            List<(Point start, Point end)>? chunks = null;
            Chunks.CreateRenderChunks(Buffer.Width, Buffer.Height, out chunks);

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
    }
}
