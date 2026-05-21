namespace SaimDataCopy.Forms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panelBottom = new Panel();
            panelMenu = new Panel();
            panelMain = new Panel();
            SuspendLayout();
            // 
            // panelBottom
            // 
            panelBottom.BackColor = SystemColors.ButtonFace;
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Location = new Point(0, 774);
            panelBottom.Name = "panelBottom";
            panelBottom.Size = new Size(1282, 35);
            panelBottom.TabIndex = 0;
            // 
            // panelMenu
            // 
            panelMenu.BackColor = Color.WhiteSmoke;
            panelMenu.Dock = DockStyle.Left;
            panelMenu.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            panelMenu.Location = new Point(0, 0);
            panelMenu.Name = "panelMenu";
            panelMenu.Size = new Size(230, 774);
            panelMenu.TabIndex = 2;
            // 
            // panelMain
            // 
            panelMain.Dock = DockStyle.Fill;
            panelMain.Location = new Point(230, 0);
            panelMain.Name = "panelMain";
            panelMain.Size = new Size(1052, 774);
            panelMain.TabIndex = 3;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1282, 809);
            Controls.Add(panelMain);
            Controls.Add(panelMenu);
            Controls.Add(panelBottom);
            MaximumSize = new Size(1300, 856);
            MinimumSize = new Size(1300, 856);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Copie automatique des données entre serveurs — SAIM LTD";
            ResumeLayout(false);
        }

        #endregion

        private Panel panelBottom;
        private Panel panelMenu;
        private Panel panelMain;
    }
}
