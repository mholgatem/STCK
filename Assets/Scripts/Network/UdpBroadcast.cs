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
	UdpClient client;
	
	// public
	[Tooltip("Seconds between broadcast pings")]
	[Range(0f, 100f)] public float pingRate = 10f;
	public Text overlayText;


	public GameObject buttonTemplate;

	[Header("Settings Menu Objects")]
	public settingObjects settings;
	[HideInInspector] public List<string[]> servers = new List<string[]>();

	private List<string[]> addresses = new List<string[]>();
	private float pingTimer;
	private GameObject ServerList;
	private string secretKey, defaultKey = "*4kap),dci30dm?";
	private string salt = "a$fk^fkj69)-YU";

	private string defaultBroadcastAddress = "255.255.255.255:55535";
	private string broadcastIP = "255.255.255.255";
	private int broadcastPort = 55535;
	


	void Awake () {
		//force application to framerate of 30 to avoid
		//battery consumption
		QualitySettings.vSyncCount = 0;  // VSync must be disabled
		Application.targetFrameRate = 30;
	}

	public void Start(){

		// SAVED SETTINGS
		if (PlayerPrefs.HasKey("broadcastAddress"))
			SetBroadcast(PlayerPrefs.GetString("broadcastAddress"));

		// secretKey
		if (PlayerPrefs.HasKey("uid"))
			setSecretKey(PlayerPrefs.GetString("uid"));
		
		if (PlayerPrefs.HasKey("pingRate"))
			setPingRate(PlayerPrefs.GetFloat("pingRate"));

		if (PlayerPrefs.HasKey("manualIP"))
			settings.manualServers.text = PlayerPrefs.GetString("manualIP");

		//Find server list
		ServerList = GameObject.Find("ServerList/Scroll View/Viewport/Content");

		setManualServer();

		receiveThread = new Thread(
			new ThreadStart(ReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start();
		
		Invite(secretKey + GetLocalIP());
	}

	void Update(){

		if (addresses.Count > servers.Count)
			addServerButton();

		if (pingTimer >= pingRate){
			Invite(secretKey + GetLocalIP());
			pingTimer = 0f;
		}else{
			pingTimer += Time.deltaTime;
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (settings.settingMenu.activeInHierarchy)
				settings.menuToggle.isOn = false;
			else
				Application.Quit();
		}
	}


	private string GetLocalIP(){
		IPHostEntry host;
		string localIP = "?";
		host = Dns.GetHostEntry(Dns.GetHostName());
		foreach (IPAddress ip in host.AddressList)
		{
			if (ip.AddressFamily == AddressFamily.InterNetwork)
			{
				localIP = ip.ToString();
			}
		}
		return localIP;

	}

	public void SetBroadcast(string addr){
		string[] temp = addr.Split (':');
		// set ip = addr or default
		if (!string.IsNullOrEmpty(temp[0])){
			broadcastIP = temp[0];
		}else{
			broadcastIP = defaultBroadcastAddress.Split (':')[0];
		}
		// set port = addr or default
		if (temp.Length > 1){
			broadcastPort = Int32.Parse(temp[1]);
		}else{
			broadcastPort = Int32.Parse(defaultBroadcastAddress.Split (':')[1]);
		}
		// save settings
		settings.broadcastIP.text = string.Format("{0}:{1}",broadcastIP,broadcastPort);
		PlayerPrefs.SetString("broadcastAddress", string.Format("{0}:{1}",broadcastIP,broadcastPort));
		PlayerPrefs.Save ();
	}

	public void setSecretKey(string newKey){
		//verify valid key
		if (string.IsNullOrEmpty(newKey))
			newKey = defaultKey;

		//set secret key accordingly
		secretKey = newKey;
		if (secretKey != settings.keyField.text)
			settings.keyField.text = secretKey;
		PlayerPrefs.SetString("uid", secretKey);
		PlayerPrefs.Save ();
	}

	public void ResetSecretKey(){
		secretKey = defaultKey;
		settings.keyField.text = defaultKey;
		PlayerPrefs.SetString("uid", defaultKey);
		PlayerPrefs.Save ();
	}

	public void setPingRate(float value){
		pingRate = value;
		settings.pingValue.text = value.ToString() + "s";
		if (pingRate != settings.pingSlider.value)
			settings.pingSlider.value = pingRate;
		PlayerPrefs.SetFloat("pingRate", pingRate);
		PlayerPrefs.Save ();
	}

	public void setManualServer(){
		char[] delimiterChars = { '{', '}',':' };
		string[] addressLines = settings.manualServers.text.Split ('\n');
		foreach(string line in addressLines){
			string[] address = line.Split(delimiterChars);
			if (address.Length > 1){
				address = new string[]{address.Length == 3 ? address[2] : address[0], address[1], address[0]};
				addresses.Add (address);
				//addresses.Add (settings.manualServers.text.Split(delimiterChars));
				addServerButton();
			}
		}
		PlayerPrefs.SetString("manualIP", settings.manualServers.text);
		PlayerPrefs.Save ();
	}

	public void addServerButton(){
		foreach(string[] address in addresses){
			if (!servers.Exists(x => x[0] == address[0]) && !address[0].Contains(GetLocalIP()))
			{
				servers.Add(address);
				GameObject button = Instantiate(buttonTemplate);
				button.SetActive(true);
				button.transform.SetParent(ServerList.transform, false);
				button.GetComponent<LobbyServerButton>().setProperties(address);
			}
		}
		addresses.Clear();
	}

	public void removeServerButton(){
		LobbyServerButton[] buttons = ServerList.GetComponentsInChildren<LobbyServerButton>();
		foreach(LobbyServerButton b in buttons){
			if (b.hostName == PlayerPrefs.GetString("manualIP") &&
			    b.port.ToString() == PlayerPrefs.GetString("manualPort")){
				Destroy(b.gameObject);
				setManualServer();
			}
		}
	}

	public void setAutoConnect(bool isActive){
		ToggleGroup tGroup = ServerList.GetComponent<ToggleGroup>();
		Toggle activeToggle = tGroup.GetActive();
		if (isActive || activeToggle == null){
			if (activeToggle){
				LobbyServerButton autoConn = activeToggle.GetComponentInParent<LobbyServerButton>();
				PlayerPrefs.SetString("autoIP", autoConn.ip);
				PlayerPrefs.SetInt("autoPort", autoConn.port);
				PlayerPrefs.SetString("autoHost", autoConn.hostName);
				PlayerPrefs.Save ();
			}else{
				PlayerPrefs.DeleteKey("autoIP");
				PlayerPrefs.DeleteKey("autoPort");
				PlayerPrefs.DeleteKey("autoHost");
				PlayerPrefs.Save ();
			}
		}

	}

	//broadcast encrypted message
	public void Invite(string message) 
	{
		Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
		                      ProtocolType.Udp);
		s.EnableBroadcast = true;
		IPAddress broadcast = IPAddress.Parse(broadcastIP);
		
		string encrypted = CipherUtility.Encrypt<AesManaged>(message, secretKey , salt);
		byte[] sendbuf = Encoding.ASCII.GetBytes(encrypted);
		
		IPEndPoint ep = new IPEndPoint(broadcast, broadcastPort);
		
		
		s.SendTo(sendbuf, ep);
	}

	// receive thread
	private  void ReceiveData()
	{
		client = new UdpClient(broadcastPort);
		while (true)
		{
			
			try
			{
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = client.Receive(ref anyIP);
				string text = Encoding.UTF8.GetString(data);


				char[] delimiterChars = { '{', '}',':' };

				//make sure server sends secret key
				string decrypted = CipherUtility.Decrypt<AesManaged>(text, secretKey, salt);
				decrypted += anyIP.ToString();

				if (decrypted.Contains(secretKey)){
					decrypted = decrypted.Replace(secretKey, "");
					addresses.Add (decrypted.Split(delimiterChars));
				}
				
			}
			catch (Exception err)
			{
				print(err.ToString());
			}
		}
	}

	// cleanup
	void OnDisable(){ 
		if ( receiveThread!= null) 
			receiveThread.Abort(); 
		
		client.Close(); 
	} 
}
