using Godot;

public partial class Player : Node2D
{
	[Export] 
	private float _aligmentSpeed;
	private Ingredient[] _ingredients;
	private CustomSignals _customSignals;
	private int _currentIndex;

	public override void _Ready()
	{
		base._Ready();
        _customSignals = GetNode<CustomSignals>("/root/Signals");
        _customSignals.Connect("SetIngredients", new Callable(this, nameof(SetIngredientPoints)));
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("move_right"))
		{
			PlayerAligment(1);
		}
		if (Input.IsActionJustPressed("move_left"))
		{
			PlayerAligment(-1);
		}
		if (Input.IsActionJustPressed("give"))
		{
			GiveIngredient();
		}
	}

	private void SetIngredientPoints(Ingredient[] ingredients)
	{
		_ingredients = ingredients;
		_currentIndex = Mathf.FloorToInt((_ingredients.Length - 1) / 2f);
		Position = new Vector2(_ingredients[_currentIndex].Position.X, Position.Y);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.IsPressed())
			{
				switch(eventKey.Keycode)
				{
					case Key.Key1:
						ChangeIndex(0);
						break;
					case Key.Key2:
						ChangeIndex(1);
						break;
					case Key.Key3:
						ChangeIndex(2);
						break;
					case Key.Key4:
						ChangeIndex(3);
						break;
					case Key.Key5:
						ChangeIndex(4);
						break;
					case Key.Key6:
						ChangeIndex(5);
						break;
				}
			}
		}
	}

	private void ChangeIndex(int value)
	{
		_currentIndex = value;
		ChangePosition();
	}

	private void PlayerAligment(int value)
	{
		_currentIndex += value;
		ChangePosition();
	}

	private void ChangePosition()
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
		tween.Chain().TweenProperty(this, "scale", new Vector2(1.2f, 0.8f), 0.1f).SetTrans(Tween.TransitionType.Bounce);
		tween.Chain().TweenProperty(this, "scale", Vector2.One, 0.1f).SetTrans(Tween.TransitionType.Bounce);
	}

	private void GiveIngredient()
	{
        INGREDIENT_TYPE ingredient = _ingredients[_currentIndex].GetIngredient();
		_customSignals.EmitSignal("HandleIngredient", (int)ingredient, false);
	}
}
