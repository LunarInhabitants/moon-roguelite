using Godot;
using System;
using System.Collections.Generic;

public class DevConsole : Control
{
	private class DevConsoleLogEntry 
	{
		public Label label;
		public DateTime spawnTime;
	}
		
	const int MAX_LABEL_COUNT = 16;
	public static DevConsole Instance { get; private set; }
	
	[Export] public NodePath vboxContainerPath;
	private VBoxContainer vboxContainer;
	
	private List<DevConsoleLogEntry> logEntries = new List<DevConsoleLogEntry>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		vboxContainer = GetNode(vboxContainerPath) as VBoxContainer;
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		for(int i = 0; i < logEntries.Count; ++i)
		{
			if((DateTime.Now - logEntries[i].spawnTime).TotalSeconds > 10.0f)
			{
				_CleanupLogEntryAt(i);
				--i;
			}
		}
	}

	public static void Log(string logString, string prefix = "LOG", Color? color = null)
	{
		Instance._Log(logString, prefix, color ?? Colors.Aqua);
	}
	
	public static void Warn(string logString, string prefix = "LOG", Color? color = null)
	{
		Instance._Log(logString, prefix, color ?? Colors.Coral);
	}
	
	public static void Error(string logString, string prefix = "LOG", Color? color = null)
	{
		Instance._Log(logString, prefix, color ?? Colors.Crimson);
	}
	
	private void _Log(string logString, string prefix, Color color)
	{
		Label newLabel = new Label();
		newLabel.Text = $" [{prefix}] {logString}";
		newLabel.AddColorOverride("font_color", color);
		newLabel.AddColorOverride("font_color_shadow", new Color(0, 0, 0, 0.5f));
		newLabel.AddConstantOverride("shadow_as_outline", 1);
		
		vboxContainer.AddChild(newLabel);
		logEntries.Add(new DevConsoleLogEntry { label = newLabel, spawnTime = DateTime.Now });
		
		while(logEntries.Count > MAX_LABEL_COUNT)
		{
			_CleanupLogEntryAt(0);
		}
	}
	
	private void _CleanupLogEntryAt(int index)
	{
		var entry = logEntries[index];
		entry.label.QueueFree();
		logEntries.RemoveAt(index);
	}
}
