using Godot;
using System;

public class Player : KinematicBody2D
{
	[Signal]
	public delegate void Hit();
	
	[Signal]
	public delegate void Picked();

	[Export]
	public int Speed = 400; // How fast the player will move (pixels/sec).

	[Export]
	public Vector2 StartPosition = new Vector2(250, 450); // Start position

	private Vector2 _screenSize; // Size of the game window.
	private Vector2 _velocity; // movement direction

	// Node Position is immutable so we can only set the new object for Position
	// because we don't want to create new object each frame we will keep the position
	// in this local variable
	private Vector2 _position; // current position

	private AnimatedSprite _sprite;
	private Area2D _hitBox;
	private CollisionShape2D _collisionShape;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_screenSize = GetViewport().Size;
		_velocity = new Vector2();
		_sprite = GetNode<AnimatedSprite>("AnimatedSprite");
		_hitBox = GetNode<Area2D>("Hitbox");
		_collisionShape = _hitBox.GetNode<CollisionShape2D>("HitboxCollisionShape2D");
		Hide();
	}

	public void Start()
	{
		// set the position of the player
		_position = StartPosition;

		// show the player sprite
		Show();

		_collisionShape.Disabled = false;
	}

	// Change the target whenever a touch event happens.
	//	public override void _Input(InputEvent @event)
	//	{
	//		if (@event is InputEventScreenTouch eventMouseButton && eventMouseButton.Pressed)
	//		{
	//			_target = (@event as InputEventScreenTouch).Position;
	//		}
	//	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		setVelocity();
		setAnimation();
		setPosition(delta);
	}

	private void setVelocity()
	{
		_velocity.x = 0;
		_velocity.y = 0;

		if (Input.IsActionPressed("ui_right"))
		{
			_velocity.x += 1;
		}
		else if (Input.IsActionPressed("ui_left"))
		{
			_velocity.x -= 1;
		}

		if (Input.IsActionPressed("ui_down"))
		{
			_velocity.y += 1;
		}
		else if (Input.IsActionPressed("ui_up"))
		{
			_velocity.y -= 1;
		}
	}

	private void setAnimation()
	{
		if (_velocity.x != 0)
		{
			_sprite.Animation = "walk";
			_sprite.FlipH = _velocity.x < 0;
		}
		else if (_velocity.y != 0)
		{
			_sprite.Animation = "up";
			_sprite.FlipV = _velocity.y > 0;
		}

		if (_velocity.Length() > 0)
		{
			_velocity = _velocity.Normalized() * Speed;
			_sprite.Play();
		}
		else
		{
			_sprite.Stop();
		}
	}

	private void setPosition(float delta)
	{
		_position.x = Mathf.Clamp(_position.x + _velocity.x * delta, 0, _screenSize.x);
		_position.y = Mathf.Clamp(_position.y + _velocity.y * delta, 0, _screenSize.y);
		Position = _position;
	}

	private void _on_Hitbox_body_entered(object body)
	{
		if (body is Mob)
		{
			//GD.Print("Player hit by : " + body);
			EmitSignal("Hit");
		}
		
		if (body is CoinV2)
		{
			//GD.Print("Player collected item : " + body);
			EmitSignal("Picked", 10);
		}
	}
}
