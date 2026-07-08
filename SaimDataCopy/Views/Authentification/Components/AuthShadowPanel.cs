using System.Drawing.Drawing2D;

namespace SaimDataCopy.Views.Authentification.Components
{
    public class AuthShadowPanel : Panel
    {
        public AuthShadowPanel()
        {
            BackColor = Color.White;
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using GraphicsPath chemin = new GraphicsPath();

            int rayon = 18;
            Rectangle rectangle = new Rectangle(0, 0, Width - 1, Height - 1);

            chemin.AddArc(rectangle.X, rectangle.Y, rayon, rayon, 180, 90);
            chemin.AddArc(rectangle.Right - rayon, rectangle.Y, rayon, rayon, 270, 90);
            chemin.AddArc(rectangle.Right - rayon, rectangle.Bottom - rayon, rayon, rayon, 0, 90);
            chemin.AddArc(rectangle.X, rectangle.Bottom - rayon, rayon, rayon, 90, 90);
            chemin.CloseFigure();

            Region = new Region(chemin);

            using SolidBrush fond = new SolidBrush(Color.White);
            e.Graphics.FillPath(fond, chemin);

            using Pen bordure = new Pen(Color.FromArgb(226, 232, 240), 1);
            e.Graphics.DrawPath(bordure, chemin);
        }
    }
}