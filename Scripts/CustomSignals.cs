using Godot;

[GlobalClass]
public partial class CustomSignals : Node
{
	[Signal]
    public delegate void AlignPlayerEventHandler(int newVaue);
	
	[Signal]
    public delegate void GameStartEventHandler();
	
	[Signal]
    public delegate void GameOverEventHandler(int finalScore);

    [Signal]
    public delegate void SetIngredientsEventHandler(Ingredient[] ingredients);

    [Signal]
    public delegate void SetTimerValueEventHandler(float timerTotalValue);

    [Signal]
    public delegate void HandleIngredientEventHandler(INGREDIENT_TYPE ingredient, bool force = false);

    [Signal]
    public delegate void HandleReactionCreationEventHandler(Expresion expression);
    
    [Signal]
    public delegate void CustomerLostPatienceEventHandler();
}
