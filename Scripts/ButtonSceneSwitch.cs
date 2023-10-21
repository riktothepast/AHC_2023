using Godot;
using System;

public partial class ButtonSceneSwitch : Button
{
    [Export]
    string SceneName;
    private SceneSwitch _sceneSwitch;

    public override void _Ready()
    {
        _sceneSwitch = (SceneSwitch)GetNode("/root/SceneSwitch");
        Pressed += OnButtonPressed;
    }

    private void ChanceScene(string sceneName)
    {
        _sceneSwitch.GotoScene($"res://Scenes/{sceneName}.tscn");
    }

    private void OnButtonPressed()
    {
        ChanceScene(SceneName);
    }
}
