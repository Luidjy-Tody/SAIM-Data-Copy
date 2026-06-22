using FontAwesome.Sharp;
using SaimDataCopy.Models.Configuration;
using SaimDataCopy.Styles;
using System.Drawing;
using System.Windows.Forms;
using SaimDataCopy.Views.Commun;


namespace SaimDataCopy.Views.Configuration
{
    // View de la page Configuration.
    // Elle affiche l'interface et récupère les valeurs saisies.
    // Elle ne contient pas la logique métier.
    public class ConfigurationView : UserControl, IPageEnregistrable
    {
        private const int LargeurMinimumContenu = 1000;
        private const int HauteurChamp = 38;

        private readonly Panel panelContenu = new Panel();
        private readonly TableLayoutPanel layoutPrincipal = new TableLayoutPanel();

        // Champs du serveur source.
        private ComboBox cmbSourceTypeServeur = new ComboBox();
        private TextBox txtSourceNomServeur = new TextBox();
        private TextBox txtSourceChaineConnexion = new TextBox();
        private TextBox txtSourceIdentifiant = new TextBox();
        private TextBox txtSourceMotDePasse = new TextBox();
        private TextBox txtSourcePort = new TextBox();


        // Champs du serveur cible.
        private TextBox txtCibleNomServeur = new TextBox();
        private TextBox txtCibleChaineConnexion = new TextBox();
        private TextBox txtCibleIdentifiant = new TextBox();
        private TextBox txtCibleMotDePasse = new TextBox();
        private TextBox txtCiblePort = new TextBox();
        private ComboBox cmbCibleTypeServeur = new ComboBox();

        // Listes déroulantes.
        private ComboBox cmbModeCopie = new ComboBox();
        private ComboBox cmbErreur = new ComboBox();
        private ComboBox cmbTentatives = new ComboBox();

        // Indique si l'utilisateur a modifié un champ sans enregistrer.
        private bool _aDesModificationsNonEnregistrees = false;

        // Évite de détecter des modifications pendant le chargement automatique.
        private bool _chargementEnCours = false;

        // Dernière configuration connue comme enregistrée.
        private ConfigurationModel _configurationEnregistree = new ConfigurationModel();

        public bool ADesModificationsNonEnregistrees => _aDesModificationsNonEnregistrees;

        // Événement envoyé au Controller quand l'utilisateur veut enregistrer.
        public event EventHandler? EnregistrerConfigurationDemande;

        public ConfigurationView()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.White;
            AutoScroll = false;

            CreerInterface();
        }

