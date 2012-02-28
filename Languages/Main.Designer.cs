namespace Languages
{
	partial class Main
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
			this.GB_Main = new System.Windows.Forms.GroupBox();
			this.Lbl_Lang = new System.Windows.Forms.Label();
			this.Btn_Generate = new System.Windows.Forms.Button();
			this.Cb_Languages = new System.Windows.Forms.ComboBox();
			this.GB_Main.SuspendLayout();
			this.SuspendLayout();
			// 
			// GB_Main
			// 
			this.GB_Main.BackColor = System.Drawing.Color.Transparent;
			this.GB_Main.Controls.Add(this.Lbl_Lang);
			this.GB_Main.Controls.Add(this.Btn_Generate);
			this.GB_Main.Controls.Add(this.Cb_Languages);
			this.GB_Main.Location = new System.Drawing.Point(28, 27);
			this.GB_Main.Name = "GB_Main";
			this.GB_Main.Size = new System.Drawing.Size(450, 133);
			this.GB_Main.TabIndex = 0;
			this.GB_Main.TabStop = false;
			this.GB_Main.Paint += new System.Windows.Forms.PaintEventHandler(this.GB_Main_Paint);
			// 
			// Lbl_Lang
			// 
			this.Lbl_Lang.AutoSize = true;
			this.Lbl_Lang.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Lbl_Lang.Location = new System.Drawing.Point(24, 31);
			this.Lbl_Lang.Name = "Lbl_Lang";
			this.Lbl_Lang.Size = new System.Drawing.Size(175, 18);
			this.Lbl_Lang.TabIndex = 2;
			this.Lbl_Lang.Text = "Please select a language:";
			// 
			// Btn_Generate
			// 
			this.Btn_Generate.Location = new System.Drawing.Point(345, 79);
			this.Btn_Generate.Name = "Btn_Generate";
			this.Btn_Generate.Size = new System.Drawing.Size(78, 23);
			this.Btn_Generate.TabIndex = 1;
			this.Btn_Generate.Text = "Generate";
			this.Btn_Generate.UseVisualStyleBackColor = true;
			this.Btn_Generate.Click += new System.EventHandler(this.Btn_Generate_Click);
			// 
			// Cb_Languages
			// 
			this.Cb_Languages.FormattingEnabled = true;
			this.Cb_Languages.Location = new System.Drawing.Point(42, 52);
			this.Cb_Languages.Name = "Cb_Languages";
			this.Cb_Languages.Size = new System.Drawing.Size(378, 21);
			this.Cb_Languages.TabIndex = 0;
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 362);
			this.Controls.Add(this.GB_Main);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Main";
			this.Text = "TDSM Language Generator";
			this.Load += new System.EventHandler(this.Main_Load);
			this.GB_Main.ResumeLayout(false);
			this.GB_Main.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox GB_Main;
		private System.Windows.Forms.Label Lbl_Lang;
		private System.Windows.Forms.Button Btn_Generate;
		private System.Windows.Forms.ComboBox Cb_Languages;
	}
}

