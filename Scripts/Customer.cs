using System;
using Godot;

public partial class Customer : Node2D
{
	[Export]
	private HSlider _slider;
	[Export]
	private Sprite2D _requestedItem;
	private INGREDIENT_TYPE _ingredientType;
	private bool _process;
	private PackedScene _reaction;
	private double _time;
	private CustomSignals _customSignals;

	public override void _Ready()
	{
		_reaction = GD.Load<PackedScene>("res://Scenes/Reaction.tscn");
		_customSignals = (CustomSignals)GetNode("/root/Signals");
	}

	public override void _Process(double delta)
	{
		if(_process)
		{
			_time -= delta;
			_slider.SetValueNoSignal(_time);
			if (_time <= 0)
			{
				_process = false;
				_customSignals.EmitSignal("CustomerLostPatience");
			}
		}
	}

	public void Initialize(double time = 3f)
	{
		SetTime(time);
		Tween tween = GetTree().CreateTween();
		tween.TweenCallback(new Callable(this, nameof(Enable)));
		_requestedItem.Scale = Vector2.One;
	}

	private void Enable()
	{
		_process = true;
	}

	public void SetIngredient(INGREDIENT_TYPE type, Texture2D texture)
	{
		_ingredientType = type;
		_requestedItem.Texture = texture;
		_requestedItem.Scale = Vector2.Zero;
	}

	private void SetTime(double time)
	{
		_time = time;
		_slider.MaxValue = time;
		_slider.Value = time;
	}

	public INGREDIENT_TYPE GetIngredient()
	{
		return _ingredientType;
	}

	public void Move(Vector2 target)
	{
		MoveTween(target);
	}

	public async void GetOut(bool gaveIngredient, Action action)
	{
		_requestedItem.Scale = Vector2.Zero;
		_process = false;
		CreateExpresion(gaveIngredient);
		await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
		Squash();
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "position", new Vector2(Position.X, 500), 1.5f).SetTrans(Tween.TransitionType.Linear);
		tween.TweenCallback(new Callable(this, nameof(Destroy)));
		action?.Invoke();
	}

	private void Destroy()
	{
		QueueFree();
	}

	public void CreateExpresion(bool gaveIngredient)
	{
		var instance = _reaction.Instantiate();
		AddChild(instance);
        Reaction reaction = (Reaction)instance;
		reaction.Initialize(gaveIngredient ? Expresion.HAPPY_REACTION : Expresion.ANGRY_REACTION);
	}

	public int CalculateScore()
	{
		double value = _time /_slider.MaxValue;
		int result = (int)((value - (int)value) * 100);

		return result;
	}

	public void MoveTween(Vector2 target)
	{
		Squash();
		Random random = new ();
        double value = random.NextDouble();
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "position", target, value < 0.5f ? 0.25f : 0.5f).SetTrans(Tween.TransitionType.Elastic);
	}

	private void Squash()
	{
		Tween tween = GetTree().CreateTween();
		tween.Chain().TweenProperty(this, "scale", new Vector2(1.2f, 0.8f), 0.1f).SetTrans(Tween.TransitionType.Elastic);
		tween.Chain().TweenProperty(this, "scale", Vector2.One, 0.1f).SetTrans(Tween.TransitionType.Elastic);
	}
}
