namespace Pen.Service.App
{
    partial class Form1
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
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.button = new System.Windows.Forms.ToolStripButton();
            this.button1 = new System.Windows.Forms.Button();
            this.dataSetLessonFiles = new System.Data.DataSet();
            this.LessonFile = new System.Data.DataTable();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetLessonFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LessonFile)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.BackColor = System.Drawing.Color.White;
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.button});
            this.toolStrip.Location = new System.Drawing.Point(26, 9);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip.Size = new System.Drawing.Size(73, 37);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            // 
            // button
            // 
            this.button.BackColor = System.Drawing.Color.White;
            this.button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(94)))), ((int)(((byte)(162)))));
            this.button.Image = global::Pen.Service.App.Properties.Resources.pause;
            this.button.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.button.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button.Name = "button";
            this.button.Size = new System.Drawing.Size(70, 34);
            this.button.Text = "CLick";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(167, 42);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dataSetLessonFiles
            // 
            this.dataSetLessonFiles.DataSetName = "NewDataSet";
            this.dataSetLessonFiles.Tables.AddRange(new System.Data.DataTable[] {
            this.LessonFile});
            // 
            // LessonFile
            // 
            this.LessonFile.TableName = "LessonFile";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 146);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "label1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 236);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.toolStrip);
            this.Name = "Form1";
            this.Text = "Form1";
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetLessonFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LessonFile)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton button;
        private System.Windows.Forms.Button button1;
        private System.Data.DataSet dataSetLessonFiles;
        private System.Data.DataTable LessonFile;
        private System.Windows.Forms.Label label1;
    }
}