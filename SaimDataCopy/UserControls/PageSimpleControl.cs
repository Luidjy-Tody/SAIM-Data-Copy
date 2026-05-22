using System.Drawing;
using System.Windows.Forms;


namespace SaimDataCopy.UserControls
{
    // Page simple utilisée temporairement pour tester l'affichage dans panelMain
    public class PageSimpleControl : UserControl
    {
        public PageSimpleControl(string titrePage)
        {
            // Couleur de fond de la page.
            this.BackColor = Color.White;

            // Le scroll sera seulement dans la page centrale.
            this.AutoScroll = true;

            // Création du titre de la page.
            Label lblTitre = new Label();

            lblTitre.Text = titrePage;
            lblTitre.AutoSize = true;
            lblTitre.Location = new Point(30, 30);

            lblTitre.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitre.ForeColor = Color.FromArgb(30, 30, 30);

            // Ajout du titre dans la page.
            this.Controls.Add(lblTitre);
        }
    }
}
