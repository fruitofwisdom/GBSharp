namespace GBSharp
{
	public partial class MainForm : Form
	{
		GameBoy _gameBoy = new();
		Thread? GameBoyThread;

		// Available debug callbacks.
		private delegate void PauseCallback();
		private static PauseCallback? PauseCallbackInternal;
		private delegate void PrintDebugMessageCallback(string debugMessage);
		private static PrintDebugMessageCallback? PrintDebugMessageCallbackInternal;
		private delegate void PrintDebugStatusCallback(string debugStatus);
		private static PrintDebugStatusCallback? PrintDebugStatisCallbackInternal;

		public MainForm()
		{
			InitializeComponent();

			PauseCallbackInternal = PauseInternal;
			PrintDebugMessageCallbackInternal = PrintDebugMessageInternal;
			PrintDebugStatisCallbackInternal = PrintDebugStatusInternal;
		}

		private void MainFormClosing(object sender, FormClosingEventArgs e)
		{
			if (GameBoyThread != null)
			{
				_gameBoy.Stop();
				GameBoyThread.Join();
			}
		}

		private void LoadROMToolStripMenuItemClick(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new()
			{
				Filter = "Game Boy ROMs (*.gb)|*.gb|All files (*.*)|*.*",
				RestoreDirectory = true
			};

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				if (ROM.Instance.Load(openFileDialog.FileName))
				{
					// Close any previous threads and reset the Game Boy.
					if (GameBoyThread != null)
					{
						_gameBoy.Stop();
						GameBoyThread.Join();
					}
					_gameBoy.Reset();

					// Put the game name and ROM type in our title.
					Text = "GB# - " + ROM.Instance.Title + " (" + ROM.Instance.CartridgeType.ToString().Replace("_", "+") + ")";

					// Start a new thread to run the Game Boy.
					GameBoyThread = new Thread(new ThreadStart(_gameBoy.Run));
					GameBoyThread.Start();
					playButton.Enabled = true;
					pauseButton.Enabled = false;
					stepButton.Enabled = true;
				}
			}
		}

		private void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			Close();
		}

		private void PrintOpcodesToolStripMenuClick(object sender, EventArgs e)
		{
			printOpcodesToolStripMenuItem.Checked = !printOpcodesToolStripMenuItem.Checked;
			CPU.ShouldPrintOpcodes = printOpcodesToolStripMenuItem.Checked;
		}

		private void AboutGBSharpToolStripMenuItemClick(object sender, EventArgs e)
		{
			AboutBox aboutBox = new();
			aboutBox.ShowDialog();
		}

		private void PlayButtonClick(object sender, EventArgs e)
		{
			_gameBoy.Play();

			playButton.Enabled = false;
			pauseButton.Enabled = true;
			stepButton.Enabled = false;
			resetButton.Enabled = false;
		}

		private void PauseButtonClick(object sender, EventArgs e)
		{
			_gameBoy.Pause();

			playButton.Enabled = true;
			pauseButton.Enabled = false;
			stepButton.Enabled = true;
			resetButton.Enabled = true;
		}

		private void StepButtonClick(object sender, EventArgs e)
		{
			_gameBoy.Step();

			playButton.Enabled = true;
			pauseButton.Enabled = false;
			stepButton.Enabled = true;
			resetButton.Enabled = true;
		}

		private void ResetButtonClick(object sender, EventArgs e)
		{
			_gameBoy.Reset();

			playButton.Enabled = true;
			pauseButton.Enabled = false;
			stepButton.Enabled = true;
			resetButton.Enabled = false;
		}

		public static void Pause()
		{
			PauseCallbackInternal?.Invoke();
		}

		private void PauseInternal()
		{
			// We must invoke on the UI thread.
			Invoke(new Action(() => PauseButtonClick(this, EventArgs.Empty)));
		}

		public static void PrintDebugMessage(string debugMessage)
		{
			PrintDebugMessageCallbackInternal?.Invoke(debugMessage);
		}

		private void PrintDebugMessageInternal(string debugMessage)
		{
			// We must invoke on the UI thread.
			Invoke(new Action(() => debugRichTextBox.AppendText(debugMessage)));
		}

		public static void PrintDebugStatus(string debugStatus)
		{
			PrintDebugStatisCallbackInternal?.Invoke(debugStatus);
		}

		private void PrintDebugStatusInternal(string debugStatus)
		{
			// We must invoke on the UI thread.
			Invoke(new Action(() => debugToolStripStatusLabel.Text = debugStatus));
		}
	}
}
