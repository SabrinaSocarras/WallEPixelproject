namespace WallEPixelproject
{
    partial class PixelWallE
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>s
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PixelWallE));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnRedimensionar = new System.Windows.Forms.Button();
            this.logText = new System.Windows.Forms.TextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.pbCanvas = new System.Windows.Forms.PictureBox();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCargar = new System.Windows.Forms.Button();
            this.btnEjecutar = new System.Windows.Forms.Button();
            this.lineNumberPanel = new System.Windows.Forms.Panel();
            this.textCanvasSize = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCanvas)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.textCanvasSize);
            this.panel1.Controls.Add(this.lineNumberPanel);
            this.panel1.Controls.Add(this.btnRedimensionar);
            this.panel1.Controls.Add(this.logText);
            this.panel1.Controls.Add(this.richTextBox1);
            this.panel1.Controls.Add(this.pbCanvas);
            this.panel1.Controls.Add(this.btnLimpiar);
            this.panel1.Controls.Add(this.btnGuardar);
            this.panel1.Controls.Add(this.btnCargar);
            this.panel1.Controls.Add(this.btnEjecutar);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1292, 706);
            this.panel1.TabIndex = 0;
            // 
            // btnRedimensionar
            // 
            this.btnRedimensionar.BackColor = System.Drawing.Color.IndianRed;
            this.btnRedimensionar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRedimensionar.Font = new System.Drawing.Font("Showcard Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRedimensionar.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnRedimensionar.Location = new System.Drawing.Point(626, 27);
            this.btnRedimensionar.Name = "btnRedimensionar";
            this.btnRedimensionar.Size = new System.Drawing.Size(194, 44);
            this.btnRedimensionar.TabIndex = 7;
            this.btnRedimensionar.Text = "Redimensionar";
            this.btnRedimensionar.UseVisualStyleBackColor = false;
            this.btnRedimensionar.Click += new System.EventHandler(this.btnRedimensionar_Click);
            // 
            // logText
            // 
            this.logText.Location = new System.Drawing.Point(1066, 508);
            this.logText.Multiline = true;
            this.logText.Name = "logText";
            this.logText.Size = new System.Drawing.Size(173, 114);
            this.logText.TabIndex = 6;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Font = new System.Drawing.Font("Consolas", 10F);
            this.richTextBox1.Location = new System.Drawing.Point(879, 97);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(369, 321);
            this.richTextBox1.TabIndex = 5;
            this.richTextBox1.Text = "";
            this.richTextBox1.VScroll += new System.EventHandler(this.richTextBox1_VScroll);
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // pbCanvas
            // 
            this.pbCanvas.BackColor = System.Drawing.Color.White;
            this.pbCanvas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbCanvas.Location = new System.Drawing.Point(82, 122);
            this.pbCanvas.Name = "pbCanvas";
            this.pbCanvas.Size = new System.Drawing.Size(500, 500);
            this.pbCanvas.TabIndex = 2;
            this.pbCanvas.TabStop = false;
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.BackColor = System.Drawing.Color.IndianRed;
            this.btnLimpiar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLimpiar.Font = new System.Drawing.Font("Showcard Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLimpiar.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnLimpiar.Location = new System.Drawing.Point(82, 27);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(103, 44);
            this.btnLimpiar.TabIndex = 3;
            this.btnLimpiar.Text = "Limpiar ";
            this.btnLimpiar.UseVisualStyleBackColor = false;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.BackColor = System.Drawing.Color.IndianRed;
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.Font = new System.Drawing.Font("Showcard Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardar.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnGuardar.Location = new System.Drawing.Point(208, 27);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(118, 44);
            this.btnGuardar.TabIndex = 2;
            this.btnGuardar.Text = "Guardar ";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnCargar
            // 
            this.btnCargar.BackColor = System.Drawing.Color.IndianRed;
            this.btnCargar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCargar.Font = new System.Drawing.Font("Showcard Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCargar.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCargar.Location = new System.Drawing.Point(344, 27);
            this.btnCargar.Name = "btnCargar";
            this.btnCargar.Size = new System.Drawing.Size(106, 44);
            this.btnCargar.TabIndex = 1;
            this.btnCargar.Text = "Cargar ";
            this.btnCargar.UseVisualStyleBackColor = false;
            this.btnCargar.Click += new System.EventHandler(this.btnCargar_Click);
            // 
            // btnEjecutar
            // 
            this.btnEjecutar.BackColor = System.Drawing.Color.IndianRed;
            this.btnEjecutar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEjecutar.Font = new System.Drawing.Font("Showcard Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEjecutar.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnEjecutar.Location = new System.Drawing.Point(466, 27);
            this.btnEjecutar.Name = "btnEjecutar";
            this.btnEjecutar.Size = new System.Drawing.Size(128, 44);
            this.btnEjecutar.TabIndex = 0;
            this.btnEjecutar.Text = " Ejecutar ";
            this.btnEjecutar.UseVisualStyleBackColor = false;
            this.btnEjecutar.Click += new System.EventHandler(this.btnEjecutar_Click);
            // 
            // lineNumberPanel
            // 
            this.lineNumberPanel.BackColor = System.Drawing.Color.LightGray;
            this.lineNumberPanel.Location = new System.Drawing.Point(834, 97);
            this.lineNumberPanel.Name = "lineNumberPanel";
            this.lineNumberPanel.Size = new System.Drawing.Size(39, 321);
            this.lineNumberPanel.TabIndex = 10;
            this.lineNumberPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.lineNumberPanel_Paint);
            // 
            // textCanvasSize
            // 
            this.textCanvasSize.Location = new System.Drawing.Point(1155, 24);
            this.textCanvasSize.Name = "textCanvasSize";
            this.textCanvasSize.Size = new System.Drawing.Size(50, 34);
            this.textCanvasSize.TabIndex = 11;
            this.textCanvasSize.Text = "20";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1.Location = new System.Drawing.Point(1003, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 28);
            this.label1.TabIndex = 12;
            this.label1.Text = "Tamaño Canvas";
            // 
            // PixelWallE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.ClientSize = new System.Drawing.Size(1292, 706);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "PixelWallE";
            this.Text = "Pixel Wall-E ";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCanvas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnEjecutar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCargar;
        private System.Windows.Forms.PictureBox pbCanvas;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TextBox logText;
        private System.Windows.Forms.Button btnRedimensionar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textCanvasSize;
    }
}