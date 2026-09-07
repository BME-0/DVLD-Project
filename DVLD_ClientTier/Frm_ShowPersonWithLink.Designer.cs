namespace DVLD_ClientTier
{
    partial class Frm_ShowPersonWithLink
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_ShowPersonWithLink));
            this.Element = new System.Windows.Forms.Integration.ElementHost();
            this.uC_PersonCardWithLink1 = new DVLD_ClientTier.UC_PersonCardWithLink();
            this.SuspendLayout();
            // 
            // Element
            // 
            this.Element.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Element.Location = new System.Drawing.Point(0, 0);
            this.Element.Name = "Element";
            this.Element.Size = new System.Drawing.Size(800, 450);
            this.Element.TabIndex = 0;
            this.Element.Text = "Element";
            this.Element.ChildChanged += new System.EventHandler<System.Windows.Forms.Integration.ChildChangedEventArgs>(this.Element_ChildChanged);
            this.Element.Child = this.uC_PersonCardWithLink1;
            // 
            // Frm_ShowPersonWithLink
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Element);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_ShowPersonWithLink";
            this.Text = "Show Person Information";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost Element;
        private UC_PersonCardWithLink uC_PersonCardWithLink1;
    }
}