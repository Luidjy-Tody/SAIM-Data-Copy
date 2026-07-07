using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SaimDataCopy.Styles.Authentification
{
    public class AuthGradientPanel : Panel
    {
        public AuthGradientPanel()
        {
            DoubleBuffered = true;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            using LinearGradientBrush brush = new LinearGradientBrush(
                ClientRectangle,
                AuthentificationFormStyle.BleuPrincipal,
                AuthentificationFormStyle.BleuFonce,
                135f);

            ColorBlend blend = new ColorBlend
            {
                Colors = new[]
                {
                    AuthentificationFormStyle.BleuPrincipal,
                    AuthentificationFormStyle.BleuSurvol,
                    AuthentificationFormStyle.BleuFonce
                },

                Positions = new[]
                {
                    0f,
                    0.60f,
                    1f
                }
            };

            brush.InterpolationColors = blend;

            e.Graphics.FillRectangle(brush, ClientRectangle);
        }
    }
}