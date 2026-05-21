using FontAwesome.Sharp;

namespace SaimDataCopy.Helpers
{
    // Classe séparée qui contient le design des boutons du menu
    public static class MenuButtonStyle
    {
        // Applique le même style à chaque bouton du menu
        //Icon bouton
        public static void Appliquer(IconButton bouton)
        {
            bouton.IconSize = 20;
            bouton.IconColor = Color.FromArgb(80, 80, 80);

            bouton.Dock = DockStyle.Top;
            bouton.Height = 50;

            bouton.TextAlign = ContentAlignment.MiddleLeft;
            bouton.ImageAlign = ContentAlignment.MiddleLeft;
            bouton.TextImageRelation = TextImageRelation.ImageBeforeText;

            bouton.Padding = new Padding(20, 0, 0, 0);

            bouton.FlatStyle = FlatStyle.Flat;
            bouton.FlatAppearance.BorderSize = 0;
            bouton.BackColor = Color.FromArgb(245, 245, 245);
            bouton.ForeColor = Color.FromArgb(30, 30, 30);
            bouton.Font = new Font("Segoe UI", 10);
        }

        //Bouton Enregistrer Parametres
        public static void Appliquer(Button btnEnregistrerParametres)
        {
            btnEnregistrerParametres.Dock = DockStyle.Right;
            btnEnregistrerParametres.Width = 220;
            btnEnregistrerParametres.Height = 35;
            btnEnregistrerParametres.BackColor = Color.FromArgb(30, 96, 190);
            btnEnregistrerParametres.ForeColor = Color.White;
            btnEnregistrerParametres.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnEnregistrerParametres.FlatStyle = FlatStyle.Flat;
            btnEnregistrerParametres.FlatAppearance.BorderSize = 1;
        }


    }

    // Applique le même style à chaque Label du menu

    public static class MenuLabelStyle
    {
        public static void Appliquer(Label lblStatus)
        {
            lblStatus.ForeColor = Color.FromArgb(30, 30, 30); ;
            lblStatus.Font = new("Segoe UI", 10);
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            lblStatus.Dock = DockStyle.Left;
            lblStatus.Width = 120;
            lblStatus.Padding = new(15, 0, 0, 0);
        }

    }

}