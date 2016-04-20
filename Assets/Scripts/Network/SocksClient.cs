using UnityEngine;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using JsonFx.Json;
using System.Security.Cryptography;

/// <summary>
/// This class handles all of the actual
/// network stuff.
/// </summary>
public class SocksClient {
	
	const int READ_BUFFER_SIZE = 1024;
	private TcpClient client;
	private byte[] readBuffer = new byte[READ_BUFFER_SIZE];
	public string strMessage = string.Empty;
	
	public event Action<string> ServerMessage = null;

	public SocksClient(){}
	public StreamWriter writer;
	public NetworkStream stream;

	public bool ConnectResult(string sNetIP, int iPORT_NUM) {
		if(isConnectedToServer(true))
			return true;
		try 
		{
			client = new TcpClient(sNetIP, iPORT_NUM);
			writer = new StreamWriter(client.GetStream());
			stream = client.GetStream();
			client.SendTimeout = 2;	
			
			// Start an asynchronous read invoking DoRead to avoid lagging the user interface.
			client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(DoRead), null);
			
			return true;
		} 
		catch
		{
			return false;
		}
	}
	
	public void Disconnect() {
		try
		{
			client.Close();
			client = null;
		}
		catch {}
	}
	
	public bool isConnectedToServer(bool verify = false) {


		if(client == null){
			return false;
		}else{
			if (verify){
				return !((client.Client.Poll(1000, SelectMode.SelectRead) && (client.Client.Available == 0)) || !client.Client.Connected);
			}else{
				return client.Connected;
			}
		}
	}

	private void DoRead(IAsyncResult ar) { 
		int BytesRead;
		try
		{

			// Finish asynchronous read into readBuffer and return number of bytes read.
			BytesRead = client.GetStream().EndRead(ar);
			if (BytesRead < 1) 
			{
				Debug.Log ("no bytes read");
				// if no bytes were read server has close.
				return;
			}
			// Convert the byte array the message was saved into, minus two for the
			// Chr(13) and Chr(10)
			strMessage += Encoding.UTF8.GetString(readBuffer, 0, BytesRead);
			char last = strMessage[strMessage.Length - 1];
			if (last == '}'){
				ProcessCommands(strMessage);
				strMessage = "";
			}


			// Start a new asynchronous read into readBuffer.
			client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(DoRead), null);

		} 
		catch
		{
			Debug.LogWarning("Disconnected");
			client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(DoRead), null);
		}
	}

	// Process the command received from the server, and send it back to listener.
	private void ProcessCommands(string strMessage) {
		if(ServerMessage != null) {
			ServerMessage(strMessage);
		}
	}

	// Use a StreamWriter to send a message to server.
	public void SendData(string message) {
		//StreamWriter writer = new StreamWriter(client.GetStream());
		byte[] data;
		if (ClientSettings.disableEncryption){
			data = System.Text.Encoding.UTF8.GetBytes(message);
		}else{
			string encrypted = CipherUtility.Encrypt<AesManaged>(message, ClientSettings.SecretKey , ClientSettings.Salt);
			data = Encoding.ASCII.GetBytes(encrypted); 
		}
		stream.Write (data, 0 , data.Length);
	}

}