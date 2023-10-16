using System.Collections.Generic;
using Godot;

public partial class GameManager : Node
{
	[Export]
	private Wave[] _waves;
	[Export]
	private Ingredient[] _ingredients;
	[Export]
	private Node2D _aligmentPoint;
	[Export]
	private TimerBar _timerBar;
	private int _currentIndex;
	private PackedScene _customer;
	private Queue<Customer> _customers;
	private bool _notAligned;
	private Dictionary<INGREDIENT_TYPE, Texture2D> _ingredientsDict;
	private bool _gameRunning;
	private double _currentTime;
	private double _currentTimePerRequest;
	private int _currentClientQueueSize;
	private int _currentWaveIndex;
	private Godot.Collections.Array<INGREDIENT_TYPE> _availableIngredients;
	private INGREDIENT_TYPE _lastIngredient;
	private CustomSignals _customSignals;

	public override void _Ready()
	{
		_customer = GD.Load<PackedScene>("res://Scenes/Customer.tscn");
		_customSignals = (CustomSignals)GetNode("/root/Signals");
		_customSignals.EmitSignal("SetIngredients", _ingredients);
        _customSignals.Connect("HandleIngredient", new Callable(this, nameof(HandleIngredientRequest)));
		_customSignals.Connect("CustomerLostPatience", new Callable(this, nameof(RemoveCurrentCustomer)));
		_customers = new Queue<Customer>();

		_ingredientsDict = new Dictionary<INGREDIENT_TYPE, Texture2D>();
		for(int x = 0; x< _ingredients.Length; x++)
		{
			_ingredientsDict.Add(_ingredients[x].GetIngredient(), _ingredients[x].GetTexture());
		}
		SetupNewWave();
		CreateCustomers();
		StartGame();
	}

	private void SetupNewWave()
	{
        Wave wave = _waves[_currentWaveIndex];
		_currentTime = wave.WaveTime;
		_currentTimePerRequest = wave.TimePerRequest;
		_currentClientQueueSize = wave.ClientQueueSize;
		_availableIngredients = wave.AvailableIngredients;
		_currentWaveIndex++;

		if (_currentWaveIndex > _waves.Length - 1)
		{
			_currentWaveIndex = +_waves.Length - 1;
		}
	}

	public override void _Process(double delta)
	{
		if(_gameRunning && !_notAligned)
		{
			_currentTime -= delta;
			_timerBar.UpdateTimer(_currentTime);
		}
	}

	public void StartGame()
	{
		_timerBar.SetTimer(_currentTime);
		_gameRunning = true;
		ActivateNewCustomer();
	}

	private void ActivateNewCustomer()
	{
		if (_customers.Count <= 0)
		{
			SetupNewWave();
			CreateCustomers();
			StartGame();
			return;
		}
        Customer customer = _customers.Peek();
		customer.Initialize(_currentTimePerRequest);
	}

	private void CreateCustomers()
	{
		_customers.Clear();
		for(int x = 0; x < _currentClientQueueSize; x ++)
		{
            Node node = CreateNewCustomer();
            Customer customer = (Customer)node;
            Vector2 position = _aligmentPoint.Position;
			position.X += 200;
			customer.Position = position;
			INGREDIENT_TYPE ingredientForCustomer = GetIngredient();
            Texture2D texture = _ingredientsDict.GetValueOrDefault(ingredientForCustomer, null);
			customer.SetIngredient(ingredientForCustomer, texture);
			customer.Move(new Vector2(160f * x, _aligmentPoint.Position.Y));
			_customers.Enqueue(customer);
		}
	}

	private INGREDIENT_TYPE GetIngredient()
	{
		System.Random random = new System.Random();
		INGREDIENT_TYPE randomItem = _availableIngredients[random.Next(_availableIngredients.Count)];
		if (randomItem == _lastIngredient)
		{
			return GetIngredient();
		}
		_lastIngredient = randomItem;

		return randomItem;
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

		if (customer.GetIngredient() == ingredient)
		{
			_customerSatisfaction = true;
			int score = customer.CalculateScore();
			_currentTime += customer.CalculateScore() / 100f;
			_customSignals.EmitSignal("IncreaseScore", score);
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