        private void CreerInterface()
        {
            panelContenu.Dock = DockStyle.Fill;
            panelContenu.BackColor = Color.White;
            panelContenu.AutoScroll = true;
            panelContenu.Padding = new Padding(25, 25, 25, 25);

            Controls.Add(panelContenu);

            layoutPrincipal.ColumnCount = 2;
            layoutPrincipal.RowCount = 0;
            layoutPrincipal.Dock = DockStyle.Top;
            layoutPrincipal.AutoSize = true;
            layoutPrincipal.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            layoutPrincipal.BackColor = Color.White;
            layoutPrincipal.Margin = new Padding(0);
            layoutPrincipal.Padding = new Padding(0);

            layoutPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            layoutPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            panelContenu.Controls.Add(layoutPrincipal);

            int ligne = 0;

            AjouterTitrePrincipal(ref ligne, "Paramètres de configuration");

            AjouterTitreSection(ref ligne, "Serveur source (production)");


            AjouterDeuxChamps(
                ref ligne,
                CreerChampTexte("Nom ou adresse IP du serveur", "Ex : PROD-SRV-01 ou 192.168.1.50", true, out txtSourceNomServeur),
                CreerChampTexte(
                    "Chaîne de connexion complète (optionnel)",
                    "Ex : Server=192.168.1.50,1433;Database=master;User Id=sa;Password=VotreMotDePasse;TrustServerCertificate=True;",
                    false,
                    out txtSourceChaineConnexion
                )
            );

            AjouterDeuxChamps(
                ref ligne,
                CreerChampTexte("Identifiant", "Ex : root ou sa", false, out txtSourceIdentifiant),
                CreerChampMotDePasse("Mot de passe", "12345678", false, out txtSourceMotDePasse)
            );

            cmbSourceTypeServeur = CreerComboBox("MySQL", "SQL Server");

            AjouterDeuxChamps(
                ref ligne,
                CreerChampTexte("Port", "Ex : 3306 ou 1433", false, out txtSourcePort),
                CreerChampComboBox("Type de serveur source", cmbSourceTypeServeur)
            );

            AjouterTitreSection(ref ligne, "Serveur cible (staging)");

            cmbCibleTypeServeur = CreerComboBox("MySQL", "SQL Server");

            AjouterDeuxChamps(
                ref ligne,
                CreerChampTexte("Nom ou adresse IP du serveur", "Ex : STAGING-SRV-01 ou 192.168.1.60", true, out txtCibleNomServeur),
                CreerChampTexte(
                    "Chaîne de connexion complète (optionnel)",
                    "Ex : Server=192.168.1.50,1433;Database=master;User Id=sa;Password=VotreMotDePasse;TrustServerCertificate=True;",
                    false,
                    out txtCibleChaineConnexion
                )
            );

            AjouterDeuxChamps(
                ref ligne,
                CreerChampTexte("Identifiant", "Ex : root ou sa", false, out txtCibleIdentifiant),
                CreerChampMotDePasse("Mot de passe", "12345678", false, out txtCibleMotDePasse)
            );

            AjouterDeuxChamps(
                ref ligne,
                CreerChampTexte("Port", "Ex : 3306 ou 1433", false, out txtCiblePort),
                CreerChampComboBox("Type de serveur cible", cmbCibleTypeServeur)
            );

            AjouterTitreSection(ref ligne, "Bases à copier");

            cmbModeCopie = CreerComboBox("Écraser", "Mise à jour");

            AjouterChampPleineLargeur(
                ref ligne,
                CreerChampComboBox("Mode de copie", cmbModeCopie)
            );

            AjouterTitreSection(ref ligne, "Comportement en cas d'erreur");

            cmbErreur = CreerComboBox(
                "Continuer avec les autres",
                "Arrêter tous les traitements"
            );

            cmbTentatives = CreerComboBox(
                "1 tentative",
                "2 tentatives",
                "3 tentatives"
            );

            AjouterDeuxChamps(
                ref ligne,
                CreerChampComboBox("Si une base échoue", cmbErreur),
                CreerChampComboBox("Tentatives de reprise", cmbTentatives)
            );

            cmbErreur.SelectedIndexChanged += (sender, e) =>
            {
                bool arreterTraitements =
                    cmbErreur.SelectedItem?.ToString() == "Arrêter tous les traitements";

                cmbTentatives.Enabled = !arreterTraitements;
                cmbTentatives.Cursor = arreterTraitements ? Cursors.No : Cursors.Default;
            };

            cmbSourceTypeServeur.SelectedIndexChanged += (sender, e) =>
            {
                MettreAJourPortParDefaut(cmbSourceTypeServeur, txtSourcePort);
            };

            cmbCibleTypeServeur.SelectedIndexChanged += (sender, e) =>
            {
                MettreAJourPortParDefaut(cmbCibleTypeServeur, txtCiblePort);
            };

            panelContenu.Resize += (sender, e) =>
            {
                AdapterLargeurContenu();
            };

            AdapterLargeurContenu();
            BrancherDetectionModifications();
        }

        private void AdapterLargeurContenu()
        {
            int largeur = panelContenu.ClientSize.Width
                - panelContenu.Padding.Left
                - panelContenu.Padding.Right
                - SystemInformation.VerticalScrollBarWidth;

            if (largeur < LargeurMinimumContenu)
            {
                largeur = LargeurMinimumContenu;
            }

            layoutPrincipal.Width = largeur;
        }
        /// <summary>
        /// Branche les événements qui permettent de savoir
        /// si l'utilisateur a modifié un champ.
        /// </summary>
        private void BrancherDetectionModifications()
        {
            txtSourceNomServeur.TextChanged += Champ_Modifie;
            txtSourceChaineConnexion.TextChanged += Champ_Modifie;
            txtSourceIdentifiant.TextChanged += Champ_Modifie;
            txtSourceMotDePasse.TextChanged += Champ_Modifie;
            txtSourcePort.TextChanged += Champ_Modifie;

            txtCibleNomServeur.TextChanged += Champ_Modifie;
            txtCibleChaineConnexion.TextChanged += Champ_Modifie;
            txtCibleIdentifiant.TextChanged += Champ_Modifie;
            txtCibleMotDePasse.TextChanged += Champ_Modifie;
            txtCiblePort.TextChanged += Champ_Modifie;

            cmbModeCopie.SelectedIndexChanged += Champ_Modifie;
            cmbErreur.SelectedIndexChanged += Champ_Modifie;
            cmbTentatives.SelectedIndexChanged += Champ_Modifie;

            cmbSourceTypeServeur.SelectedIndexChanged += Champ_Modifie;
            cmbCibleTypeServeur.SelectedIndexChanged += Champ_Modifie;
        }

