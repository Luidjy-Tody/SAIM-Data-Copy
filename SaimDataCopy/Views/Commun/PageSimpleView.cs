using SaimDataCopy.Styles;
using System.Drawing;
using System.Windows.Forms;

namespace SaimDataCopy.Views.Commun
{
    // View simple utilisée temporairement pour les pages pas encore développées.
    // Exemple : Historique, Exécution, Paramètres Email, etc.
    public class PageSimpleView : UserControl
    {
        public PageSimpleView(string titrePage)
        {
            // Fond blanc de la page.
            BackColor = Color.White;

            // Le scroll reste seulement dans la page centrale.
            AutoScroll = true;

            // Titre affiché dans la page.
            Label lblTitre = new Label();

            lblTitre.Text = titrePage;
            lblTitre.Location = new Point(30, 30);

            // Style du titre depuis Helpers/PageFormStyle.cs
            PageFormStyle.AppliquerTitre(lblTitre);

            Controls.Add(lblTitre);
        }
    }
}