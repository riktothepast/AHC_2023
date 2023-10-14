using Godot;
using System;

public partial class Ingredient : Node2D
{
    [Export]	
	private INGREDIENT_TYPE _ingredientType;
	[Export]	
	private Texture2D _texture2D;
	private Sprite2D _sprite2D;

	public override void _Ready()
	{
		_sprite2D = GetNode<Sprite2D>("Sprite2D");
		_sprite2D.Texture = _texture2D;
	}

	public override void _Process(double delta)
	{
	}

	public INGREDIENT_TYPE GetIngredient()
	{
		return _ingredientType;
	}

	public Texture2D GetTexture()
	{
		return _texture2D;
	}
}
