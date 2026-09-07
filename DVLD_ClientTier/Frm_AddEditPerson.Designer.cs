namespace DVLD_ClientTier
{
    partial class Frm_AddEditPerson
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_AddEditPerson));
            this.AddPersonElement = new System.Windows.Forms.Integration.ElementHost();
            this.uC_PersonCardAddEdit1 = new DVLD_ClientTier.UC_PersonCardAddEdit();
            this.SuspendLayout();
            // 
            // AddPersonElement
            // 
            this.AddPersonElement.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AddPersonElement.Location = new System.Drawing.Point(0, 0);
            this.AddPersonElement.Name = "AddPersonElement";
            this.AddPersonElement.Size = new System.Drawing.Size(800, 450);
            this.AddPersonElement.TabIndex = 0;
            this.AddPersonElement.Text = "PersonCard";
            this.AddPersonElement.Child = this.uC_PersonCardAddEdit1;
            // 
            // Frm_AddEditPerson
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.AddPersonElement);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_AddEditPerson";
            this.Text = "Add New Person";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost AddPersonElement;
        private UC_PersonCardAddEdit uC_PersonCardAddEdit1;
    }
}