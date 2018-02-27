namespace Go
{
    partial class Startup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Startup));
            this.FiveBoard = new System.Windows.Forms.RadioButton();
            this.NineBoard = new System.Windows.Forms.RadioButton();
            this.NineteenBoard = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.PlayButton = new System.Windows.Forms.Button();
            this.ExitButton = new System.Windows.Forms.Button();
            this.thinkingTimeUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.thinkingTimeUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // FiveBoard
            // 
            this.FiveBoard.AutoSize = true;
            this.FiveBoard.Location = new System.Drawing.Point(12, 25);
            this.FiveBoard.Name = "FiveBoard";
            this.FiveBoard.Size = new System.Drawing.Size(42, 17);
            this.FiveBoard.TabIndex = 0;
            this.FiveBoard.TabStop = true;
            this.FiveBoard.Text = "5x5";
            this.FiveBoard.UseVisualStyleBackColor = true;
            // 
            // NineBoard
            // 
            this.NineBoard.AutoSize = true;
            this.NineBoard.Location = new System.Drawing.Point(12, 48);
            this.NineBoard.Name = "NineBoard";
            this.NineBoard.Size = new System.Drawing.Size(42, 17);
            this.NineBoard.TabIndex = 1;
            this.NineBoard.TabStop = true;
            this.NineBoard.Text = "9x9";
            this.NineBoard.UseVisualStyleBackColor = true;
            // 
            // NineteenBoard
            // 
            this.NineteenBoard.AutoSize = true;
            this.NineteenBoard.Location = new System.Drawing.Point(12, 71);
            this.NineteenBoard.Name = "NineteenBoard";
            this.NineteenBoard.Size = new System.Drawing.Size(54, 17);
            this.NineteenBoard.TabIndex = 2;
            this.NineteenBoard.TabStop = true;
            this.NineteenBoard.Text = "19x19";
            this.NineteenBoard.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "GridSize:";
            // 
            // PlayButton
            // 
            this.PlayButton.Location = new System.Drawing.Point(72, 9);
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.Size = new System.Drawing.Size(96, 39);
            this.PlayButton.TabIndex = 4;
            this.PlayButton.Text = "Play";
            this.PlayButton.UseVisualStyleBackColor = true;
            this.PlayButton.Click += new System.EventHandler(this.PlayButtonClick);
            // 
            // ExitButton
            // 
            this.ExitButton.Location = new System.Drawing.Point(72, 54);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(96, 39);
            this.ExitButton.TabIndex = 5;
            this.ExitButton.Text = "Exit";
            this.ExitButton.UseVisualStyleBackColor = true;
            this.ExitButton.Click += new System.EventHandler(this.ExitClicked);
            // 
            // thinkingTimeUpDown
            // 
            this.thinkingTimeUpDown.Location = new System.Drawing.Point(12, 135);
            this.thinkingTimeUpDown.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.thinkingTimeUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.thinkingTimeUpDown.Name = "thinkingTimeUpDown";
            this.thinkingTimeUpDown.Size = new System.Drawing.Size(41, 20);
            this.thinkingTimeUpDown.TabIndex = 6;
            this.thinkingTimeUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 119);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Computer thinking time:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(59, 137);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "(seconds)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 158);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(150, 26);
            this.label4.TabIndex = 9;
            this.label4.Text = "*Actual computer thinking time\r\nmay vary on large boards";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(12, 99);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(106, 17);
            this.checkBox1.TabIndex = 10;
            this.checkBox1.Text = "Player vs player?";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // Startup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(180, 193);
            this.ControlBox = false;
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.thinkingTimeUpDown);
            this.Controls.Add(this.ExitButton);
            this.Controls.Add(this.PlayButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NineteenBoard);
            this.Controls.Add(this.NineBoard);
            this.Controls.Add(this.FiveBoard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Startup";
            this.Text = "Startup";
            ((System.ComponentModel.ISupportInitialize)(this.thinkingTimeUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton FiveBoard;
        private System.Windows.Forms.RadioButton NineBoard;
        private System.Windows.Forms.RadioButton NineteenBoard;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button PlayButton;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.NumericUpDown thinkingTimeUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}