using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class LobbyServerButton : MonoBehaviour {

	public ServerSettings config = new ServerSettings();
	
	public void setProperties(ServerSettings server){
		config = server;

		GetComponentInChildren<Text>().text = config.hostName;
		if (PlayerPrefs.HasKey("autoIP")){
			if (PlayerPrefs.GetString("autoIP") == config.IP &&
			    PlayerPrefs.GetInt("autoPort") == config.portNumber &&
			    PlayerPrefs.GetString("autoHost") == config.hostName){
			    	if (Time.realtimeSinceStartup < 3f)
			    		MakeConnection();
					else
						GetComponentInChildren<Toggle>().isOn = true;
				}
		}
	}

	public void MakeConnection(){
		Client.serverHostName = config.hostName;
		Client.serverIP = config.IP;
		Client.serverPort = config.portNumber;
		Client.currentInstance.gameObject.SetActive(true); 
		Client.secretKey = ClientSettings.SecretKey;
		Client.salt = ClientSettings.Salt;
	}
	
}
