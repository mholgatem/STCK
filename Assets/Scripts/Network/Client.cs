using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Cryptography;
using JsonFx.Json;
 
public class Client : MonoBehaviour {

	public static Client currentInstance;
	public SocksClient socksClient = new SocksClient();
	public float rate = .05f;

	// screenshot
	[HideInInspector]public Screenshot screenshotWindow;
	[HideInInspector]public byte[] image;
	private bool newScreenshot = false;

	public static string serverIP = "";
	public static string serverHostName = "";
	public static int serverPort = -1;
	public static int packetSize = 1024;

	public static List<string> failedHost = new List<string>();
	public static string secretKey, salt;

	private string commandQueue = "";
	private string COMMAND_SEPARATOR = ",";
	private float rateTimer;
	private string[] queue = new string[100];
	private int qPlaceholder = 0;


	public void Awake(){
		// SINGLETON
		if( currentInstance == null ) {
			currentInstance = this;
			DontDestroyOnLoad(this);
		}else if (currentInstance != this){
			Destroy (this.gameObject);
		}
	}
	
	public void OnEnable ()  {
		socksClient.ServerMessage += MessageDecoder;
		if (serverIP != "" && serverPort >= 0)
			Connect();
	}

	public void OnDisable () {
		Disconnect();
	}

	void OnApplicationQuit () {
		Disconnect();
	}
	
	void Update() {

		// disable client when in lobby
		if (Application.loadedLevel == 0){
			this.gameObject.SetActive (false);
			return;
		}
		
		if (newScreenshot){
			if (screenshotWindow == null)
				screenshotWindow = GameObject.FindGameObjectWithTag("ScreenCap").GetComponent<Screenshot>();
			screenshotWindow.LoadImage(image);
			newScreenshot = false;
		}

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
		}else if (rateTimer <= rate){
			rateTimer += Time.deltaTime;
		}
	}
	
	public void Connect() {
		bool isConnected = socksClient.ConnectResult( serverIP, serverPort );
		if(isConnected) {
			if (Application.loadedLevel == 0) {
				Application.LoadLevel (1);
			}
		}
		else {
			socksClient.Disconnect();
			MessageBox.Show ("Try Again?","Could not connect to server.\nRetry?", MessageBox.YesNo, UserReconnectResponse);
		}
		
	}

	void UserReconnectResponse(UserResponse answer){
		if (answer == UserResponse.Yes){
			Connect();
		}else{
			Disconnect();
			failedHost.Add(serverHostName);
			Application.LoadLevel(0);
		}
	}

	public void SendButtonState(string cmd, string type = "EV_KEY", int state = 0) {

		if(socksClient.isConnectedToServer()) {
			commandQueue += "{\"key\":\""+cmd+"\",\"type\":\""+type+"\",\"state\":\""+state.ToString()+"\"}" + COMMAND_SEPARATOR;
		}else {
			MessageBox.Show ("Reconnect?","Unexpectedly disconnected from server.\nTry to reconnect?", MessageBox.YesNo, UserReconnectResponse);
		}
	}

	public void SendAxisState(string axisValue, string type = "EV_ABS") {
		
		if(socksClient.isConnectedToServer()) {
			commandQueue += "{\"value\":\""+axisValue+"\",\"type\":\""+type+"\"}" + COMMAND_SEPARATOR;
		}else {
			MessageBox.Show ("Reconnect?","Unexpectedly disconnected from server.\nTry to reconnect?", MessageBox.YesNo, UserReconnectResponse);
		}
	}

	public void SetServerOption(string cmd) {
		if(socksClient.isConnectedToServer()) {
			socksClient.SendData("{\"-1\" : [{\"option\":\""+cmd+"\",\"type\":\"SET_OPTION\",\"value\":\"begin\"}]}");
		}else {
			MessageBox.Show ("Reconnect?","Unexpectedly disconnected from server.\nTry to reconnect?", MessageBox.YesNo, UserReconnectResponse);
		}
	}

	public void SendCommand(string cmd) {
		if(socksClient.isConnectedToServer()) {
			socksClient.SendData("{\"-1\" : [{\"key\":\""+cmd+"\",\"type\":\"EV_KEY\"}]}");
		}else if(cmd != "disconnect" && cmd != "srv_shutdown"){
			MessageBox.Show ("disconnectReconnect?","Unexpectedly disconnected from server.\nTry to reconnect?", MessageBox.YesNo, UserReconnectResponse);
		}
	}
	
	public void Disconnect() {
		try
		{
			SendCommand("disconnect");
			socksClient.Disconnect();
			socksClient.ServerMessage -= MessageDecoder;
		}
		catch{}
	}
	
	public void Shutdown() {
		try
		{
			SendCommand("srv_shutdown");
			socksClient.Disconnect();
			socksClient.ServerMessage -= MessageDecoder;
		}
		catch{}
	}

	/// <summary>
	/// method subscribed to [socksClient.ServerMessage]
	/// </summary>
	void MessageDecoder(string msg) {
		
		Hashtable hash = JsonReader.Deserialize<Hashtable>(msg);
		if(hash.ContainsKey("screen")){
			image = Convert.FromBase64String(hash["screen"].ToString());
			newScreenshot = true;
		}
		if(hash.ContainsKey("disconnect")) {
			socksClient.Disconnect();
		}
		else if(hash.ContainsKey("srv_shutdown")) {
			socksClient.Disconnect();
		}
		else {
		}
	}
 
}