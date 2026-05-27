using System.Drawing;
using System.Windows.Forms;

namespace SaimDataCopy.Styles
{
    /// <summary>
    /// Helper pour centraliser le design de la page Paramètres Email.
    /// </summary>
    public static class EmailFormStyle
    {
        public static readonly Color CouleurBleu = Color.FromArgb(30, 96, 190);
        public static readonly Color CouleurFondPage = Color.White;
        public static readonly Color CouleurFondInfo = Color.FromArgb(232, 240, 254);
        public static readonly Color CouleurTexte = Color.FromArgb(20, 20, 20);
        public static readonly Color CouleurTexteSecondaire = Color.FromArgb(90, 90, 90);
        public static readonly Color CouleurBordure = Color.FromArgb(160, 160, 160);
        public static readonly Color CouleurBadgeRequis = Color.FromArgb(255, 235, 238);
        public static readonly Color CouleurTexteBadge = Color.FromArgb(180, 50, 50);

        public static void AppliquerPage(UserControl page)
        {
            page.Dock = DockStyle.Fill;
            page.BackColor = CouleurFondPage;
            page.AutoScroll = true;
        }

        public static void AppliquerTitre(Label label)
        {
            label.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            label.ForeColor = CouleurTexte;
            label.AutoSize = true;
        }

        public static void AppliquerTitreSection(Label label)
        {
            label.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            label.ForeColor = CouleurTexte;
            label.AutoSize = true;
        }

        public static void AppliquerLabel(Label label)
        {
            label.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            label.ForeColor = CouleurTexte;
            label.AutoSize = true;
        }

        public static void AppliquerBadgeRequis(Label label)
        {
            label.Text = "requis";
            label.Font = new Font("Segoe UI", 8, FontStyle.Regular);
            label.ForeColor = CouleurTexteBadge;
            label.BackColor = CouleurBadgeRequis;
            label.AutoSize = true;
            label.Padding = new Padding(6, 2, 6, 2);
        }

        public static void AppliquerTextBoxDansBordure(TextBox textBox)
        {
            textBox.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            textBox.BorderStyle = BorderStyle.None;
            textBox.BackColor = Color.White;
            textBox.ForeColor = CouleurTexte;
            textBox.AutoSize = false;
            textBox.Height = 22;
            textBox.TextAlign = HorizontalAlignment.Left;
        }

        public static void AppliquerTextBoxMultiligne(TextBox textBox)
        {
            textBox.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.BackColor = Color.White;
            textBox.ForeColor = CouleurTexte;
            textBox.Multiline = true;
            textBox.ScrollBars = ScrollBars.Vertical;
        }

        public static void AppliquerPanelBordure(Panel panel)
        {
            panel.BackColor = Color.White;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Height = 40;
        }

        public static void AppliquerComboBoxDansBordure(ComboBox comboBox)
        {
            comboBox.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.FlatStyle = FlatStyle.Flat;
            comboBox.BackColor = Color.White;
            comboBox.ForeColor = CouleurTexte;
        }

        public static void AppliquerCheckBox(CheckBox checkBox)
        {
            checkBox.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            checkBox.ForeColor = CouleurTexte;
            checkBox.BackColor = CouleurFondPage;
            checkBox.AutoSize = true;
        }

        public static void AppliquerBoutonTest(Button bouton)
        {
            bouton.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            bouton.BackColor = Color.White;
            bouton.ForeColor = CouleurTexte;
            bouton.FlatStyle = FlatStyle.Flat;
            bouton.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
            bouton.FlatAppearance.BorderSize = 1;
            bouton.Cursor = Cursors.Hand;
            bouton.Height = 36;
        }

        public static void AppliquerBarreInfo(Panel panel)
        {
            panel.BackColor = CouleurFondInfo;
            panel.Height = 48;
        }

        public static void AppliquerTexteInfo(Label label)
        {
            label.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            label.ForeColor = CouleurTexte;
            label.AutoSize = true;
            label.BackColor = CouleurFondInfo;
        }

        public static void AppliquerAlerte(Panel panel)
        {
            panel.BackColor = Color.FromArgb(255, 245, 225);
            panel.Height = 48;
        }
    }
}