using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SaimDataCopy.Styles.Authentification
{
    public class AuthShadowPanel : Panel
    {
        private const int Rayon = 18;
        private const int TailleOmbre = 10;

        public AuthShadowPanel()
        {
            DoubleBuffered = true;
            BackColor = Color.Transparent;
            Padding = new Padding(TailleOmbre);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rectangleOmbre = new Rectangle(
                TailleOmbre,
                TailleOmbre,
                Width - TailleOmbre * 2,
                Height - TailleOmbre * 2
            );

            Rectangle rectangleCarte = new Rectangle(
                TailleOmbre / 2,
                TailleOmbre / 2,
                Width - TailleOmbre * 2,
                Height - TailleOmbre * 2
            );

            using GraphicsPath cheminOmbre = CreerRectangleArrondi(rectangleOmbre, Rayon);
            using SolidBrush brosseOmbre = new SolidBrush(Color.FromArgb(45, 0, 0, 0));
            e.Graphics.FillPath(brosseOmbre, cheminOmbre);

            using GraphicsPath cheminCarte = CreerRectangleArrondi(rectangleCarte, Rayon);
            using SolidBrush brosseCarte = new SolidBrush(Color.White);
            e.Graphics.FillPath(brosseCarte, cheminCarte);
        }

        private static GraphicsPath CreerRectangleArrondi(Rectangle rectangle, int rayon)
        {
            GraphicsPath chemin = new GraphicsPath();

            int diametre = rayon * 2;

            chemin.AddArc(rectangle.X, rectangle.Y, diametre, diametre, 180, 90);
            chemin.AddArc(rectangle.Right - diametre, rectangle.Y, diametre, diametre, 270, 90);
            chemin.AddArc(rectangle.Right - diametre, rectangle.Bottom - diametre, diametre, diametre, 0, 90);
            chemin.AddArc(rectangle.X, rectangle.Bottom - diametre, diametre, diametre, 90, 90);

            chemin.CloseFigure();

            return chemin;
        }
    }
}