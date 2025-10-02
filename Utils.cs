namespace CommonGraphics
{
    public class Utils
    {
        public static bool AddUserControlToPanel(Panel panel, UserControl control)
        {
            if (panel.Controls.Contains(control))
                return false;

            if (control == null)
                return false;

            control.Dock = DockStyle.Fill;
            panel.Controls.Clear();
            panel.Controls.Add(control);
            control.BringToFront();

            return true;
        }

        public static void NumericUpDown_SelectAll(object? sender, EventArgs e)
        {
            if (sender is NumericUpDown nud)
            {
                // Get the internal TextBox
                var tb = nud.Controls[1] as TextBox;
                tb?.SelectAll();
            }
        }

        public static void NumericUpDown_MouseUp(object? sender, MouseEventArgs e)
        {
            if (sender is NumericUpDown nud)
            {
                var tb = nud.Controls[1] as TextBox;
                if (tb != null && !tb.Focused)
                    return;

                // Select all text only if no selection exists
                if (tb != null && tb.SelectionLength == 0)
                {
                    tb.SelectAll();
                }
            }
        }

        public static Point TranslatePointToImage(PictureBox pictureBox, Point point)
        {
            if (pictureBox.Image == null)
                return Point.Empty;

            Image img = pictureBox.Image;

            switch (pictureBox.SizeMode)
            {
                case PictureBoxSizeMode.Normal:
                case PictureBoxSizeMode.AutoSize:
                    // No scaling, just clamp inside image bounds
                    return new Point(
                        Math.Max(0, Math.Min(img.Width - 1, point.X)),
                        Math.Max(0, Math.Min(img.Height - 1, point.Y))
                    );

                case PictureBoxSizeMode.StretchImage:
                    {
                        // Image stretched to fit PictureBox
                        float scaleX = (float)img.Width / pictureBox.Width;
                        float scaleY = (float)img.Height / pictureBox.Height;

                        int imgX = (int)(point.X * scaleX);
                        int imgY = (int)(point.Y * scaleY);

                        return new Point(
                            Math.Max(0, Math.Min(img.Width - 1, imgX)),
                            Math.Max(0, Math.Min(img.Height - 1, imgY))
                        );
                    }

                case PictureBoxSizeMode.CenterImage:
                    {
                        // Image centered, no scaling
                        int offsetX = (pictureBox.Width - img.Width) / 2;
                        int offsetY = (pictureBox.Height - img.Height) / 2;

                        int imgX = point.X - offsetX;
                        int imgY = point.Y - offsetY;

                        return new Point(
                            Math.Max(0, Math.Min(img.Width - 1, imgX)),
                            Math.Max(0, Math.Min(img.Height - 1, imgY))
                        );
                    }

                case PictureBoxSizeMode.Zoom:
                default:
                    return TranslatePointToImage_Zoom(pictureBox, point, img);
            }
        }

        private static Point TranslatePointToImage_Zoom(PictureBox pictureBox, Point point, Image img)
        {
            float imageAspect = (float)img.Width / img.Height;
            float boxAspect = (float)pictureBox.Width / pictureBox.Height;

            int drawWidth, drawHeight;
            int offsetX, offsetY;

            if (imageAspect > boxAspect)
            {
                // Image is wider: letterbox top/bottom
                drawWidth = pictureBox.Width;
                drawHeight = (int)(pictureBox.Width / imageAspect);
                offsetX = 0;
                offsetY = (pictureBox.Height - drawHeight) / 2;
            }
            else
            {
                // Image is taller: pillarbox left/right
                drawHeight = pictureBox.Height;
                drawWidth = (int)(pictureBox.Height * imageAspect);
                offsetX = (pictureBox.Width - drawWidth) / 2;
                offsetY = 0;
            }

            Rectangle imageRect = new Rectangle(offsetX, offsetY, drawWidth, drawHeight);
            if (!imageRect.Contains(point))
                return Point.Empty; // Point outside actual image area

            float scaleX = (float)img.Width / drawWidth;
            float scaleY = (float)img.Height / drawHeight;

            int imgX = (int)((point.X - offsetX) * scaleX);
            int imgY = (int)((point.Y - offsetY) * scaleY);

            return new Point(
                Math.Max(0, Math.Min(img.Width - 1, imgX)),
                Math.Max(0, Math.Min(img.Height - 1, imgY))
            );
        }

    }
}
