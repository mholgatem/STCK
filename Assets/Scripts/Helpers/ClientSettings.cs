using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System;
using System.Net;
using System.Net.Sockets;



public static class ClientSettings {

	private static string secretKey, defaultKey = "*4kap),dci30dm?";
	private static string salt = "a$fk^fkj69)-YU"; //DO NOT CHANGE UNLESS YOU CHANGE SERVER CODE AS WELL!
	
	private static string broadcastIP, defaultBroadcastIP = "255.255.255.255";
	private static int broadcastPort, defaultBroadcastPort = 55535;
	private static float broadcastPingRate = 10f;

	private static string manualIP = "";
	private static string clientIP = "";

	private static bool broadcastInvitation;

	public static bool disableEncryption = false;
	
	// constructor to initialize settings
	static ClientSettings(){
		// SAVED SETTINGS

		// ADDRESS:PORT TO BROADCAST INVITATIONS
		if (PlayerPrefs.HasKey("broadcastAddress"))
			BroadcastAddress = PlayerPrefs.GetString("broadcastAddress");
		else
			BroadcastAddress = defaultBroadcastIP + ":" + defaultBroadcastPort.ToString();
		
		// SECRET KEY - named 'uid' for obfuscation
		if (PlayerPrefs.HasKey("uid"))
			secretKey = CipherUtility.Decrypt<AesManaged>(PlayerPrefs.GetString("uid"), defaultKey + salt, salt);
		else
			SecretKey = defaultKey;

		// RATE AT WHICH TO SEND INVITATIONS TO SERVERS
		if (PlayerPrefs.HasKey("pingRate"))
			broadcastPingRate = PlayerPrefs.GetFloat("pingRate");

		// STRING LIST OF USER ENTERED SERVERS
		if (PlayerPrefs.HasKey("manualIP"))
			manualIP = PlayerPrefs.GetString("manualIP");

		// BOOL - SHOULD CLIENT BROADCAST INVITATION?
		if (PlayerPrefs.HasKey("broadcastInvitation"))
		    broadcastInvitation = Convert.ToBoolean(PlayerPrefs.GetInt("broadcastInvitation"));
		else
			BroadcastInvitation = true;

	}

	public static string SecretKey{
		get{ return secretKey; }
		set{ 
			if (string.IsNullOrEmpty(value))
				value = defaultKey;
			secretKey = value;
			string encryptedKey = CipherUtility.Encrypt<AesManaged>(secretKey, defaultKey + salt, salt);
			PlayerPrefs.SetString("uid", encryptedKey);
			PlayerPrefs.Save ();
		}
	}

	public static string DefaultKey{
		get{ return defaultKey; }
	}

	/// <summary>
	/// Sets the broadcast address.
	/// </summary>
	/// <value>The broadcast address.(format: #.#.#.#:port)</value>
	public static string BroadcastAddress{
		get{ return broadcastIP + ":" + broadcastPort; }
		set{
			if (value.IndexOf(":") > 6){
				broadcastIP = value.Split(':')[0];
				broadcastPort = Int32.Parse(value.Split(':')[1]);
			}else{
				broadcastIP = defaultBroadcastIP;
				broadcastPort = defaultBroadcastPort;
			}
			PlayerPrefs.SetString("broadcastAddress", broadcastIP + ":" + broadcastPort.ToString() );
			PlayerPrefs.Save ();
		}
	}

	public static bool BroadcastInvitation{
		get{ return broadcastInvitation; }
		set{
			broadcastInvitation = value;
			PlayerPrefs.SetInt("broadcastInvitation", Convert.ToInt32(value));
			PlayerPrefs.Save ();
		}
	}

	public static string Salt{
		get{ return salt; }
	}

	public static string BroadcastIP{
		get{ return broadcastIP; }
	}

	public static int BroadcastPort{
		get{ return broadcastPort; }
	}

	public static float BroadcastPingRate{
		get{ return broadcastPingRate; }
		set{
			broadcastPingRate = value;
			PlayerPrefs.SetFloat("pingRate", broadcastPingRate );
			PlayerPrefs.Save ();
		}
	}

	public static string ManualIP{
		get{ return manualIP; }
		set{
			manualIP = value;
			PlayerPrefs.SetString("manualIP", manualIP );
			PlayerPrefs.Save ();
		}
	}

	public static string ClientIP{
		get{
			if (clientIP.Length > 0)
				return clientIP;
			IPHostEntry host;
			host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (IPAddress ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					clientIP = ip.ToString();
				}
			}
			return clientIP;
		}
		
	}
}
