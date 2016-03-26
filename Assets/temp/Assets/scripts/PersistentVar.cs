
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class PersistentVar : MonoBehaviour {

	public PersistentVar currentID;

	public static string ip="";
	public static string hostName="";
	public static int port=55536;
	public static List<string> failedHost = new List<string>();
	public static PersistentVar use = null;

	void Awake () {
		if (currentID == null){
			DontDestroyOnLoad(gameObject);
			currentID = this;
		}
		else if (currentID != this){
			Destroy(gameObject);
		}
	}

}
