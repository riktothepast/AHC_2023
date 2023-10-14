using Godot;

public partial class ItemStruct : Node
{
    [Export]
    public Texture2D sprite;
    [Export]
    public INGREDIENT_TYPE ingredientType;
}