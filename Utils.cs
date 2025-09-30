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
    }
}
