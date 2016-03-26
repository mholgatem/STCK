using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class LobbyServerButton : MonoBehaviour {

	public string ip;
	public string hostName;
	public int port;
	public bool isManualServer = false;
	
	public void setProperties(string[] server){
		hostName = server[0]; 
		port = Convert.ToInt32(server[1]);
		ip = server[2]; 
		GetComponentInChildren<Text>().text = hostName;
		if (PlayerPrefs.HasKey("autoIP")){
			if (PlayerPrefs.GetString("autoIP") == ip &&
			    PlayerPrefs.GetInt("autoPort") == port &&
			    PlayerPrefs.GetString("autoHost") == hostName){
			    	if (Time.realtimeSinceStartup < 3f)
			    		MakeConnection();
					else
						GetComponentInChildren<Toggle>().isOn = true;
				}
		}
	}

	public void MakeConnection(){
		PersistentVar.hostName = hostName; 
		PersistentVar.port = port;
		PersistentVar.ip = ip; 
		Application.LoadLevelAsync("Client");
	}
	
}
