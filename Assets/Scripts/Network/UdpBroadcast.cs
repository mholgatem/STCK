using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;
using JsonFx.Json;


[System.Serializable]
public class settingObjects
{
	public GameObject settingMenu;
	public Toggle menuToggle;
	public Toggle broadcastToggle;
	public InputField broadcastIP;
	public InputField keyField;
	public Slider pingSlider;
	public Text pingValue;
	public InputField manualServers;
}

public class UdpBroadcast : MonoBehaviour {
	
	// receiving Thread
	Thread receiveThread;
	private bool runThread = true;

	// UDP Broadcast Client
	UdpClient bcastClient;
	public static UdpBroadcast currentInstance;

	// set prefabs + link game objects
	public Text overlayText;
	public GameObject buttonTemplate;

	[Header("Settings Menu Objects")]
	public settingObjects settings;

	// internals to keep track
	[HideInInspector] public List<ServerSettings> servers = new List<ServerSettings>();
	private List<ServerSettings> addresses = new List<ServerSettings>();
	private float pingTimer;
	private GameObject ServerList;


	void Awake () {
		// force application to framerate of 30 to avoid
		// battery consumption
		QualitySettings.vSyncCount = 0;  // VSync must be disabled
		Application.targetFrameRate = 30;
	}

	public void Start(){
		// set static currentInstance for easy access
		currentInstance = this;

		// register CloseMenu method
		PlayerActionInterpreter.EscapePressed += CloseMenu;

		// Find server list
		ServerList = GameObject.Find("ServerList/Scroll View/Viewport/Content");

		// BROADCAST SETTINGS
		settings.broadcastToggle.isOn = ClientSettings.BroadcastInvitation;
		settings.broadcastIP.text = ClientSettings.BroadcastAddress;

		// SECRET KEY SETTINGS
		settings.keyField.text = ClientSettings.SecretKey;

		// PING RATE SETTINGS
		settings.pingSlider.value = ClientSettings.BroadcastPingRate;
		settings.pingValue.text = ClientSettings.BroadcastPingRate.ToString() + "s";

		// MANUAL IP SETTINGS
		settings.manualServers.text = ClientSettings.ManualIP;
		SetManualServers();

		// START RECEIVING
		receiveThread = new Thread(
			new ThreadStart(ReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start();

		// Broadcast invitation
		pingTimer = ClientSettings.BroadcastPingRate;
	}

	void Update(){

		// CHECK IF ANY NEW ADDRESSES
		if (addresses.Count > servers.Count)
			addServerButton();

		// IF USER WANTS TO BROADCAST INVITATION:
		if (ClientSettings.BroadcastInvitation){
			if (pingTimer >= ClientSettings.BroadcastPingRate){
				Invite();
				pingTimer = 0f;
			}else{
				pingTimer += Time.deltaTime;
			}
		}
		
	}

	/* CLEANUP */
	void OnDisable(){ 
		if ( receiveThread!= null) {
			runThread = false;
			receiveThread.Abort ();
		}
		bcastClient.Close();
	} 

	/* PARSE AND ADD MANUAL SERVERS */
	public void AddManualServers(){
		ClientSettings.ManualIP = settings.manualServers.text;
		char[] delimiterChars = { ':' };
		string[] addressLines = ClientSettings.ManualIP.Split ('\n');
		foreach(string line in addressLines){
			string[] temp = line.Split(delimiterChars);
			if (temp.Length > 1){
				temp = new string[]{temp.Length == 3 ? temp[2] : temp[0], temp[1], temp[0]};
				ServerSettings address = new ServerSettings(temp);
				address.wasManuallyAdded = true;
				addresses.Add (address);
				addServerButton();
			}
		}
	}

	/* ADD NEW SERVERS TO LIST */
	public void addServerButton(){
		foreach(ServerSettings address in addresses){
			if (!servers.Exists(x => x.IP == address.IP) && !address.IP.Contains(ClientSettings.ClientIP))
			{
				servers.Add(address);
				GameObject button = Instantiate(buttonTemplate);
				button.SetActive(true);
				button.transform.SetParent(ServerList.transform, false);
				button.GetComponent<LobbyServerButton>().setProperties(address);
			}
		}
	}

	/* IF AUTO-CONNECT TOGGLE SELECTED */
	public void setAutoConnect(bool isActive){
		ToggleGroup tGroup = ServerList.GetComponent<ToggleGroup>();
		Toggle activeToggle = tGroup.GetActive();
		if (isActive || activeToggle == null){
			if (activeToggle){
				LobbyServerButton autoConn = activeToggle.GetComponentInParent<LobbyServerButton>();
				PlayerPrefs.SetString("autoIP", autoConn.config.IP);
				PlayerPrefs.SetInt("autoPort", autoConn.config.portNumber);
				PlayerPrefs.SetString("autoHost", autoConn.config.hostName);
				PlayerPrefs.Save ();
			}else{
				PlayerPrefs.DeleteKey("autoIP");
				PlayerPrefs.DeleteKey("autoPort");
				PlayerPrefs.DeleteKey("autoHost");
				PlayerPrefs.Save ();
			}
		}

	}

	/* BROADCAST ENCRYPTED INVITATION */
	public void Invite() 
	{
		Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
		                      ProtocolType.Udp);
		s.EnableBroadcast = true;
		IPAddress broadcast = IPAddress.Parse(ClientSettings.BroadcastIP);
		IPEndPoint endPoint = new IPEndPoint(broadcast, ClientSettings.BroadcastPort);

		string message = ClientSettings.SecretKey + ClientSettings.ClientIP;
		string encrypted = CipherUtility.Encrypt<AesManaged>(message, ClientSettings.SecretKey , ClientSettings.Salt);
		byte[] sendbuf = Encoding.ASCII.GetBytes(encrypted);

		s.SendTo(sendbuf, endPoint);
	}

	/* ASYNCHRONOUS RECEIVE THREAD */
	private  void ReceiveData()
	{	
		bcastClient = new UdpClient(ClientSettings.BroadcastPort);
		while (runThread)
		{
			try
			{
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);

				byte[] data = bcastClient.Receive(ref anyIP);
				string senderIP = anyIP.ToString().Split (':')[0];

				if (!addresses.Exists(x => x.IP == senderIP) && !senderIP.StartsWith(ClientSettings.ClientIP)){
					string text = Encoding.UTF8.GetString(data);
					string decrypted = CipherUtility.Decrypt<AesManaged>(text, ClientSettings.SecretKey, ClientSettings.Salt);

					Hashtable hash = JsonReader.Deserialize<Hashtable>(decrypted);

					//make sure response contains secret key
					if (hash.ContainsKey("response")){
						if (hash["response"].ToString() == ClientSettings.SecretKey){

							ServerSettings address = new ServerSettings();
							address.hostName = hash["host_name"].ToString();
							address.portNumber = (int)hash["server_port"];
							// need to find way to get ip address
							// on server side, this is not secure
							address.IP = anyIP.ToString().Split (':')[0];
							addresses.Add (address);

							ClientSettings.disableEncryption = Convert.ToBoolean(hash["disable_encrypt"].ToString());
						}
					}
				}
			}
			catch (Exception err)
			{
				print(err.ToString());
			}
		}


	}

