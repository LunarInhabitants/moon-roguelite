using Godot;
using System;
using System.Collections.Generic;

public class DEV_SceneButton : MenuButton
{
	[Export] public Dictionary<string, string> sceneList = new Dictionary<string, string>();
	
	private Dictionary<int, string> sceneIdMappings = new Dictionary<int, string>();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var popup = GetPopup();
		popup.Connect("id_pressed", this, nameof(OnItemPressed));
		int x = 0;
		foreach(var kvp in sceneList)
		{
			++x;
			sceneIdMappings[x] = kvp.Value;
			popup.AddItem(kvp.Key, x);
		}
	}
	
	private void OnItemPressed(int id)
	{
		string fullPath = $"res://scenes/devtest/{sceneIdMappings[id]}.tscn";
		var levelNode = GetNode("/root/GameRoot/Level");
		DevConsole.Log($"Changing scene to '{fullPath}'", "DEV");

		var newLevelResource = ResourceLoader.Load(fullPath) as PackedScene;
		if(newLevelResource == null)
		{
			DevConsole.Error($"Could not load scene '{fullPath}'! ResourceLoader.Load returned null. Did you spell the scene name correctly?");
			return;
		}
		
		var newLevel = newLevelResource.Instance();
		foreach(var child in levelNode.GetChildren())
		{
			if(child is Node n)
			{
				n.QueueFree();
			}
		}
		
		levelNode.AddChild(newLevel);
	}
}

public struct DevSceneEntry 
{
	[Export] public string label;
	[Export(PropertyHint.File, "*.tscn")] public string scene;
}
