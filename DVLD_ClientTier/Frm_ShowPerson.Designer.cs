namespace DVLD_ClientTier
{
    partial class Frm_ShowPerson
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_ShowPerson));
            this.ShowPersonElement = new System.Windows.Forms.Integration.ElementHost();
            this.uC_PersonCard1 = new DVLD_ClientTier.UC_PersonCard();
            this.SuspendLayout();
            // 
            // ShowPersonElement
            // 
            this.ShowPersonElement.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ShowPersonElement.Location = new System.Drawing.Point(0, 0);
            this.ShowPersonElement.Name = "ShowPersonElement";
            this.ShowPersonElement.Size = new System.Drawing.Size(800, 450);
            this.ShowPersonElement.TabIndex = 0;
            this.ShowPersonElement.Text = "elementHost1";
            //this.ShowPersonElement.ChildChanged += new System.EventHandler<System.Windows.Forms.Integration.ChildChangedEventArgs>(this.ShowPersonElement_ChildChanged);
            this.ShowPersonElement.Child = this.uC_PersonCard1;
            // 
            // Frm_ShowPerson
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ShowPersonElement);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_ShowPerson";
            this.Text = "Show Person Information";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost ShowPersonElement;
        private UC_PersonCard uC_PersonCard1;
    }
}