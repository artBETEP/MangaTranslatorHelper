namespace MangaTranslatorHelper
{
    partial class Form1
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
            components = new System.ComponentModel.Container();
            button1 = new Button();
            button2 = new Button();
            pictureBox1 = new PictureBox();
            listView1 = new ListView();
            textBox1 = new TextBox();
            button3 = new Button();
            imageList1 = new ImageList(components);
            contextMenuStrip1 = new ContextMenuStrip(components);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(29, 22);
            button1.Name = "button1";
            button1.Size = new Size(185, 48);
            button1.TabIndex = 0;
            button1.Text = "Open Image";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(221, 22);
            button2.Name = "button2";
            button2.Size = new Size(211, 48);
            button2.TabIndex = 1;
            button2.Text = "Open Mark File";
            button2.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(457, 35);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(800, 800);
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            pictureBox1.Paint += pictureBox1_Paint;
            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.MouseUp += pictureBox1_MouseUp;
            // 
            // listView1
            // 
            listView1.Location = new Point(37, 594);
            listView1.Name = "listView1";
            listView1.Size = new Size(388, 590);
            listView1.TabIndex = 3;
            listView1.UseCompatibleStateImageBehavior = false;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(37, 360);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(388, 182);
            textBox1.TabIndex = 4;
            // 
            // button3
            // 
            button3.Location = new Point(221, 77);
            button3.Name = "button3";
            button3.Size = new Size(211, 46);
            button3.TabIndex = 5;
            button3.Text = "Save Mark FIle";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth8Bit;
            imageList1.ImageSize = new Size(16, 16);
            imageList1.TransparentColor = Color.Transparent;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(20, 20);
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(61, 4);
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(1984, 1278);
            Controls.Add(button3);
            Controls.Add(textBox1);
            Controls.Add(listView1);
            Controls.Add(pictureBox1);
            Controls.Add(button2);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Manga Translator Helper";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Button button2;
        private PictureBox pictureBox1;
        private ListView listView1;
        private TextBox textBox1;
        private Button button3;
        private ImageList imageList1;
        private ContextMenuStrip contextMenuStrip1;
    }
}