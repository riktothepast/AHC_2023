using Godot;
using System;

public partial class ScoreUI : RichTextLabel
{
	private CustomSignals _customSignals;
	private int _currentScore;

	public override void _Ready()
	{
		_customSignals = (CustomSignals)GetNode("/root/Signals");
		_customSignals.Connect("IncreaseScore", new Callable(this, nameof(UpdateScore)));
	}

	public override void _Process(double delta)
	{
	}

	private void UpdateScore(int value)
	{
		_currentScore += value;
		Text = _currentScore.ToString();
	}
}
