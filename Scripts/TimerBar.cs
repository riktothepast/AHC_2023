using Godot;

public partial class TimerBar : Node
{
	[Export]
	private HSlider _slider;
	private CustomSignals _customSignals;

	public override void _Ready()
	{
		base._Ready();
        _customSignals = GetNode<CustomSignals>("/root/Signals");
		_customSignals.Connect("SetTimerValue", new Callable(this, nameof(SetTimer)));
	}

	public override void _Process(double delta)
	{
		// _slider.Value = delta;
	}

	private void SetTimer(float value)
	{
		_slider.MaxValue = value;
		_slider.Value = value;
	}
}
