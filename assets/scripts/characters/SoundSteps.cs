using Godot;
using Godot.Collections;

namespace RoyalCupcakes.Characters;

public partial class SoundSteps : AudioStreamPlayer3D
{
	private const double WalkTime = 0.3f;
	private const double RunTime = 0.6f;
	
	[Export] private string currentMaterial;
	[Export] private Dictionary<string, Array<AudioStream>> walkSteps = new()
	{
		["stone"] = new Array<AudioStream>(),
		["carpet"] = new Array<AudioStream>(),
	};

	[Export] private Dictionary<string, Array<AudioStream>> runSteps = new()
	{
		["stone"] = new Array<AudioStream>(),
		["carpet"] = new Array<AudioStream>(),
	};

	[Export] private AudioStream tied;

	private Character parent;
	private double timer;
	private int walkI, runI;

	private Array<AudioStream> currentWalk => walkSteps[currentMaterial];
	private Array<AudioStream> currentRun  => runSteps[currentMaterial];

	public void SetMaterial(string material)
	{
		if (!walkSteps.ContainsKey(material) || !runSteps.ContainsKey(material)) return;
		currentMaterial = material;
	}

	public override void _Ready()
	{
		currentWalk.Shuffle();
		currentRun.Shuffle();
		parent = GetParent<Character>();
		parent.Tied += () =>
		{
			Stream = tied;
			Play();
		};
	}

	public override void _Process(double delta)
	{
		if (!parent.IsMoving) return;

		if (timer > 0)
		{
			timer -= delta;
		}
		else
		{
			timer = parent.IsRunning ? RunTime : WalkTime;
			GetStepStream();
			Play();
		}
	}

	private void GetStepStream()
	{
		if (parent.IsRunning)
		{
			Stream = currentRun[runI];
			if (runI < currentRun.Count - 1)
			{
				runI += 1;
			}
			else
			{
				runI = 0;
				currentRun.Shuffle();
			}
		}
		else
		{
			Stream = currentWalk[walkI];
			if (walkI < currentWalk.Count - 1)
			{
				walkI += 1;
			}
			else
			{
				walkI = 0;
				currentWalk.Shuffle();
			}
		}
	}
}
