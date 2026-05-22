using SaimDataCopy.Helpers;
using System.Drawing;
using System.Windows.Forms;
using FontAwesome.Sharp;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace SaimDataCopy.UserControls
{
    // Page des paramètres de configuration.
    // Elle sera affichée dans panelMain depuis MainForm.

    public class ConfigurationControl : UserControl
    {
        public ConfigurationControl()
        {
            // Fond blanc de la page
            this.BackColor = Color.White;

            //Le scroll est seulement dans cette page
            //Le menu gauche et la barre restent fixes

            this.AutoScroll = true;

            CreerInterface();
        }

        private void CreerInterface()
        {
            //Panel principal qui contient tous les champs.
            Panel panelContenu = new Panel();

            panelContenu.Dock = DockStyle.Top;
            panelContenu.Height = 900;
            panelContenu.BackColor = Color.White;

            this.Controls.Add(panelContenu);

            //Titre principal.
            Label lblTitre = new Label();
            lblTitre.Text = "Paramètres de configuration";
            lblTitre.Location = new Point(25, 25);

            PageFormStyle.AppliquerTitre(lblTitre);
            panelContenu.Controls.Add(lblTitre);

            //Section serveur source.
            CreerSectionServeurSource(
                panelContenu,
                "Serveur source (production)",
                25,
                85
            );

            //Section serveur cible.
            CreerSectionServeurCible(
                panelContenu,
                "Serveur cible (staging)",
                25,
                365
            );

            // Section comportement en cas d'erreur.
            CreerSectionErreur(panelContenu, 650
                );

        }


        private void CreerSectionServeurSource(Panel parent, string titre, int x, int y)
        {
            Label lblSection = new Label();
            lblSection.Text = titre;
            lblSection.Location = new Point(x, y);

            PageFormStyle.AppliquerSousTitre(lblSection);
            parent.Controls.Add(lblSection);

            // Ligne 1 : Nom serveur + chaîne de connexion.
            AjouterLabelChamp(parent, "Nom du serveur", x, y + 45, true);
            AjouterTextBox(parent, "PROD-SRV-01", x, y + 70, 475);

            AjouterLabelChamp(parent, "Chaîne de connexion", x + 500, y + 45, false);
            AjouterTextBox(parent, "Server=...", x + 500, y + 70, 475);


            //Ligne 2 : Identifiant + mot de passe.
            AjouterLabelChamp(parent, "Identifiant", x, y + 130, false);
            AjouterTextBox(parent, "sa", x, y + 155, 475);

            AjouterLabelChamp(parent, "Mot de passe", x + 500, y + 130, false);
            AjouterPasswordBox(parent, "12345678", x + 500, y + 155, 475);

            //Ligne 2 : Port

            AjouterLabelChamp(parent, "Port", x, y + 215, false);
            AjouterTextBox(parent, "1433", x, y + 240, 475);
        }



        private void CreerSectionServeurCible(Panel parent, string titre, int x, int y)
        {
            Label lblSection = new Label();
            lblSection.Text = titre;
            lblSection.Location = new Point(x, y);

            PageFormStyle.AppliquerSousTitre(lblSection);
            parent.Controls.Add(lblSection);

            // Ligne 1 : Nom serveur + chaîne de connexion.
            AjouterLabelChamp(parent, "Nom du serveur", x, y + 45, true);
            AjouterTextBox(parent, "STAGING-SRV-01", x, y + 70, 475);

            AjouterLabelChamp(parent, "Chaîne de connexion", x + 500, y + 45, false);
            AjouterTextBox(parent, "Server=...", x + 500, y + 70, 475);


            //Ligne 2 : Identifiant + mot de passe.
            AjouterLabelChamp(parent, "Identifiant", x, y + 130, false);
            AjouterTextBox(parent, "sa", x, y + 155, 475);

            AjouterLabelChamp(parent, "Mot de passe", x + 500, y + 130, false);
            AjouterPasswordBox(parent, "12345678", x + 500, y + 155, 475);

            //Ligne 2 : Port

            AjouterLabelChamp(parent, "Port", x, y + 215, false);
            AjouterTextBox(parent, "1433", x, y + 240, 475);

            AjouterLabelChamp(parent, "Mode de copie", x + 500, y + 215, false);

            ComboBox cmbModeCopie = new ComboBox();
            cmbModeCopie.Location = new Point(x + 500, y + 240);
            cmbModeCopie.Width = 475;

            cmbModeCopie.Items.Add("Écraser");
            cmbModeCopie.Items.Add("Mettre à jour");
            cmbModeCopie.SelectedIndex = 0;

            PageFormStyle.AppliquerComboBox(cmbModeCopie);
            parent.Controls.Add(cmbModeCopie);
        }



        private void CreerSectionErreur(Panel parent, int y)
        {
            Label lblSection = new Label();
            lblSection.Text = "Comportement en cas d'erreur";
            lblSection.Location = new Point(25, y);

            PageFormStyle.AppliquerSousTitre(lblSection);
            parent.Controls.Add(lblSection);

            // ComboBox : action si une base échoue.
            AjouterLabelChamp(parent, "Si une base échoue", 25, y + 45, false);

            ComboBox cmbErreur = new ComboBox();
            cmbErreur.Location = new Point(25, y + 70);
            cmbErreur.Width = 475;

            cmbErreur.Items.Add("Continuer avec les autres");
            cmbErreur.Items.Add("Arrêter tous les traitements");
            cmbErreur.SelectedIndex = 0;

            PageFormStyle.AppliquerComboBox(cmbErreur);
            parent.Controls.Add(cmbErreur);

            // ComboBox : nombre de tentatives.
            AjouterLabelChamp(parent, "Tentatives de reprise", 525, y + 45, false);

            ComboBox cmbTentatives = new ComboBox();
            cmbTentatives.Location = new Point(525, y + 70);
            cmbTentatives.Width = 475;

            cmbTentatives.Items.Add("1 tentative");
            cmbTentatives.Items.Add("2 tentatives");
            cmbTentatives.Items.Add("3 tentatives");
            cmbTentatives.SelectedIndex = 0;

            PageFormStyle.AppliquerComboBox(cmbTentatives);
            parent.Controls.Add(cmbTentatives);

            // Si l'utilisateur choisit "Arrêter tous les traitements",
            // alors les tentatives de reprise ne sont plus utiles.
            cmbErreur.SelectedIndexChanged += (sender, e) =>
            {
                if (cmbErreur.SelectedItem?.ToString() == "Arrêter tous les traitements")
                {
                    cmbTentatives.Enabled = false;
                    cmbTentatives.Cursor = Cursors.No;
                }
                else
                {
                    cmbTentatives.Enabled = true;
                    cmbTentatives.Cursor = Cursors.Default;
                }
            };
        }

        private void AjouterLabelChamp(Panel parent, string texte, int x, int y, bool requis)
        {
            Label label = new Label();

            label.Text = texte;
            label.Location = new Point(x, y);

            PageFormStyle.AppliquerLabelChamp(label);
            parent.Controls.Add(label);

            // Si le champ est obligatoire, on ajoute le badge "requis".
            if (requis)
            {
                Label badge = PageFormStyle.CreerBadgeRequis();

                // On place le badge à côté du label.
                badge.Location = new Point(x + label.Width + 10, y - 2);

                parent.Controls.Add(badge);
            }
        }

        private TextBox AjouterTextBox(Panel parent, string texte, int x, int y, int largeur)
        {
            TextBox textBox = new TextBox();

            textBox.Text = texte;
            textBox.Location = new Point(x, y);
            textBox.Width = largeur;

            textBox.PlaceholderText = texte;
            textBox.Text = "";

            PageFormStyle.AppliquerTextBox(textBox);

            parent.Controls.Add(textBox);

            return textBox;
        }

        private void InitializeComponent()
        {

        }

        private void AjouterPasswordBox(Panel parent, string texte, int x, int y, int largeur)
        {
            // Panel utilisé pour mettre le TextBox et l'icône œil ensemble.
            Panel panelPassword = new Panel();

            panelPassword.Location = new Point(x, y);
            panelPassword.Width = largeur;
            panelPassword.Height = 38;
            panelPassword.BackColor = Color.White;
            panelPassword.BorderStyle = BorderStyle.FixedSingle;

            parent.Controls.Add(panelPassword);

            TextBox txtPassword = new TextBox();
            txtPassword.PlaceholderText = texte;
            txtPassword.Text = texte;
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.BorderStyle = BorderStyle.None;
            txtPassword.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            txtPassword.Location = new Point(10, 9);
            txtPassword.Width = largeur - 55;

            panelPassword.Controls.Add(txtPassword);

            IconButton btnVoirPassword = new IconButton();

            btnVoirPassword.IconChar = IconChar.Eye;
            btnVoirPassword.IconSize = 18;
            btnVoirPassword.IconColor = Color.FromArgb(80, 80, 80);

            btnVoirPassword.FlatStyle = FlatStyle.Flat;
            btnVoirPassword.FlatAppearance.BorderSize = 0;

            btnVoirPassword.BackColor = Color.White;
            btnVoirPassword.Width = 40;
            btnVoirPassword.Height = 34;
            btnVoirPassword.Location = new Point(largeur - 43, 1);

            btnVoirPassword.Click += (sender, e) =>
            {
                // Si le mot de passe est caché, on l'affiche.
                if (txtPassword.UseSystemPasswordChar)
                {
                    txtPassword.UseSystemPasswordChar = false;
                    btnVoirPassword.IconChar = IconChar.EyeSlash;
                }
                else
                {
                    txtPassword.UseSystemPasswordChar = true;
                    btnVoirPassword.IconChar = IconChar.Eye;
                }
            };

            panelPassword.Controls.Add(btnVoirPassword);


        }
    }
}
