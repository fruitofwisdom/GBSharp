﻿namespace GBSharp
{
	partial class MainForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			menuStrip = new MenuStrip();
			fileToolStripMenuItem = new ToolStripMenuItem();
			loadROMToolStripMenuItem = new ToolStripMenuItem();
			exitToolStripMenuItem = new ToolStripMenuItem();
			helpToolStripMenuItem = new ToolStripMenuItem();
			aboutGBSharpToolStripMenuItem = new ToolStripMenuItem();
			toolStrip = new ToolStrip();
			playButton = new ToolStripButton();
			menuStrip.SuspendLayout();
			toolStrip.SuspendLayout();
			SuspendLayout();
			// 
			// menuStrip
			// 
			menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, helpToolStripMenuItem });
			menuStrip.Location = new Point(0, 0);
			menuStrip.Name = "menuStrip";
			menuStrip.Size = new Size(800, 24);
			menuStrip.TabIndex = 0;
			menuStrip.Text = "menuStrip";
			// 
			// fileToolStripMenuItem
			// 
			fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loadROMToolStripMenuItem, exitToolStripMenuItem });
			fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			fileToolStripMenuItem.Size = new Size(37, 20);
			fileToolStripMenuItem.Text = "File";
			// 
			// loadROMToolStripMenuItem
			// 
			loadROMToolStripMenuItem.Name = "loadROMToolStripMenuItem";
			loadROMToolStripMenuItem.Size = new Size(139, 22);
			loadROMToolStripMenuItem.Text = "Load ROM...";
			loadROMToolStripMenuItem.Click += loadROMToolStripMenuItem_Click;
			// 
			// exitToolStripMenuItem
			// 
			exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			exitToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
			exitToolStripMenuItem.Size = new Size(139, 22);
			exitToolStripMenuItem.Text = "Exit";
			exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
			// 
			// helpToolStripMenuItem
			// 
			helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutGBSharpToolStripMenuItem });
			helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			helpToolStripMenuItem.Size = new Size(44, 20);
			helpToolStripMenuItem.Text = "Help";
			// 
			// aboutGBSharpToolStripMenuItem
			// 
			aboutGBSharpToolStripMenuItem.Name = "aboutGBSharpToolStripMenuItem";
			aboutGBSharpToolStripMenuItem.Size = new Size(155, 22);
			aboutGBSharpToolStripMenuItem.Text = "About GBSharp";
			aboutGBSharpToolStripMenuItem.Click += aboutGBSharpToolStripMenuItem_Click;
			// 
			// toolStrip
			// 
			toolStrip.Items.AddRange(new ToolStripItem[] { playButton });
			toolStrip.Location = new Point(0, 24);
			toolStrip.Name = "toolStrip";
			toolStrip.Size = new Size(800, 25);
			toolStrip.TabIndex = 1;
			toolStrip.Text = "toolStrip";
			// 
			// playButton
			// 
			playButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
			playButton.Enabled = false;
			playButton.Font = new Font("Segoe MDL2 Assets", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
			playButton.ImageTransparentColor = Color.Magenta;
			playButton.Name = "playButton";
			playButton.Size = new Size(23, 22);
			playButton.Text = "";
			playButton.Click += playButton_Click;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(800, 450);
			Controls.Add(toolStrip);
			Controls.Add(menuStrip);
			Icon = (Icon)resources.GetObject("$this.Icon");
			MainMenuStrip = menuStrip;
			Name = "MainForm";
			Text = "GB#";
			menuStrip.ResumeLayout(false);
			menuStrip.PerformLayout();
			toolStrip.ResumeLayout(false);
			toolStrip.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private MenuStrip menuStrip;
		private ToolStripMenuItem fileToolStripMenuItem;
		private ToolStripMenuItem loadROMToolStripMenuItem;
		private ToolStripMenuItem exitToolStripMenuItem;
		private ToolStripMenuItem helpToolStripMenuItem;
		private ToolStripMenuItem aboutGBSharpToolStripMenuItem;
		private ToolStrip toolStrip;
		private ToolStripButton playButton;
	}
}
