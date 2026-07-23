namespace DVLD_PresentationLayer
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
            this.Element1 = new System.Windows.Forms.Integration.ElementHost();
            this.uC_PersonCard1 = new DVLD_PresentationLayer.UC_PersonCard();
            this.SuspendLayout();
            // 
            // Element1
            // 
            this.Element1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Element1.Location = new System.Drawing.Point(0, 0);
            this.Element1.Name = "Element1";
            this.Element1.Size = new System.Drawing.Size(991, 569);
            this.Element1.TabIndex = 0;
            this.Element1.Text = "elementHost1";
            this.Element1.ChildChanged += new System.EventHandler<System.Windows.Forms.Integration.ChildChangedEventArgs>(this.Element1_ChildChanged);
            this.Element1.Child = this.uC_PersonCard1;
            // 
            // Frm_ShowPerson
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(991, 569);
            this.Controls.Add(this.Element1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_ShowPerson";
            this.Text = "Show Person";
            this.Load += new System.EventHandler(this.Frm_ShowPerson_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost Element1;
        private UC_PersonCard uC_PersonCard1;
    }
}