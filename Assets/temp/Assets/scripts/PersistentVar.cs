
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class PersistentVar : MonoBehaviour {

	public static PersistentVar currentInstance;


	//public static PersistentVar use = null;

	void Awake () {
		// SINGLETON
		if( currentInstance == null ) {
			currentInstance = this;
			DontDestroyOnLoad(this.gameObject);
		}else{
			Destroy( this.gameObject );
		}
	}

}
