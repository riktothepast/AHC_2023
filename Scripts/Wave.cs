using System;
using Godot;

public partial class Wave : Resource
{
    [Export]
	public double WaveTime { get; set; }
    [Export]
	public double TimePerRequest { get; set; }
    [Export]
	public int ClientQueueSize { get; set; }
    [Export]
	public Godot.Collections.Array<INGREDIENT_TYPE> AvailableIngredients { get; set; }

    public Wave() : this(0, 0, 0, null) {}

    public Wave(double waveTime, double timePerRequest, int clientQueueSize, Godot.Collections.Array<INGREDIENT_TYPE> ingredients)
    {
        WaveTime = waveTime;
        TimePerRequest = timePerRequest;
        ClientQueueSize = clientQueueSize;
        AvailableIngredients = ingredients;
    }
}