        /// <summary>
        /// Appelé quand un champ est modifié par l'utilisateur.
        /// </summary>
        private void Champ_Modifie(object? sender, EventArgs e)
        {
            if (_chargementEnCours)
            {
                return;
            }

            _aDesModificationsNonEnregistrees = true;
        }

        /// <summary>
        /// Appelé par le Controller après un enregistrement réussi.
        /// </summary>
        public void MarquerCommeEnregistre()
        {
            _configurationEnregistree = RecupererConfiguration();
            _aDesModificationsNonEnregistrees = false;
        }

        /// <summary>
        /// Appelé si l'utilisateur choisit "Non".
        /// On remet les valeurs comme elles étaient lors du dernier enregistrement.
        /// </summary>
        public void AnnulerModificationsNonEnregistrees()
        {
            AfficherConfiguration(_configurationEnregistree);
            _aDesModificationsNonEnregistrees = false;
        }

        private void AjouterTitrePrincipal(ref int ligne, string texte)
        {
            Label label = new Label();
            label.Text = texte;
            label.Margin = new Padding(0, 0, 0, 25);

            PageFormStyle.AppliquerTitre(label);

            AjouterLigneAuto();
            layoutPrincipal.Controls.Add(label, 0, ligne);
            layoutPrincipal.SetColumnSpan(label, 2);

            ligne++;
        }

        private void AjouterTitreSection(ref int ligne, string texte)
        {
            Label label = new Label();
            label.Text = texte;
            label.Margin = new Padding(0, 8, 0, 16);

            PageFormStyle.AppliquerSousTitre(label);

            AjouterLigneAuto();
            layoutPrincipal.Controls.Add(label, 0, ligne);
            layoutPrincipal.SetColumnSpan(label, 2);

            ligne++;
        }

        private void AjouterDeuxChamps(ref int ligne, Panel champGauche, Panel champDroite)
        {
            AjouterLigneAuto();

            champGauche.Dock = DockStyle.Fill;
            champGauche.Margin = new Padding(0, 0, 12, 20);

            champDroite.Dock = DockStyle.Fill;
            champDroite.Margin = new Padding(12, 0, 0, 20);

            layoutPrincipal.Controls.Add(champGauche, 0, ligne);
            layoutPrincipal.Controls.Add(champDroite, 1, ligne);

            ligne++;
        }

        private void AjouterChampPleineLargeur(ref int ligne, Panel champ)
        {
            AjouterLigneAuto();

            champ.Dock = DockStyle.Fill;
            champ.Margin = new Padding(0, 0, 0, 20);

            layoutPrincipal.Controls.Add(champ, 0, ligne);
            layoutPrincipal.SetColumnSpan(champ, 2);

            ligne++;
        }

        private void AjouterLigneAuto()
        {
            layoutPrincipal.RowCount++;
            layoutPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        }

