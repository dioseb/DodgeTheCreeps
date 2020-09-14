using Godot;
using System;

public class Mob : RigidBody2D
{	
	// types of mobs, each will have different animation
	private String[] _mobTypes = {"walk", "swim", "fly"};

	static private Random _random = new Random();
	
	private AnimatedSprite _sprite;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite>("AnimatedSprite");
		_sprite.Animation = _mobTypes[_random.Next(0, _mobTypes.Length)];
		_sprite.Play();
	}

	private void OnVisibilityExited()
	{
		QueueFree();
	}
}
