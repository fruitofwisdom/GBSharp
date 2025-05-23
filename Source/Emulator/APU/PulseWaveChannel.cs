﻿using NAudio.Wave.SampleProviders;

namespace GBSharp
{
	// A pulse wave generator for sound channels 1 and 2.
	internal class PulseWaveProvider : SampleProvider
	{
		public PulseWaveProvider()
		{
			BuildWaveform(0);
		}

		public void BuildWaveform(uint waveformDuty)
		{
			for (int i = 0; i < kSampleRate; i++)
			{
				if (waveformDuty == 0)
				{
					_waveTable[i] = i > kSampleRate / 8 ? 1.0f : 0.0f;
				}
				else if (waveformDuty == 1)
				{
					_waveTable[i] = i > kSampleRate / 4 ? 1.0f : 0.0f;
				}
				else if (waveformDuty == 2)
				{
					_waveTable[i] = i > kSampleRate / 2 ? 1.0f : 0.0f;
				}
				else if (waveformDuty == 3)
				{
					_waveTable[i] = i > kSampleRate * 0.75f ? 1.0f : 0.0f;
				}
			}
		}
	}

	// TODO: Other generators for sound channels 3 and 4.

	// Sound channels 1 and 2 are a rectangular, "pulse" wave. Note that channel 2 doesn't support sweep.
	internal class PulseWaveChannel : Channel
	{
		private readonly PulseWaveProvider _pulseWaveProvider = new();

		// The sweep settings. (NR10, 0xFF10)
		private readonly bool _sweepEnabled = false;
		private uint _currentSweepStep = 0;
		public uint SweepTime = 0;
		public bool SweepIncDec = false;
		public int SweepShiftNumber = 0;
		private float _sweepFrequencyShift = 0.0f;

		// The waveform duty. (NR11 and NR21, 0xFF11 and 0xFF21)
		private uint _lastWaveformDuty = 0;
		public uint WaveformDuty = 0;

		// The envelope settings. (NR12 and NR22, 0xFF12 and 0xFF22)
		private uint _currentEnvelopeValue = 0;
		public uint DefaultEnvelopeValue { get; private set; } = 0;
		public bool EnvelopeUpDown = false;
		private uint _currentEnvelopeStep = 0;
		public uint LengthOfEnvelopeSteps { get; private set; } = 0;

		// The low-order frequency period. (NR13 and NR23, 0xFF13 and 0xFF23)
		public uint LowOrderFrequencyData = 0;

		// The high-order frequency period. (NR14 and NR24, 0xFF14 and 0xFF24)
		public uint HighOrderFrequencyData = 0;

		public PulseWaveChannel(bool sweepEnabled = false) : base(64)
		{
			_sweepEnabled = sweepEnabled;
			_waveOut.Init(new SampleToWaveProvider(_pulseWaveProvider));
		}

		public override void UpdateDiv(ushort divApu)
		{
			// Update length timer.
			base.UpdateDiv(divApu);

			if (!SoundOn)
			{
				return;
			}

			// DIV-APU runs at 512Hz, envelope sweep at 64Hz
			if (divApu % 8 == 0)
			{
				// Apply the envelope sweep, if it's enabled.
				if (LengthOfEnvelopeSteps != 0)
				{
					_currentEnvelopeStep++;
					if (_currentEnvelopeStep >= LengthOfEnvelopeSteps)
					{
						if (EnvelopeUpDown && _currentEnvelopeValue < 15)
						{
							_currentEnvelopeValue++;
						}
						else if (!EnvelopeUpDown && _currentEnvelopeValue > 0)
						{
							_currentEnvelopeValue--;
						}
						_currentEnvelopeStep = 0;
					}
				}
			}

			// DIV-APU runs at 512Hz, frequency sweep at 128Hz.
			if (_sweepEnabled && divApu % 4 == 0)
			{
				// Apply the frequency sweep, if it's enabled.
				if (SweepTime != 0 && SweepShiftNumber != 0)
				{
					_currentSweepStep++;
					if (_currentSweepStep >= SweepTime)
					{
						// Calculate the shift to apply in Update.
						if (SweepIncDec)
						{
							_sweepFrequencyShift -= (_pulseWaveProvider._frequency - _sweepFrequencyShift) / (2 << SweepShiftNumber);
						}
						else
						{
							_sweepFrequencyShift += (_pulseWaveProvider._frequency + _sweepFrequencyShift) / (2 << SweepShiftNumber);
						}
						// Keep the frequency in bounds.
						if (_pulseWaveProvider._frequency + _sweepFrequencyShift > 131072f)
						{
							_sweepFrequencyShift = 0.0f;
							SoundOn = false;
						}
						else if (_pulseWaveProvider._frequency - _sweepFrequencyShift < 0f)
						{
							_sweepFrequencyShift = 0.0f;
						}
						_currentSweepStep = 0;
					}
				}
				else
				{
					_sweepFrequencyShift = 0.0f;
				}
			}
		}

