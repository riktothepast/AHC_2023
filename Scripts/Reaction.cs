using Godot;
using System;

public partial class Reaction : Node
{
	[Export]
	private Texture2D _superReaction;
	[Export]
	private Texture2D _happyReaction;
	[Export]
	private Texture2D _sadReaction;
	[Export]
	private Texture2D _angryReaction;
	[Export]
	private float _speed;
	private Sprite2D _sprite2D;

	public override void _Ready()
	{
        _sprite2D = GetNode<Sprite2D>("Sprite2D");
	}

	public override void _Process(double delta)
	{
        Vector2 position = _sprite2D.Position;
		position.Y -= (float)delta * _speed;
	}

	public void Initialize(Expresion expresion)
	{
		switch(expresion)
		{
			case Expresion.SUPER_REACTION:
				_sprite2D.Texture = _superReaction;
				break;
			case Expresion.HAPPY_REACTION:
				_sprite2D.Texture = _happyReaction;
				break;
			case Expresion.SAD_REACTION:
				_sprite2D.Texture = _sadReaction;
				break;
			case Expresion.ANGRY_REACTION:
				_sprite2D.Texture = _angryReaction;
				break;
		}
	}
}

public enum Expresion
{
	SUPER_REACTION,
	HAPPY_REACTION,
	SAD_REACTION,
	ANGRY_REACTION
}
