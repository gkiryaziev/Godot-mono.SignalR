using Godot;
using System;
using Microsoft.AspNetCore.SignalR.Client;

public class ChatMessage
{
	public string GUID { get; set; }
	public string Name { get; set; }
	public string Message { get; set; }
}

public class ClientScene : Node2D
{
	HubConnection connection;

	LineEdit txtAddress, txtName, txtMessage;
	TextEdit txtResult;
	Button btnConnect, btnDisconnect, btnSend;

	public override void _Ready()
	{
		txtAddress = GetNode<LineEdit>("txtAddress");
		txtName = GetNode<LineEdit>("txtName");
		txtMessage = GetNode<LineEdit>("txtMessage");
		txtResult = GetNode<TextEdit>("txtResult");
		btnConnect = GetNode<Button>("btnConnect");
		btnDisconnect = GetNode<Button>("btnDisconnect");
		btnSend = GetNode<Button>("btnSend");

		btnConnect.Connect("pressed", this, nameof(BtnConnect_Click));
		btnDisconnect.Connect("pressed", this, nameof(BtnDisconnect_Click));
		btnSend.Connect("pressed", this, nameof(BtnSend_Click));
		txtMessage.Connect("text_entered", this, nameof(TxtMessage_TextEntered));

		ControlsOnDisconnected();
	}

	private void ControlsOnConnected()
	{
		btnConnect.Disabled = true;
		btnDisconnect.Disabled = false;
		btnSend.Disabled = false;
		txtMessage.Editable = true;
		if (!txtMessage.IsConnected("text_entered", this, nameof(TxtMessage_TextEntered)))
			txtMessage.Connect("text_entered", this, nameof(TxtMessage_TextEntered));
	}

	private void ControlsOnDisconnected()
	{
		btnConnect.Disabled = false;
		btnDisconnect.Disabled = true;
		btnSend.Disabled = true;
		txtMessage.Editable = false;
		if (txtMessage.IsConnected("text_entered", this, nameof(TxtMessage_TextEntered)))
			txtMessage.Disconnect("text_entered", this, nameof(TxtMessage_TextEntered));
	}

	private async void Connect()
	{
		connection = new HubConnectionBuilder().WithUrl(txtAddress.Text).Build();
		connection.On<ChatMessage>("ClientChatMessage", OnReceiveMessage);
		await connection.StartAsync();
	}

	private void OnReceiveMessage(ChatMessage chat)
	{
		AddResult($"{chat.Name} : {chat.Message}");
	}

	public void BtnConnect_Click()
	{
		try
		{
			Connect();
			AddResult("Connected.");
			ControlsOnConnected();
		}
		catch (Exception ex)
		{
			AddResult($"Error: {ex.InnerException.Message}");
		}
	}

	public async void BtnDisconnect_Click()
	{
		try
		{
			await connection.StopAsync();
			await connection.DisposeAsync();
			AddResult("Disconnected.");
			ControlsOnDisconnected();
		}
		catch (Exception ex)
		{
			AddResult(ex.Message);
		}
	}

	private async void BtnSend_Click()
	{
		try
		{
			var chat = new ChatMessage
			{
				GUID = "123",
				Name = txtName.Text,
				Message = txtMessage.Text,
			};

			await connection.InvokeAsync("ServerChatMessage", chat);
		}
		catch (Exception ex)
		{
			AddResult(ex.Message);
		}
	}

	public void AddResult(string result)
	{
		txtResult.Text += result + "\n";
	}

	private void TxtMessage_TextEntered(string text)
	{
		BtnSend_Click();
		txtMessage.Clear();
	}
}