		static uint lastFrequencyData = 0;

		public override void Update()
		{
			// Are we muted?
			if (APU.Instance.Mute ||
				(_sweepEnabled && APU.Instance.MuteChannels[0]) ||
				(!_sweepEnabled && APU.Instance.MuteChannels[1]) ||
				!APU.Instance.IsOn() || !SoundOn)
			{
				_pulseWaveProvider._leftVolume = 0.0f;
				_pulseWaveProvider._rightVolume = 0.0f;
			}
			else
			{
				// Set volume levels.
				// NOTE: Channel 1 is the pulse wave channel with sweep enabled.
				if (_sweepEnabled)
				{
					_pulseWaveProvider._leftVolume = APU.Instance.Channel1LeftOn ? _currentEnvelopeValue / 15.0f : 0.0f;
					_pulseWaveProvider._rightVolume = APU.Instance.Channel1RightOn ? _currentEnvelopeValue / 15.0f : 0.0f;
				}
				// NOTE: Channel 2 is the other.
				else
				{
					_pulseWaveProvider._leftVolume = APU.Instance.Channel2LeftOn ? _currentEnvelopeValue / 15.0f : 0.0f;
					_pulseWaveProvider._rightVolume = APU.Instance.Channel2RightOn ? _currentEnvelopeValue / 15.0f : 0.0f;
				}

				// If the shape of the waveform has changed, rebuild it.
				if (WaveformDuty != _lastWaveformDuty)
				{
					_pulseWaveProvider.BuildWaveform(WaveformDuty);
					_lastWaveformDuty = WaveformDuty;
				}

				// Update the frequency.
				uint frequencyData = LowOrderFrequencyData + (HighOrderFrequencyData << 8);
				if (_sweepEnabled && frequencyData != lastFrequencyData)
				{
					//GameBoy.DebugOutput += "Channel 0 changing frequency from " + lastFrequencyData + " to " + frequencyData + "\n";
					lastFrequencyData = frequencyData;
				}
				if (frequencyData < 2047)
				{
					float periodValue = 2048 - frequencyData;
					float newFrequency = (131072 / periodValue) + _sweepFrequencyShift;
					newFrequency = Math.Max(0.0f, newFrequency);
					_pulseWaveProvider._frequency = newFrequency;
					float justFrequency = (131072 / periodValue);
					//GameBoy.DebugOutput += $"Channel 0 frequency is {frequencyData}, so {justFrequency:F3} + {_sweepFrequencyShift:F3} = {newFrequency:F3},\n";
				}
			}

			// Fill the audio buffer with the latest wave table data.
			_pulseWaveProvider.FillAudioBuffer();
		}

		public void SetDefaultEnvelopeValue(uint defaultEnvelopeValue)
		{
			_currentEnvelopeValue = defaultEnvelopeValue;
			DefaultEnvelopeValue = defaultEnvelopeValue;
		}

		public void SetLengthOfEnvelopeSteps(uint lengthOfEnvelopeSteps)
		{
			_currentEnvelopeStep = 0;
			LengthOfEnvelopeSteps = lengthOfEnvelopeSteps;
		}
	}
}
