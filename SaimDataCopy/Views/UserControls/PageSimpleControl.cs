using SaimDataCopy.Helpers;
using System.Drawing;
using System.Windows.Forms;

namespace SaimDataCopy.Views.UserControls
{
    // View simple utilisée temporairement pour les pages pas encore développées.
    // Exemple : Historique, Exécution, Paramètres Email, etc.
    public class PageSimpleControl : UserControl
    {
        public PageSimpleControl(string titrePage)
        {
            // Fond blanc de la page.
            this.BackColor = Color.White;

            // Le scroll reste seulement dans la page centrale.
            this.AutoScroll = true;

            // Titre affiché au centre de la page.
            Label lblTitre = new Label();

            lblTitre.Text = titrePage;
            lblTitre.Location = new Point(30, 30);

            // Style du titre depuis Helpers/PageFormStyle.cs
            PageFormStyle.AppliquerTitre(lblTitre);

            this.Controls.Add(lblTitre);
        }
    }
}