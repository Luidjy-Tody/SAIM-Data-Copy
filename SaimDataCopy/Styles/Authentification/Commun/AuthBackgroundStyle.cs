using System.Drawing;
using System.Drawing.Drawing2D;

namespace SaimDataCopy.Styles.Authentification.Commun
{
    public static class AuthBackgroundStyle
    {
        public static readonly Color BleuPrincipal = Color.FromArgb(15, 132, 145);
        public static readonly Color BleuSecondaire = Color.FromArgb(19, 124, 115);
        public static readonly Color BleuFonce = Color.FromArgb(14, 111, 112);

        public static void DessinerFond(Graphics graphics, Rectangle rectangle)
        {
            if (rectangle.Width <= 0 || rectangle.Height <= 0)
            {
                return;
            }

            using LinearGradientBrush brush = new LinearGradientBrush(
                rectangle,
                BleuPrincipal,
                BleuFonce,
                135f
            );

            ColorBlend blend = new ColorBlend
            {
                Colors =
                [
                    BleuPrincipal,
                    BleuSecondaire,
                    BleuFonce
                ],
                Positions =
                [
                    0f,
                    0.55f,
                    1f
                ]
            };

            brush.InterpolationColors = blend;
            graphics.FillRectangle(brush, rectangle);
        }
    }
}