        private Panel CreerChampTexte(
            string texteLabel,
            string textePlaceholder,
            bool requis,
            out TextBox textBox)
        {
            Panel panel = CreerPanelChamp();

            AjouterLabelChamp(panel, texteLabel, requis);

            // Panel qui représente le cadre du champ.
            // Le cadre garde la même hauteur que les autres champs.
            Panel panelBordure = new Panel();
            panelBordure.Location = new Point(0, 28);
            panelBordure.Width = panel.Width;
            panelBordure.Height = HauteurChamp;
            panelBordure.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelBordure.BackColor = Color.White;
            panelBordure.BorderStyle = BorderStyle.FixedSingle;

            panel.Controls.Add(panelBordure);

            TextBox textBoxLocal = new TextBox();
            textBoxLocal.PlaceholderText = textePlaceholder;
            textBoxLocal.Text = "";
            textBoxLocal.BorderStyle = BorderStyle.None;
            textBoxLocal.BackColor = Color.White;
            textBoxLocal.ForeColor = Color.Black;
            textBoxLocal.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            if (texteLabel.Contains("Chaîne de connexion", StringComparison.OrdinalIgnoreCase))
            {
                // Texte plus petit seulement pour la chaîne de connexion.
                textBoxLocal.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            }
            else
            {
                // Texte normal pour les autres champs.
                textBoxLocal.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            }

            textBoxLocal.Height = textBoxLocal.PreferredHeight;
            textBoxLocal.Location = new Point(
                8,
                CalculerPositionVerticale(panelBordure, textBoxLocal)
            );

            textBoxLocal.Width = panelBordure.Width - 16;

            panelBordure.Controls.Add(textBoxLocal);

            panelBordure.Resize += (sender, e) =>
            {
                textBoxLocal.Width = panelBordure.Width - 16;
                textBoxLocal.Top = CalculerPositionVerticale(panelBordure, textBoxLocal);
            };

            textBox = textBoxLocal;

            return panel;
        }