	/// <summary>
	/// method registered to PlayerActionInterpreter.cs
	/// close menu if open and escape key pressed
	/// </summary>
	public void CloseMenu(){
		if (settings.menuToggle.isOn){
			PlayerActionInterpreter.menuIsOpen = true;
			settings.menuToggle.isOn = false;
		}
	}

	/* FUNCTIONS USED BY SETTINGS MENU */
	public void SetBroadcastInvitation(bool value){
		ClientSettings.BroadcastInvitation = value;
	}
	
	public void SetBroadcastAddress(string addr){
		ClientSettings.BroadcastAddress = addr;
	}
	
	public void setSecretKey(string newKey){
		ClientSettings.SecretKey = newKey;
	}
	
	public void ResetSecretKey(){
		ClientSettings.SecretKey = ClientSettings.DefaultKey;
	}
	
	public void setPingRate(float value){
		ClientSettings.BroadcastPingRate = value;
		settings.pingValue.text = value.ToString() + "s";
		if (ClientSettings.BroadcastPingRate != settings.pingSlider.value)
			settings.pingSlider.value = ClientSettings.BroadcastPingRate;
	}
	
	public void SetManualServers(){
		LobbyServerButton[] buttons = ServerList.GetComponentsInChildren<LobbyServerButton>();
		foreach(LobbyServerButton b in buttons){
			if (b.config.wasManuallyAdded){
				addresses.Remove (b.config);
				servers.Remove(b.config);
				Destroy(b.gameObject);
				
			}
		}
		AddManualServers();
	}
}
