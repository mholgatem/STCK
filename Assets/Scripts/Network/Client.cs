using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ClientSockets;
using System.Security.Permissions;
using JsonFx.Json;
 
public class Client : MonoBehaviour {

	public static Client currentInstance;
	public SimpleClient socksClient = new SimpleClient();
	public float rate = .05f;
	public ImageRenderer IR;
	public byte[] image;
	private bool newScreenshot = false;

	private string ipAddress = PersistentVar.ip;
	private string hostName = PersistentVar.hostName;
	private int portNumber = PersistentVar.port;

	private string serverMsg = "Not connected";
	private string commandQueue = "";
	private string COMMAND_SEPARATOR = ",";
	private float rateTimer;
	private string[] queue = new string[100];
	private int qPlaceholder = 0;

	
	void Awake () {
		if( currentInstance == null ) {
			currentInstance = this;
			DontDestroyOnLoad(this.gameObject);
		}else{
			Destroy( this.gameObject );
		}
	}
	
	public void Start ()  {
		//needToRefreshUsers = false;
		serverMsg = "Connecting...";
		socksClient.ServerMessage += MessageDecoder;
		Connect();


	}
	
	void Update() {
		if (newScreenshot){
			if (IR == null)
				IR = GameObject.FindGameObjectWithTag("ScreenCap").GetComponent<ImageRenderer>();
			IR.ScreenShot(image);
			newScreenshot = false;
		}

		if (Application.loadedLevel == 0)
			this.gameObject.SetActive (false);
			//Destroy(this.gameObject);

		if (commandQueue.Length > 0 && rateTimer > rate) {
			qPlaceholder++;
			if (qPlaceholder == queue.Length)
				qPlaceholder = 0;

			//data string sent to server
			//format = {"qplaceholder": [{command1},{command2},...}]}
			queue[qPlaceholder] = string.Format(@"{{""{0}"": [{1}]}}", qPlaceholder, commandQueue.TrimEnd(','));
			socksClient.SendData(queue[qPlaceholder]);
			commandQueue = "";
			rateTimer = 0;
		}else{
			rateTimer += Time.deltaTime;
		}

		
	}
	
	public string GetLastServerMessage() {
		return serverMsg;	
	}
	
	public void Connect() {
		
		bool isConnected = socksClient.ConnectResult( ipAddress, portNumber );
		if(isConnected) {
			serverMsg = "Connected.";
			if (Application.loadedLevel < 2) 
				Application.LoadLevel (2);
		}
		else {
			serverMsg = "";
			Destroy ( this.gameObject );
			PersistentVar.failedHost.Add (PersistentVar.hostName);
			Application.LoadLevel ("Lobby");
		}
		
	}

	public void SendButtonState(string cmd, string type = "EV_KEY", int state = 0) {

		if(socksClient.isConnectedToServer()) {
			commandQueue += "{\"key\":\""+cmd+"\",\"type\":\""+type+"\",\"state\":\""+state.ToString()+"\"}" + COMMAND_SEPARATOR;
			//Connect();
		}
		else {
			serverMsg = "Not connected to server. Press Enter to connect.";
		}
	}

	public void SendAxisState(string axisValue, string type = "EV_ABS") {
		
		if(socksClient.isConnectedToServer()) {
			commandQueue += "{\"value\":\""+axisValue+"\",\"type\":\""+type+"\"}" + COMMAND_SEPARATOR;
			//Connect();
		}
		else {
			serverMsg = "Not connected to server. Press Enter to connect.";
		}
	}

	public void SendCommand(string cmd) {
		if(socksClient.isConnectedToServer()) {
			socksClient.SendData("{\"-1\" : [{\"key\":\""+cmd+"\",\"type\":\"EV_KEY\"}]}");
			//Connect();
		}
		else {
			serverMsg = "Not connected to server. Press Enter to connect.";
		}
	}
	
	public void Disconnect() {
		SendCommand("disconnect");
	}
	
	public void Shutdown() {
		SendCommand("srv_shutdown");
	}

	void MessageDecoder(string msg) {

		serverMsg = "Message from server: "+msg+"\n\n";
		Hashtable hash = JsonReader.Deserialize<Hashtable>(msg);
		if(hash.ContainsKey("screen")){
			image = Convert.FromBase64String(hash["screen"].ToString());
			newScreenshot = true;
		}
		if(hash.ContainsKey("disconnect")) {
			socksClient.Disconnect();
			serverMsg += "Disconnected from server.";
		}
		else if(hash.ContainsKey("srv_shutdown")) {
			socksClient.Disconnect();
			serverMsg += "Disconnected from server and server killed.";
		}
		else {
			serverMsg += "Server command not supported.";
		}
	}
 
	void OnApplicationQuit () {
		
		try
		{
			SendCommand("disconnect");
			socksClient.ServerMessage -= MessageDecoder;
		}
		catch{}
		
	}
}