        private Panel CreerChampMotDePasse(
            string texteLabel,
            string textePlaceholder,
            bool requis,
            out TextBox txtPassword)
        {
            Panel panel = CreerPanelChamp();

            AjouterLabelChamp(panel, texteLabel, requis);

            Panel panelPassword = new Panel();
            panelPassword.Location = new Point(0, 28);
            panelPassword.Width = panel.Width;
            panelPassword.Height = HauteurChamp;
            panelPassword.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelPassword.BackColor = Color.White;
            panelPassword.BorderStyle = BorderStyle.FixedSingle;

            panel.Controls.Add(panelPassword);

            TextBox txtPasswordLocal = new TextBox();

            txtPasswordLocal.PlaceholderText = textePlaceholder;
            txtPasswordLocal.Text = "";
            txtPasswordLocal.UseSystemPasswordChar = false;
            txtPasswordLocal.BorderStyle = BorderStyle.None;
            txtPasswordLocal.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            txtPasswordLocal.Height = txtPasswordLocal.PreferredHeight;
            txtPasswordLocal.Location = new Point(
                10,
                CalculerPositionVerticale(panelPassword, txtPasswordLocal)
            );
            txtPasswordLocal.Width = panelPassword.Width - 55;
            txtPasswordLocal.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            panelPassword.Controls.Add(txtPasswordLocal);

            IconButton btnVoirPassword = new IconButton();
            btnVoirPassword.IconChar = IconChar.Eye;
            btnVoirPassword.IconSize = 18;
            btnVoirPassword.IconColor = Color.FromArgb(80, 80, 80);
            btnVoirPassword.FlatStyle = FlatStyle.Flat;
            btnVoirPassword.FlatAppearance.BorderSize = 0;
            btnVoirPassword.BackColor = Color.White;
            btnVoirPassword.Width = 40;
            btnVoirPassword.Height = 34;
            btnVoirPassword.Location = new Point(panelPassword.Width - 43, 1);
            btnVoirPassword.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            bool motDePasseVisible = false;

            txtPasswordLocal.TextChanged += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtPasswordLocal.Text))
                {
                    txtPasswordLocal.UseSystemPasswordChar = false;
                }
                else
                {
                    txtPasswordLocal.UseSystemPasswordChar = !motDePasseVisible;
                }
            };

            btnVoirPassword.Click += (sender, e) =>
            {
                motDePasseVisible = !motDePasseVisible;

                if (!string.IsNullOrWhiteSpace(txtPasswordLocal.Text))
                {
                    txtPasswordLocal.UseSystemPasswordChar = !motDePasseVisible;
                }

                btnVoirPassword.IconChar = motDePasseVisible
                    ? IconChar.EyeSlash
                    : IconChar.Eye;
            };

            panelPassword.Controls.Add(btnVoirPassword);

            panelPassword.Resize += (sender, e) =>
            {
                txtPasswordLocal.Width = panelPassword.Width - 55;
                txtPasswordLocal.Top = CalculerPositionVerticale(panelPassword, txtPasswordLocal);
                btnVoirPassword.Left = panelPassword.Width - 43;
            };

            txtPassword = txtPasswordLocal;

            return panel;
        }

        private Panel CreerChampComboBox(string texteLabel, ComboBox comboBox)
        {
            Panel panel = CreerPanelChamp();

            AjouterLabelChamp(panel, texteLabel, false);

            Panel panelBordure = new Panel();
            panelBordure.Location = new Point(0, 28);
            panelBordure.Width = panel.Width;
            panelBordure.Height = HauteurChamp;
            panelBordure.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelBordure.BackColor = Color.White;
            panelBordure.BorderStyle = BorderStyle.FixedSingle;

            panel.Controls.Add(panelBordure);

            PageFormStyle.AppliquerComboBox(comboBox);

            comboBox.Location = new Point(1, CalculerPositionVerticale(panelBordure, comboBox));
            comboBox.Width = panelBordure.Width - 2;
            comboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            panelBordure.Controls.Add(comboBox);

            panelBordure.Resize += (sender, e) =>
            {
                comboBox.Width = panelBordure.Width - 2;
                comboBox.Top = CalculerPositionVerticale(panelBordure, comboBox);
            };

            comboBox.SelectionChangeCommitted += (sender, e) =>
            {
                panel.Focus();
            };

            return panel;
        }

        private int CalculerPositionVerticale(Control parent, Control enfant)
        {
            return (parent.Height - enfant.Height) / 2;
        }

        private Panel CreerPanelChamp()
        {
            Panel panel = new Panel();
            panel.Height = 75;
            panel.BackColor = Color.White;
            panel.Margin = new Padding(0);

            return panel;
        }

        private void AjouterLabelChamp(Panel parent, string texte, bool requis)
        {
            Label label = new Label();

            label.Text = texte;
            label.Location = new Point(0, 0);

            PageFormStyle.AppliquerLabelChamp(label);
            parent.Controls.Add(label);

            if (requis)
            {
                Label badge = PageFormStyle.CreerBadgeRequis();

                badge.Location = new Point(label.Right + 10, 0);

                parent.Controls.Add(badge);
            }
        }

        private ComboBox CreerComboBox(params string[] valeurs)
        {
            ComboBox comboBox = new ComboBox();

            foreach (string valeur in valeurs)
            {
                comboBox.Items.Add(valeur);
            }

            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }

            return comboBox;
        }

        // Affiche une configuration sauvegardée dans l'interface.
        public void AfficherConfiguration(ConfigurationModel configuration)
        {
            _chargementEnCours = true;

            SelectionnerComboBox(
                cmbSourceTypeServeur,
                NormaliserTypeServeur(configuration.ServeurSource.TypeServeur),
                "SQL Server"
            );


            txtSourceNomServeur.Text = configuration.ServeurSource.NomServeur;
            txtSourceChaineConnexion.Text = configuration.ServeurSource.ChaineConnexion;
            txtSourceIdentifiant.Text = configuration.ServeurSource.Identifiant;
            txtSourceMotDePasse.Text = configuration.ServeurSource.MotDePasse;
            txtSourcePort.Text = configuration.ServeurSource.Port > 0
                ? configuration.ServeurSource.Port.ToString()
                : "";

            SelectionnerComboBox(
                cmbCibleTypeServeur,
                NormaliserTypeServeur(configuration.ServeurCible.TypeServeur),
                "SQL Server"
            );


            txtCibleNomServeur.Text = configuration.ServeurCible.NomServeur;
            txtCibleChaineConnexion.Text = configuration.ServeurCible.ChaineConnexion;
            txtCibleIdentifiant.Text = configuration.ServeurCible.Identifiant;
            txtCibleMotDePasse.Text = configuration.ServeurCible.MotDePasse;
            txtCiblePort.Text = configuration.ServeurCible.Port > 0
                ? configuration.ServeurCible.Port.ToString()
                : "";

            SelectionnerComboBox(
                cmbModeCopie,
                NormaliserModeCopie(configuration.ModeCopie),
                "Écraser"
            );

            SelectionnerComboBox(
                cmbErreur,
                configuration.ComportementErreur,
                "Continuer avec les autres"
            );

            SelectionnerComboBox(
                cmbTentatives,
                ConvertirTentativesEnTexte(configuration.TentativesReprise),
                "1 tentative"
            );

            bool arreterTraitements =
                cmbErreur.SelectedItem?.ToString() == "Arrêter tous les traitements";

            cmbTentatives.Enabled = !arreterTraitements;
            cmbTentatives.Cursor = arreterTraitements ? Cursors.No : Cursors.Default;

            _configurationEnregistree = RecupererConfiguration();
            _aDesModificationsNonEnregistrees = false;
            _chargementEnCours = false;
        }

        // Cette méthode transforme les champs de l'interface en Model.
        // La View récupère seulement les valeurs, puis le Controller les donne au Service.
        public ConfigurationModel RecupererConfiguration()
        {
            ConfigurationModel configuration = new ConfigurationModel();

            configuration.ServeurSource = new ServeurConfigModel
            {

                TypeServeur = cmbSourceTypeServeur.SelectedItem?.ToString() ?? "SQL Server",
                NomServeur = txtSourceNomServeur.Text.Trim(),
                ChaineConnexion = txtSourceChaineConnexion.Text.Trim(),
                Identifiant = txtSourceIdentifiant.Text.Trim(),
                MotDePasse = txtSourceMotDePasse.Text.Trim(),
                Port = ConvertirPort(txtSourcePort.Text)
            };

            configuration.ServeurCible = new ServeurConfigModel
            {

                TypeServeur = cmbCibleTypeServeur.SelectedItem?.ToString() ?? "SQL Server",
                NomServeur = txtCibleNomServeur.Text.Trim(),
                ChaineConnexion = txtCibleChaineConnexion.Text.Trim(),
                Identifiant = txtCibleIdentifiant.Text.Trim(),
                MotDePasse = txtCibleMotDePasse.Text.Trim(),
                Port = ConvertirPort(txtCiblePort.Text)
            };

            configuration.ModeCopie =
                NormaliserModeCopie(cmbModeCopie.SelectedItem?.ToString() ?? string.Empty);

            configuration.ComportementErreur =
                cmbErreur.SelectedItem?.ToString() ?? string.Empty;

            configuration.TentativesReprise =
                ConvertirTentatives(cmbTentatives.SelectedItem?.ToString());

            return configuration;
        }

        // MainForm appelle cette méthode quand l'utilisateur clique
        // sur le bouton "Enregistrer les paramètres".
        public void DemanderEnregistrement()
        {
            EnregistrerConfigurationDemande?.Invoke(this, EventArgs.Empty);
        }

        public void AfficherMessageSucces(string message)
        {
            MessageBox.Show(
                message,
                "Succès",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        public void AfficherMessageErreur(string message)
        {
            MessageBox.Show(
                message,
                "Erreur",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }

        private void SelectionnerComboBox(ComboBox comboBox, string valeur, string valeurDefaut)
        {
            if (!string.IsNullOrWhiteSpace(valeur) && comboBox.Items.Contains(valeur))
            {
                comboBox.SelectedItem = valeur;
                return;
            }

            if (comboBox.Items.Contains(valeurDefaut))
            {
                comboBox.SelectedItem = valeurDefaut;
                return;
            }

            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }
        }

        private int ConvertirPort(string valeur)
        {
            if (int.TryParse(valeur, out int port))
            {
                return port;
            }

            return 0;
        }

        private int ConvertirTentatives(string? valeur)
        {
            return valeur switch
            {
                "1 tentative" => 1,
                "2 tentatives" => 2,
                "3 tentatives" => 3,
                _ => 0
            };
        }

        private string ConvertirTentativesEnTexte(int tentatives)
        {
            return tentatives switch
            {
                1 => "1 tentative",
                2 => "2 tentatives",
                3 => "3 tentatives",
                _ => "1 tentative"
            };
        }

        private string NormaliserModeCopie(string modeCopie)
        {
            return modeCopie switch
            {
                "Mettre à jour" => "Mise à jour",
                "Mise à jour" => "Mise à jour",
                "Écraser" => "Écraser",
                _ => "Écraser"
            };
        }

        private string NormaliserTypeServeur(string typeServeur)
        {
            return typeServeur switch
            {
                "SQL Server" => "SQL Server",
                "MySQL" => "MySQL",
                _ => "SQL Server"
            };
        }

        private void MettreAJourPortParDefaut(ComboBox comboBoxTypeServeur, TextBox txtPort)
        {
            if (_chargementEnCours)
            {
                return;
            }

            string typeServeur = comboBoxTypeServeur.SelectedItem?.ToString() ?? "SQL Server";

            txtPort.Text = typeServeur switch
            {
                "MySQL" => "3306",
                "SQL Server" => "1433",
                _ => "1433"
            };
        }
    }
}