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

        public static Rectangle TranslateSelectionToImage(PictureBox pictureBox, Rectangle selection)
        {
            if (pictureBox.Image == null)
                return Rectangle.Empty;

            Image img = pictureBox.Image;

            switch (pictureBox.SizeMode)
            {
                case PictureBoxSizeMode.Normal:
                case PictureBoxSizeMode.AutoSize:
                    // Image is shown at original size, no scaling
                    return Rectangle.Intersect(selection, new Rectangle(0, 0, img.Width, img.Height));

                case PictureBoxSizeMode.StretchImage:
                    {
                        // Image is stretched to PictureBox
                        float scaleX = (float)img.Width / pictureBox.Width;
                        float scaleY = (float)img.Height / pictureBox.Height;
                        return new Rectangle(
                            (int)(selection.X * scaleX),
                            (int)(selection.Y * scaleY),
                            (int)(selection.Width * scaleX),
                            (int)(selection.Height * scaleY)
                        );
                    }

                case PictureBoxSizeMode.CenterImage:
                    {
                        // Image is centered in PictureBox at original size
                        int offsetX = (pictureBox.Width - img.Width) / 2;
                        int offsetY = (pictureBox.Height - img.Height) / 2;
                        Rectangle imgRect = new Rectangle(
                            selection.X - offsetX,
                            selection.Y - offsetY,
                            selection.Width,
                            selection.Height
                        );
                        return Rectangle.Intersect(imgRect, new Rectangle(0, 0, img.Width, img.Height));
                    }

                case PictureBoxSizeMode.Zoom:
                default:
                    return TranslateSelectionToImage_Zoom(pictureBox, selection, img);
            }
        }

        private static Rectangle TranslateSelectionToImage_Zoom(PictureBox pictureBox, Rectangle selection, Image img)
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

            // Selection relative to drawn image rectangle
            Rectangle imageRect = new Rectangle(offsetX, offsetY, drawWidth, drawHeight);
            Rectangle intersect = Rectangle.Intersect(selection, imageRect);
            if (intersect.IsEmpty) return Rectangle.Empty;

            float scaleX = (float)img.Width / drawWidth;
            float scaleY = (float)img.Height / drawHeight;

            int imgX = (int)((intersect.X - offsetX) * scaleX);
            int imgY = (int)((intersect.Y - offsetY) * scaleY);
            int imgW = (int)(intersect.Width * scaleX);
            int imgH = (int)(intersect.Height * scaleY);

            return new Rectangle(imgX, imgY, imgW, imgH);
        }

    }
}
