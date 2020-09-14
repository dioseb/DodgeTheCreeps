using Godot;
using System;

public class Heart : Area2D
{
	private async void _on_heart_body_entered(object body)
	{
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
		var tween = GetNode<Tween>("Tween");

		tween.InterpolateProperty(
			this,
			"position",
			Position,
			new Vector2(Position.x, Position.y - 300),
			1,
			Tween.TransitionType.Back,
			Tween.EaseType.InOut);

		tween.Start();
		await ToSignal(tween, "tween_completed");

		QueueFree();
	}
}
