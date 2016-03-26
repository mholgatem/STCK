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


public class Invite 
{
	public static void Main(string args) 
	{
		Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
		                      ProtocolType.Udp);
		s.EnableBroadcast = true;
		IPAddress broadcast = IPAddress.Parse("255.255.255.255");

		string encrypted = CipherUtility.Encrypt<AesManaged>(args, "password", "testtest");
		byte[] sendbuf = Encoding.ASCII.GetBytes(encrypted);

		IPEndPoint ep = new IPEndPoint(broadcast, 55535);


		s.SendTo(sendbuf, ep);

	}
}

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

public class udp_broadcast : MonoBehaviour {
	
	// receiving Thread
	Thread receiveThread;
	UdpClient client;
	
	// public
	[Tooltip("Seconds between broadcast pings")]
	[Range(0f, 100f)] public float pingRate = 10f;
	public int port; // define > init
	public Text overlayText;

	
	public GameObject buttonTemplate;

	[Header("Settings Menu Objects")]
	public settingObjects settingObj;
	[HideInInspector] public List<string[]> servers = new List<string[]>();

	private List<string[]> addresses = new List<string[]>();
	private float pingTimer;
	private GameObject ServerList;
	private string secretKey, defaultKey = "*4kap),dci30dm?";
	


	void Awake () {
		QualitySettings.vSyncCount = 0;  // VSync must be disabled
		Application.targetFrameRate = 30;
	}

	public void Start(){

		//Get and set any saved settings
		if (PlayerPrefs.HasKey("uid"))
			setSecretKey(PlayerPrefs.GetString("uid"));
		
		if (PlayerPrefs.HasKey("pingRate"))
			setPingRate(PlayerPrefs.GetFloat("pingRate"));

		//if (PlayerPrefs.HasKey("manualIP"))
			//settingObj.manualServers.text = PlayerPrefs.GetString("manualIP");

		//Find server list
		ServerList = GameObject.Find("ServerList/Scroll View/Viewport/Content");

		setManualServer();

		receiveThread = new Thread(
			new ThreadStart(ReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start();

		Invite.Main(secretKey + GetLocalIP());


	}

	void Update(){

		if (addresses.Count > servers.Count)
			addServerButton();

		if (pingTimer >= pingRate){
			Invite.Main(secretKey + GetLocalIP());
			pingTimer = 0f;
		}else{
			pingTimer += Time.deltaTime;
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (settingObj.settingMenu.activeInHierarchy)
				settingObj.menuToggle.isOn = false;
			else
				Application.Quit();
		}
	}


	string GetLocalIP(){
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

	public void SetBroadcast(string broadcastIP){

	}

	public void setSecretKey(string newKey){
		secretKey = newKey;
		if (secretKey != settingObj.keyField.text)
			settingObj.keyField.text = secretKey;
		PlayerPrefs.SetString("uid", secretKey);
		PlayerPrefs.Save ();
	}

	public void ResetSecretKey(){
		secretKey = defaultKey;
		settingObj.keyField.text = defaultKey;
		PlayerPrefs.SetString("uid", defaultKey);
		PlayerPrefs.Save ();
	}

	public void setPingRate(float value){
		pingRate = value;
		settingObj.pingValue.text = value.ToString() + "s";
		if (pingRate != settingObj.pingSlider.value)
			settingObj.pingSlider.value = pingRate;
		PlayerPrefs.SetFloat("pingRate", pingRate);
		PlayerPrefs.Save ();
	}

	public void setManualServer(){
		char[] delimiterChars = { '{', '}',':' };

		string[] address = settingObj.manualServers.text.Split(delimiterChars);
		if (address.Length > 1){
			address = new string[]{address.Length == 3 ? address[0] : address[2], address[1], address[0]};
			addresses.Add (address);
			//addresses.Add (settingObj.manualServers.text.Split(delimiterChars));
			addServerButton();
		}
		PlayerPrefs.SetString("manualIP", settingObj.manualServers.text);
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

	// receive thread
	private  void ReceiveData()
	{
		client = new UdpClient(port);
		while (true)
		{
			
			try
			{
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = client.Receive(ref anyIP);
				string text = Encoding.UTF8.GetString(data);

				char[] delimiterChars = { '{', '}',':' };


				//make sure server sends secret key
				string decrypted = CipherUtility.Decrypt<AesManaged>(text, "password", "testtest");
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


	void OnDisable(){ 
		if ( receiveThread!= null) 
			receiveThread.Abort(); 
		
		client.Close(); 
	} 
}
