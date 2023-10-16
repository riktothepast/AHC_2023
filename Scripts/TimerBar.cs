using Godot;

public partial class TimerBar : HSlider
{
	public override void _Ready()
	{
		base._Ready();
	}

	public override void _Process(double delta)
	{
	}

	public void SetTimer(double value)
	{
		MaxValue = value;
		Value = value;
	}

	public void UpdateTimer(double value)
	{
		SetValueNoSignal(value);
	}
}
