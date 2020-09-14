using Godot;
using System;

public class HUD : CanvasLayer
{
	[Signal]
	public delegate void StartGame();
	
	[Signal]
	public delegate void SelectDifficulty();
	
	public void ShowMessage(string text, int time = 1)
	{
		var message = GetNode<Label>("MessageLabel");
		message.Text = text;
		message.Show();
	
		var _messageTimer = GetNode<Timer>("MessageTimer");
		_messageTimer.WaitTime = time;
		_messageTimer.Start();
	}
	
	public async void ShowGameOver()
	{
		ShowMessage("Game Over");
	
		var messageTimer = GetNode<Timer>("MessageTimer");
		await ToSignal(messageTimer, "timeout");
	
		var message = GetNode<Label>("MessageLabel");
		message.Text = "Dodge the\nCreeps!";
		message.Show();		
	
		await ToSignal(GetTree().CreateTimer(1), "timeout");
		GetNode<Button>("StartButton").Show();
		GetNode<OptionButton>("OptionButton").Show();
		GetNode<Label>("DifficultyLabel").Show();
		
		GetTree().ReloadCurrentScene();
	}
	
	public void UpdateTimer(int timer)
	{
		GetNode<Label>("TimerLabel").Text = timer.ToString();
	}

	public void UpdateScore(int score)
	{
		GetNode<Label>("ScoreLabel").Text = score.ToString();
	}
	
	private void _on_MessageTimer_timeout()
	{
		GetNode<Label>("MessageLabel").Hide();
	}
		
	private void _on_StartButton_pressed()
	{
		var option = GetNode<OptionButton>("OptionButton");
		var idx = option.Selected;
		var text = option.GetItemText(idx);
		
		GetNode<Button>("StartButton").Hide();
		GetNode<OptionButton>("OptionButton").Hide();
		GetNode<Label>("DifficultyLabel").Hide();
		EmitSignal("StartGame", idx, text);
	}
}
