using Godot;
using System;
using System.Threading.Tasks;

public partial class RoundMessage : Node2D
{
    [Export]
    public double waitTime = 2f;
    private CustomSignals _customSignals;

    public override void _Ready()
	{
		_customSignals = (CustomSignals)GetNode("/root/Signals");
        _customSignals.Connect("RoundComplete", new Callable(this, nameof(RoundComplete)));
    }

    private void RoundComplete()
    {
        GD.Print("round complete!");
        Tween tween = GetTree().CreateTween();
		tween.Chain().TweenProperty(this, "scale", Vector2.One, 0.15f).SetTrans(Tween.TransitionType.Bounce);
        RoundResume();
    }

    private async void RoundResume()
    {
        TimeSpan span = TimeSpan.FromSeconds(waitTime);
		await Task.Delay(span);
        Tween tween = GetTree().CreateTween();
        tween.Chain().TweenProperty(this, "scale", Vector2.Right, 0.15f).SetTrans(Tween.TransitionType.Bounce);
        _customSignals.EmitSignal("RoundResume");
    }
}
