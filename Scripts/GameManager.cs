using System.Collections.Generic;
using Godot;

public partial class GameManager : Node
{
	[Export]
	private float _timePerRequest = 2f;
	[Export]
	private int _CustomersPerRound = 10;
	[Export]
	private Ingredient[] _ingredients;
	[Export]
	private Node2D _aligmentPoint;
	private int _currentIndex;
	private PackedScene _customer;
	private Queue<Customer> _customers;
	private bool _notAligned;
	private Dictionary<INGREDIENT_TYPE, Texture2D> _ingredientsDict;

	public override void _Ready()
	{
		_customer = GD.Load<PackedScene>("res://Scenes/Customer.tscn");
		CustomSignals customSignals = (CustomSignals)GetNode("/root/Signals");
		customSignals.EmitSignal("SetIngredients", _ingredients);
		customSignals.EmitSignal("SetTimerValue", 1);
        customSignals.Connect("HandleIngredient", new Callable(this, nameof(HandleIngredientRequest)));
		customSignals.Connect("CustomerLostPatience", new Callable(this, nameof(RemoveCurrentCustomer)));
		_customers = new Queue<Customer>();

		_ingredientsDict = new Dictionary<INGREDIENT_TYPE, Texture2D>();
		for(int x = 0; x< _ingredients.Length; x++)
		{
			_ingredientsDict.Add(_ingredients[x].GetIngredient(), _ingredients[x].GetTexture());
		}

		CreateCustomers();
		StartGame();
	}

	public override void _Process(double delta)
	{
		
	}

	public void StartGame()
	{
		ActivateNewCustomer();
	}

	private void ActivateNewCustomer()
	{
		if (_customers.Count <= 0)
		{
			return;
		}
        Customer customer = _customers.Peek();
		customer.Initialize(_timePerRequest);
	}

	private void CreateCustomers()
	{
		_customers.Clear();
		for(int x = 0; x < _CustomersPerRound; x ++)
		{
            Node node = CreateNewCustomer();
            Customer customer = (Customer)node;
            System.Array array = INGREDIENT_TYPE.GetValues(typeof(INGREDIENT_TYPE));
            System.Random random = new System.Random();
			INGREDIENT_TYPE randomItem = (INGREDIENT_TYPE)array.GetValue(random.Next(array.Length));
            Texture2D texture = _ingredientsDict.GetValueOrDefault(randomItem, null);
			customer.SetIngredient(randomItem, texture);
			customer.Move(new Vector2(160f * x, _aligmentPoint.Position.Y));
			_customers.Enqueue(customer);
		}
	}

	private void HandleIngredientRequest(INGREDIENT_TYPE ingredient, bool forceRemoval = false)
	{
		if (_customers.Count <= 0 || _notAligned)
		{
			return;
		}
		bool _customerSatisfaction = false;
        Customer customer = _customers.Dequeue();
		if (forceRemoval)
		{
			_notAligned = true;
			customer.GetOut(_customerSatisfaction, ReAlignCustomers);
			return;
		}

		GD.Print($"Customer wanted: {customer.GetIngredient()}, but player gave {ingredient}");
		if (customer.GetIngredient() == ingredient)
		{
			_customerSatisfaction = true;
			GD.Print($"got points : {customer.CalculateScore()}");
		} else {
			_customerSatisfaction = false;
		}
		_notAligned = true;
		customer.GetOut(_customerSatisfaction, ReAlignCustomers);
	}

	private void RemoveCurrentCustomer()
	{
		HandleIngredientRequest(INGREDIENT_TYPE.SAUSAGE, true);
	}

	private void ReAlignCustomers()
	{
		foreach ( Customer customer in _customers )
		{
			Vector2 newPos = customer.Position;
			newPos.X -= 160;
			customer.Move(newPos);
		}
		ActivateNewCustomer();
		_notAligned = false;
	}

	private Node CreateNewCustomer()
	{
		var instance = _customer.Instantiate();
		AddChild(instance);
		return instance;
	}
}
