using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.ImageEffects;
using UnityStandardAssets;

public class GUI_Overlay : MonoBehaviour {

	
	public GUIText overlayText;
	//public GameObject userGO;
	public GUISkin customSkin;


	private List<GameObject> usersGO = new List<GameObject>();
	private KinUser[] users;

	void Start(){

	}

	void OnGUI (){

		/*if (menuOpen)
		{

			settingsMenu.enabled = true;
			GUI.Box (menuBox, "Settings");

			if (GUI.Button(new Rect(Screen.width - 390, 110, 380, 50), "Customize Controls"))
			{
				Debug.Log ("customize");
				menuOpen = false;
			}
			if (GUI.Button(new Rect(Screen.width - 390, 170, 380, 50), "arcade"))
			{
				Debug.Log ("arcade");
				Application.LoadLevel("arcade-8button");
				menuOpen = false;
			}
			if (GUI.Button(new Rect(Screen.width - 390, 230, 380, 50), "retro"))
			{
				Debug.Log ("retro");
				menuOpen = false;
			}
			if (GUI.Button(new Rect(Screen.width - 390, 290, 380, 50), "media"))
			{
				Debug.Log ("media");
				menuOpen = false;
			}
			if (GUI.Button(new Rect(Screen.width - 390, 350, 380, 50), "keyboard"))
			{
				Debug.Log ("keyboard");
				keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
				menuOpen = false;
			}

			if (Application.loadedLevelName != "Client"){



				GUI.Box (new Rect(Screen.width - 400, 410, 400, 80), "FX: " + graphics_setting);
				graphicSliderValue = Mathf.Round(GUI.HorizontalSlider(new Rect(Screen.width - 390, 450, 380, 60), graphicSliderValue, 0.0f, 2f));

				if(graphicLevel != graphicSliderValue)
					setGraphicState(graphicSliderValue);

			}

			if (GUI.Button(new Rect(Screen.width - 390, 590, 380, 50), "Quit"))
			{
				Application.Quit ();
			}

		}*/
	}


	void Update () {


		
		//axis.x = Mathf.Round (CrossPlatformInputManager.GetAxis("joystick-H") * 100) / 100f;
		//axis.y = Mathf.Round (CrossPlatformInputManager.GetAxis("joystick-V") * 100) / 100f;

		/*
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			//if (!menuOpen)
			//{Application.LoadLevel("Lobby");}
			//else
			//{menuOpen = false;}
		}

		if (Input.GetKeyDown (KeyCode.Return)) {
			XTionControl.use.Connect();
		}
		else if (Input.GetKeyDown (KeyCode.Space)) {
			XTionControl.use.GetUserNumber();
		}
		else if (Input.GetKeyDown ("u")) {
			XTionControl.use.GetUsers();
		}
		else if (Input.GetKeyDown ("q")) {
			XTionControl.use.Disconnect();
		}
		else if (Input.GetKeyDown ("s")) {
			if(XTionControl.use.xtionClient.isConnectedToServer()) {
				XTionControl.use.Shutdown();
			}
		}
		*/
		
		//overlayText.text = XTionControl.use.GetLastServerMessage();
		
	}
	
	void OnConnected( bool isConnected ) {
		Debug.Log("OnConnected: " + isConnected );
	}
	
	void OnUserNumber( int number ) {
		Debug.Log("OnUserNumber: " + number );
	}
	
	void OnUsers( KinUser[] users ) {
		Debug.Log("OnUsers: " + users.Length );
		this.users = users;
	}
	
	/*void RefreshUsersGO() {
		
		if(users == null || users.Length <= 0) {
			foreach(GameObject go in usersGO)
				GameObject.Destroy(go);
			usersGO = new List<GameObject>();	
		}
		else {
			
			List<GameObject> list = new List<GameObject>();
			
			// remove user no more in the scene
			foreach(GameObject go in usersGO) {
				UserGO script = go.GetComponent<UserGO>();
				bool found = false;
				
				foreach(KinUser ku in users) {
					if(script.CompareUser(ku)) {
						list.Add(go);
						found = true;
						break;
					}
				}
				if(!found)
					GameObject.Destroy(go);
			}
			
			// update existing users and add new users.
			foreach(KinUser ku in users) {
				
				bool isNewUser = true;
				
				foreach(GameObject go in list) {
					UserGO script = go.GetComponent<UserGO>();
					if(script.CompareUser(ku)) {
						script.UpdatePosition(ku);
						isNewUser = false;
						break;
					}
				}
				
				if(isNewUser) {
					GameObject newGO = GameObject.Instantiate(userGO) as GameObject;
					UserGO script = newGO.GetComponent<UserGO>();
					script.User = ku;
					list.Add(newGO);	
				}
				
			}
			
			usersGO = list;
			
		}
				
	}*/
}
