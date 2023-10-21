using System;
using Godot;

public partial class Player : Node2D
{
	[Export] 
	private float _aligmentSpeed;
	private Ingredient[] _ingredients;
	private CustomSignals _customSignals;
	private int _currentIndex;
	private bool _isGameOver;

	public override void _Ready()
	{
		base._Ready();
        _customSignals = GetNode<CustomSignals>("/root/Signals");
        _customSignals.Connect("SetIngredients", new Callable(this, nameof(SetIngredientPoints)));
		_customSignals.Connect("GameOver", new Callable(this, nameof(GameOver)));
	}

	public override void _Process(double delta)
	{
	}

	private void SetIngredientPoints(Ingredient[] ingredients)
	{
		_ingredients = ingredients;
		_currentIndex = Mathf.FloorToInt((_ingredients.Length - 1) / 2f);
		Position = new Vector2(_ingredients[_currentIndex].Position.X, Position.Y);
	}

	private void GameOver(int finalScore)
	{
		_isGameOver = true;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if(_isGameOver)
		{
			return;
		}
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.IsPressed())
			{
				switch(eventKey.Keycode)
				{
					case Key.A:
						ChangeIndex(0);
						break;
					case Key.S:
						ChangeIndex(1);
						break;
					case Key.D:
						ChangeIndex(2);
						break;
					case Key.J:
						ChangeIndex(3);
						break;
					case Key.K:
						ChangeIndex(4);
						break;
					case Key.L:
						ChangeIndex(5);
						break;
				}
			}
		}
	}

	private void ChangeIndex(int value)
	{
		_currentIndex = value;
		ChangePosition(GiveIngredient);
	}

	private void ChangePosition(Action onComplete = null)
	{
		if(_currentIndex < 0)
		{
			_currentIndex = _ingredients.Length - 1;
		}
		if(_currentIndex > _ingredients.Length - 1)
		{
			_currentIndex = 0;
		}
		Position = new Vector2(_ingredients[_currentIndex].Position.X, Position.Y);
		Tween tween = GetTree().CreateTween();
		if (onComplete != null)
		{
			tween.TweenCallback(new Callable(this, nameof(GiveIngredient)));
		}
		tween.Chain().TweenProperty(this, "scale", new Vector2(1.2f, 0.8f), 0.1f).SetTrans(Tween.TransitionType.Bounce);
		tween.Chain().TweenProperty(this, "scale", Vector2.One, 0.1f).SetTrans(Tween.TransitionType.Bounce);
	}

	private void GiveIngredient()
	{
        INGREDIENT_TYPE ingredient = _ingredients[_currentIndex].GetIngredient();
		_customSignals.EmitSignal("HandleIngredient", (int)ingredient, false);
	}
}
