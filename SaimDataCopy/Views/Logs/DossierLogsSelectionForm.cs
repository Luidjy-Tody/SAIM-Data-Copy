using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SaimDataCopy.Views.Logs
{
    /// <summary>
    /// Fenêtre personnalisée pour choisir le dossier des logs.
    /// On utilise un TreeView pour éviter FolderBrowserDialog,
    /// car il bloque sur certains postes.
    /// </summary>
    public class DossierLogsSelectionForm : Form
    {
        private readonly TreeView _treeDossiers;
        private readonly TextBox _txtCheminSelectionne;
        private readonly Button _btnValider;
        private readonly Button _btnAnnuler;

        public string DossierSelectionne { get; private set; } = string.Empty;

        public DossierLogsSelectionForm(string cheminInitial)
        {
            Text = "Choisir le dossier des logs";
            StartPosition = FormStartPosition.CenterParent;

            // ClientSize définit la vraie taille intérieure disponible.
            // Cela évite que les boutons soient coupés par la barre de titre Windows.
            ClientSize = new Size(720, 540);

            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;

            _treeDossiers = new TreeView();
            _txtCheminSelectionne = new TextBox();
            _btnValider = new Button();
            _btnAnnuler = new Button();

            CreerInterface();
            ChargerLecteurs();

            if (!string.IsNullOrWhiteSpace(cheminInitial) &&
                Directory.Exists(cheminInitial))
            {
                _txtCheminSelectionne.Text = cheminInitial;
            }
        }

        private void CreerInterface()
        {
            Label lblTitre = new Label
            {
                Text = "Sélectionnez le dossier où enregistrer les logs",
                Location = new Point(20, 15),
                AutoSize = true,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold)
            };

            _treeDossiers.Location = new Point(20, 50);
            _treeDossiers.Size = new Size(660, 300);
            _treeDossiers.Font = new Font("Segoe UI", 10F);
            _treeDossiers.BeforeExpand += TreeDossiers_BeforeExpand;
            _treeDossiers.AfterSelect += TreeDossiers_AfterSelect;

            Label lblChemin = new Label
            {
                Text = "Dossier sélectionné",
                Location = new Point(20, 365),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F)
            };

            _txtCheminSelectionne.Location = new Point(20, 390);
            _txtCheminSelectionne.Size = new Size(660, 30);
            _txtCheminSelectionne.Font = new Font("Segoe UI", 10F);
            _txtCheminSelectionne.ReadOnly = true;

            _btnValider.Text = "Valider";
            _btnValider.Location = new Point(490, 450);
            _btnValider.Size = new Size(90, 32);
            _btnValider.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _btnValider.Click += BtnValider_Click;

            _btnAnnuler.Text = "Annuler";
            _btnAnnuler.Location = new Point(590, 450);
            _btnAnnuler.Size = new Size(90, 32);
            _btnAnnuler.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _btnAnnuler.Click += BtnAnnuler_Click;

            Controls.Add(lblTitre);
            Controls.Add(_treeDossiers);
            Controls.Add(lblChemin);
            Controls.Add(_txtCheminSelectionne);
            Controls.Add(_btnValider);
            Controls.Add(_btnAnnuler);
        }

        private void ChargerLecteurs()
        {
            _treeDossiers.Nodes.Clear();

            foreach (string lecteur in Directory.GetLogicalDrives())
            {
                TreeNode noeudLecteur = new TreeNode(lecteur)
                {
                    Tag = lecteur
                };

                // Faux enfant pour afficher le bouton d'ouverture.
                noeudLecteur.Nodes.Add("Chargement...");

                _treeDossiers.Nodes.Add(noeudLecteur);
            }
        }

        private void TreeDossiers_BeforeExpand(object? sender, TreeViewCancelEventArgs e)
        {
            TreeNode? noeud = e.Node;

            if (noeud == null)
            {
                return;
            }

            if (noeud.Nodes.Count == 1 &&
                noeud.Nodes[0].Text == "Chargement...")
            {
                noeud.Nodes.Clear();

                string? cheminNoeud = noeud.Tag as string;

                if (string.IsNullOrWhiteSpace(cheminNoeud))
                {
                    return;
                }

                ChargerSousDossiers(noeud, cheminNoeud);
            }
        }

        private void ChargerSousDossiers(TreeNode noeudParent, string cheminParent)
        {
            try
            {
                string[] sousDossiers = Directory.GetDirectories(cheminParent);

                foreach (string sousDossier in sousDossiers)
                {
                    DirectoryInfo infoDossier = new DirectoryInfo(sousDossier);

                    TreeNode noeudSousDossier = new TreeNode(infoDossier.Name)
                    {
                        Tag = sousDossier
                    };

                    // Faux enfant pour permettre l'ouverture plus tard.
                    noeudSousDossier.Nodes.Add("Chargement...");

                    noeudParent.Nodes.Add(noeudSousDossier);
                }
            }
            catch
            {
                // Certains dossiers Windows sont protégés.
                // On les ignore pour éviter de bloquer l'application.
            }
        }

        private void TreeDossiers_AfterSelect(object? sender, TreeViewEventArgs e)
        {
            TreeNode? noeud = e.Node;

            if (noeud == null)
            {
                return;
            }

            string? cheminSelectionne = noeud.Tag as string;

            if (!string.IsNullOrWhiteSpace(cheminSelectionne))
            {
                _txtCheminSelectionne.Text = cheminSelectionne;
            }
        }

        private void BtnValider_Click(object? sender, EventArgs e)
        {
            string cheminChoisi = _txtCheminSelectionne.Text.Trim();

            if (string.IsNullOrWhiteSpace(cheminChoisi))
            {
                MessageBox.Show(
                    "Veuillez sélectionner un dossier.",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return;
            }

            if (!Directory.Exists(cheminChoisi))
            {
                MessageBox.Show(
                    "Le dossier sélectionné n'existe pas.",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return;
            }

            DossierSelectionne = cheminChoisi;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnAnnuler_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}