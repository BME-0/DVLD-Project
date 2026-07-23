namespace DVLD_PresentationLayer
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
            this.AddEdit_Element = new System.Windows.Forms.Integration.ElementHost();
            this.uC_PersonCardAddEdit1 = new DVLD_PresentationLayer.UC_PersonCardAddEdit();
            this.SuspendLayout();
            // 
            // AddEdit_Element
            // 
            this.AddEdit_Element.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AddEdit_Element.Location = new System.Drawing.Point(0, 0);
            this.AddEdit_Element.Name = "AddEdit_Element";
            this.AddEdit_Element.Size = new System.Drawing.Size(891, 515);
            this.AddEdit_Element.TabIndex = 0;
            this.AddEdit_Element.Text = "AddEdit_Element";
            this.AddEdit_Element.ChildChanged += new System.EventHandler<System.Windows.Forms.Integration.ChildChangedEventArgs>(this.AddEdit_Element_ChildChanged);
            this.AddEdit_Element.Child = this.uC_PersonCardAddEdit1;
            // 
            // Frm_AddEditPerson
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(891, 515);
            this.Controls.Add(this.AddEdit_Element);
            this.Name = "Frm_AddEditPerson";
            this.Text = "Add New Person";
            this.Load += new System.EventHandler(this.Frm_AddEditPerson_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost AddEdit_Element;
        private UC_PersonCardAddEdit uC_PersonCardAddEdit1;
    }
}