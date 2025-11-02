
namespace Eng2Myan
{
    partial class Eng2Myan
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Eng2Myan));
            usrInput = new TextBox();
            transliterateOutput = new TextBox();
            unicodeOut = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            pictureBox1 = new PictureBox();
            label4 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // usrInput
            // 
            usrInput.Location = new Point(148, 12);
            usrInput.Multiline = true;
            usrInput.Name = "usrInput";
            usrInput.Size = new Size(680, 38);
            usrInput.TabIndex = 0;
            usrInput.TextChanged += usrInput_TextChanged;
            // 
            // transliterateOutput
            // 
            transliterateOutput.Location = new Point(12, 104);
            transliterateOutput.Multiline = true;
            transliterateOutput.Name = "transliterateOutput";
            transliterateOutput.Size = new Size(816, 145);
            transliterateOutput.TabIndex = 1;
            // 
            // unicodeOut
            // 
            unicodeOut.Font = new Font("Segoe UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            unicodeOut.Location = new Point(12, 285);
            unicodeOut.Multiline = true;
            unicodeOut.Name = "unicodeOut";
            unicodeOut.Size = new Size(816, 145);
            unicodeOut.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(12, 76);
            label1.Name = "label1";
            label1.Size = new Size(108, 25);
            label1.TabIndex = 3;
            label1.Text = "Zawgyi-One";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(12, 257);
            label2.Name = "label2";
            label2.Size = new Size(77, 25);
            label2.TabIndex = 4;
            label2.Text = "Unicode";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(2, 20);
            label3.Name = "label3";
            label3.Size = new Size(140, 25);
            label3.TabIndex = 5;
            label3.Text = "Myanglish Input";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.InitialImage = null;
            pictureBox1.Location = new Point(12, 450);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(59, 49);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(77, 463);
            label4.Name = "label4";
            label4.Size = new Size(438, 20);
            label4.TabIndex = 7;
            label4.Text = "UnikTek FZ LLC | 2025 | +971504762739 | au.naingoo@gmail.com";
            // 
            // Eng2Myan
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(840, 527);
            Controls.Add(label4);
            Controls.Add(pictureBox1);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(unicodeOut);
            Controls.Add(transliterateOutput);
            Controls.Add(usrInput);
            MaximizeBox = false;
            Name = "Eng2Myan";
            Text = "Eng2Myan";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private void transliterateOutput_TextChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        private TextBox usrInput;
        private TextBox transliterateOutput;
        private TextBox unicodeOut;
        private Label label1;
        private Label label2;
        private Label label3;
        private PictureBox pictureBox1;
        private Label label4;
    }
}
