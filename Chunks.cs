using System;
using System.Drawing;
using System.Collections.Generic;


namespace CommonGraphics
{
    public class Chunks
    {
        private static int _columns = 3;

        public static void CreateRenderChunks(int width, int height, out List<(Point start, Point end)> chunks)
        {
            int procCount = Environment.ProcessorCount;

            // Compute rows based on processors
            int rows = (int)Math.Ceiling(procCount / (double)_columns);

            int cellWidth = width / _columns;
            int cellHeight = height / rows;

            chunks = new();

            //  Create chunks
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < _columns; col++)
                {
                    int x1 = col * cellWidth;
                    int y1 = row * cellHeight;

                    int x2 = (col == _columns - 1) ? width : (x1 + cellWidth);
                    int y2 = (row == rows - 1) ? height : (y1 + cellHeight);

                    Point start = new Point(x1, y1);
                    Point end = new Point(x2, y2);

                    chunks.Add((start, end));

                    if (chunks.Count == procCount)
                        break; // stop if we've scheduled enough
                }
                if (chunks.Count == procCount)
                    break;
            }
        }
    }
}
