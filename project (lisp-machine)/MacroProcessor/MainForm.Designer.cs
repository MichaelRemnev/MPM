namespace MacroProcessor
{
    partial class MainForm
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
            this.btn_first_run = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_source_code = new System.Windows.Forms.RichTextBox();
            this.tb_result = new System.Windows.Forms.RichTextBox();
            this.tb_output_file = new System.Windows.Forms.TextBox();
            this.btn_save = new System.Windows.Forms.Button();
            this.tb_input_file = new System.Windows.Forms.TextBox();
            this.btn_load = new System.Windows.Forms.Button();
            this.gpb_result = new System.Windows.Forms.GroupBox();
            this.gpb_source_code = new System.Windows.Forms.GroupBox();
            this.tb_error = new System.Windows.Forms.TextBox();
            this.gpb_result.SuspendLayout();
            this.gpb_source_code.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_first_run
            // 
            this.btn_first_run.Location = new System.Drawing.Point(601, 169);
            this.btn_first_run.Margin = new System.Windows.Forms.Padding(4);
            this.btn_first_run.Name = "btn_first_run";
            this.btn_first_run.Size = new System.Drawing.Size(157, 58);
            this.btn_first_run.TabIndex = 12;
            this.btn_first_run.Text = "Выполнить";
            this.btn_first_run.UseVisualStyleBackColor = true;
            this.btn_first_run.Click += new System.EventHandler(this.btn_first_run_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(93, 32);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "Исходный код";
            // 
            // tb_source_code
            // 
            this.tb_source_code.Location = new System.Drawing.Point(8, 60);
            this.tb_source_code.Margin = new System.Windows.Forms.Padding(4);
            this.tb_source_code.Name = "tb_source_code";
            this.tb_source_code.Size = new System.Drawing.Size(421, 598);
            this.tb_source_code.TabIndex = 4;
            this.tb_source_code.Text = "";
            // 
            // tb_result
            // 
            this.tb_result.Enabled = false;
            this.tb_result.Location = new System.Drawing.Point(11, 60);
            this.tb_result.Margin = new System.Windows.Forms.Padding(4);
            this.tb_result.Name = "tb_result";
            this.tb_result.ReadOnly = true;
            this.tb_result.Size = new System.Drawing.Size(415, 508);
            this.tb_result.TabIndex = 5;
            this.tb_result.Text = "";
            // 
            // tb_output_file
            // 
            this.tb_output_file.Enabled = false;
            this.tb_output_file.Location = new System.Drawing.Point(45, 320);
            this.tb_output_file.Margin = new System.Windows.Forms.Padding(4);
            this.tb_output_file.Name = "tb_output_file";
            this.tb_output_file.ReadOnly = true;
            this.tb_output_file.Size = new System.Drawing.Size(377, 22);
            this.tb_output_file.TabIndex = 2;
            this.tb_output_file.Visible = false;
            // 
            // btn_save
            // 
            this.btn_save.Location = new System.Drawing.Point(249, 26);
            this.btn_save.Margin = new System.Windows.Forms.Padding(4);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(105, 27);
            this.btn_save.TabIndex = 0;
            this.btn_save.Text = "Сохранить";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // tb_input_file
            // 
            this.tb_input_file.Enabled = false;
            this.tb_input_file.Location = new System.Drawing.Point(27, 154);
            this.tb_input_file.Margin = new System.Windows.Forms.Padding(4);
            this.tb_input_file.Name = "tb_input_file";
            this.tb_input_file.ReadOnly = true;
            this.tb_input_file.Size = new System.Drawing.Size(384, 22);
            this.tb_input_file.TabIndex = 3;
            // 
            // btn_load
            // 
            this.btn_load.Location = new System.Drawing.Point(207, 26);
            this.btn_load.Margin = new System.Windows.Forms.Padding(4);
            this.btn_load.Name = "btn_load";
            this.btn_load.Size = new System.Drawing.Size(97, 27);
            this.btn_load.TabIndex = 1;
            this.btn_load.Text = "Загрузить";
            this.btn_load.UseVisualStyleBackColor = true;
            this.btn_load.Click += new System.EventHandler(this.btn_load_Click);
            // 
            // gpb_result
            // 
            this.gpb_result.Controls.Add(this.tb_result);
            this.gpb_result.Controls.Add(this.tb_output_file);
            this.gpb_result.Controls.Add(this.btn_save);
            this.gpb_result.Location = new System.Drawing.Point(913, 97);
            this.gpb_result.Margin = new System.Windows.Forms.Padding(4);
            this.gpb_result.Name = "gpb_result";
            this.gpb_result.Padding = new System.Windows.Forms.Padding(4);
            this.gpb_result.Size = new System.Drawing.Size(432, 576);
            this.gpb_result.TabIndex = 11;
            this.gpb_result.TabStop = false;
            this.gpb_result.Text = "Результат";
            // 
            // gpb_source_code
            // 
            this.gpb_source_code.Controls.Add(this.label1);
            this.gpb_source_code.Controls.Add(this.tb_source_code);
            this.gpb_source_code.Controls.Add(this.tb_input_file);
            this.gpb_source_code.Controls.Add(this.btn_load);
            this.gpb_source_code.Location = new System.Drawing.Point(16, 15);
            this.gpb_source_code.Margin = new System.Windows.Forms.Padding(4);
            this.gpb_source_code.Name = "gpb_source_code";
            this.gpb_source_code.Padding = new System.Windows.Forms.Padding(4);
            this.gpb_source_code.Size = new System.Drawing.Size(439, 667);
            this.gpb_source_code.TabIndex = 9;
            this.gpb_source_code.TabStop = false;
            this.gpb_source_code.Text = "Исходный код";
            // 
            // tb_error
            // 
            this.tb_error.Enabled = false;
            this.tb_error.Location = new System.Drawing.Point(465, 15);
            this.tb_error.Margin = new System.Windows.Forms.Padding(4);
            this.tb_error.Multiline = true;
            this.tb_error.Name = "tb_error";
            this.tb_error.ReadOnly = true;
            this.tb_error.Size = new System.Drawing.Size(879, 70);
            this.tb_error.TabIndex = 16;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1359, 711);
            this.Controls.Add(this.tb_error);
            this.Controls.Add(this.btn_first_run);
            this.Controls.Add(this.gpb_result);
            this.Controls.Add(this.gpb_source_code);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximumSize = new System.Drawing.Size(1377, 758);
            this.MinimumSize = new System.Drawing.Size(1377, 758);
            this.Name = "MainForm";
            this.Text = "Lisp-машина";
            this.Load += new System.EventHandler(this.fm_main_Load);
            this.gpb_result.ResumeLayout(false);
            this.gpb_result.PerformLayout();
            this.gpb_source_code.ResumeLayout(false);
            this.gpb_source_code.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btn_first_run;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox tb_source_code;
        private System.Windows.Forms.RichTextBox tb_result;
        private System.Windows.Forms.TextBox tb_output_file;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.TextBox tb_input_file;
        private System.Windows.Forms.Button btn_load;
        private System.Windows.Forms.GroupBox gpb_result;
        private System.Windows.Forms.GroupBox gpb_source_code;
        private System.Windows.Forms.TextBox tb_error;
    }
}