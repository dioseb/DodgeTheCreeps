using Godot;
using System;

public class Main : Node2D
{
	// Don't forget to rebuild the project so the editor knows about the new export variable.

	[Export]
	public PackedScene Mob;

	[Export]
	public PackedScene CoinV2;

	public int MinSpeed = 50; // Minimum speed range.
	public int MaxSpeed = 150; // Maximum speed range.
	public float WatiTimeBeforeSpawn = 10; // Maximum speed range.
	public string DifficultyText;

	private int _score;
	private int _timer;

	// We use 'System.Random' as an alternative to GDScript's random methods.
	private Random _random = new Random();
	private Player _player;

	private Timer _startTimer;
	private Timer _mobTimer;
	private Timer _coinTimer;
	private Timer _timerTimer;
	private Timer _scoreTimer;

	private HUD _hud;

	private PathFollow2D _mobSpawnLocation;
	private PathFollow2D _coinSpawnLocation;

	public override void _Ready()
	{
		_player = GetNode<Player>("Player");
		_mobTimer = GetNode<Timer>("MobTimer");
		_coinTimer = GetNode<Timer>("CoinTimer");
		_startTimer = GetNode<Timer>("StartTimer");
		_timerTimer = GetNode<Timer>("TimerTimer");
		_scoreTimer = GetNode<Timer>("ScoreTimer");
		_hud = GetNode<HUD>("HUD");
		_mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		_coinSpawnLocation = GetNode<PathFollow2D>("CoinPath/CoinSpawnLocation");
	}

	private void NewGame()
	{
		_timer = 0;
		_score = 0;
		_hud.UpdateTimer(_timer);
		_hud.ShowMessage("Arrows to move");

		_startTimer.Start();
	}

	private void SelectDifficulty(int idx, string text)
	{
		DifficultyText = text;
		if (idx == 0)
		{
			WatiTimeBeforeSpawn = _score > 30 ? 1.0f : (-0.03333f * _score + 1.5f);;
			MinSpeed = 50;
			MaxSpeed = 100;
		}
		if (idx == 1)
		{
			WatiTimeBeforeSpawn = _score > 20 ? 0.5f : (-0.03333f * _score + 1.0f);;
			MinSpeed = 150;
			MaxSpeed = 300;
		}
		if (idx == 2)
		{
			WatiTimeBeforeSpawn = _score > 10 ? 0.3f : (-0.03333f * _score + 0.5f);;
			MinSpeed = 300;
			MaxSpeed = 500;
		}
		if (idx == 3)
		{
			WatiTimeBeforeSpawn = _score > 5 ? 0.2f : (-0.03333f * _score + 0.2f);;
			MinSpeed = 500;
			MaxSpeed = 700;
		}
	}

	// We'll use this later because C# doesn't support GDScript's randi().
	private float RandRange(float min, float max)
	{
		return (float)_random.NextDouble() * (max - min) + min;
	}

	private void gameOver()
	{
		// lets clear up the scene
		foreach (var node in GetChildren())
		{
			if (node is Mob || node is CoinV2)
			{
				((RigidBody2D)node).Hide();
				((RigidBody2D)node).QueueFree();
			}
		}

		// stop all the timers
		_mobTimer.Stop();
		_coinTimer.Stop();
		_timerTimer.Stop();
		_scoreTimer.Stop();

		// show a game over message
		_hud.ShowGameOver();

		// hide the player
		_player.Hide();
	}

	private void PlayerPicked(object body)
	{
		_score = _score += (int)body;
		_hud.UpdateScore(_score);
		// GD.Print("La scene récupère : " + body);
	}

	private void MobTimerTimeout()
	{
		// Choose a random location on Path2D.
		_mobSpawnLocation.Offset = _random.Next();

		// Create a Mob instance and add it to the scene.
		var mobInstance = (RigidBody2D)Mob.Instance();
		AddChild(mobInstance);

		// Set the mob's direction perpendicular to the path direction.
		float direction = _mobSpawnLocation.Rotation + Mathf.Pi / 2;

		// Set the mob's position to a random location.
		mobInstance.Position = _mobSpawnLocation.Position;

		// Add some randomness to the direction.
		direction += RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
		mobInstance.Rotation = direction;

		GD.Print("Difficulty : " + DifficultyText + ", Min speed : " + MinSpeed + ", Max speed : " + MaxSpeed + ", Spawn count : " + WatiTimeBeforeSpawn);
		// Choose the velocity.
		mobInstance.LinearVelocity = (new Vector2(RandRange(MinSpeed, MaxSpeed), 0).Rotated(direction));

		// decrease timer
		_mobTimer.WaitTime = WatiTimeBeforeSpawn;
	}

	private void CoinTimerTimeout()
	{
		// Choose a random location on Path2D.
		_coinSpawnLocation.Offset = _random.Next();

		// Create a Coin instance and add it to the scene.
		var coinInstance = (RigidBody2D)CoinV2.Instance();
		AddChild(coinInstance);

		// Set the mob's position to a random location.
		coinInstance.Position = _coinSpawnLocation.Position;

		// decrease timer
		_coinTimer.WaitTime = 5;
	}

	private void TimerTimerTimeout()
	{
		_timer++;
		_hud.UpdateTimer(_timer);
	}

	private void StartTimerTimeout()
	{
		_player.Start();
		_mobTimer.Start();
		_coinTimer.Start();
		_timerTimer.Start();
		_scoreTimer.Start();
	}

	private void HUDStartGame(int idx, string text)
	{
		SelectDifficulty(idx, text);
		NewGame();
	}
}
