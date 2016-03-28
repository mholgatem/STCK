using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class KeyboardHandler : MonoBehaviour {

	public Client client;
	public InputField inputField;
	public Text textField;
	public bool capsLock = false;
	Dictionary<char, string> dictionary =
			new Dictionary<char, string>(){
				{"\b"[0], "BACKSPACE"},
				{"\n"[0], "ENTER"},
				{"\r"[0], "ENTER"},
				{'\t', "TAB"}
			};

	// Use this for initialization
	void Start () {
		client = GameObject.FindWithTag("Client").GetComponent<Client>();
		inputField = GetComponent<InputField>();
		TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, true);
	}

	void Update(){
		string key;
		foreach (char c in Input.inputString){
			if (!dictionary.TryGetValue(c, out key)){
				key = ((KeyCode)c).ToString();

			}
			Debug.Log(((KeyCode)c).ToString());
			Debug.Log ((KeyCode)(97));
			if ((int)c >= 65 && (int)c <= 90){
				capsLock = true;
				key = ((KeyCode)((int)c + 32)).ToString();
			}else{
				capsLock = false;
			}
			sendKey (key);
		}
	}


	public void sendKey(string key){
		string command = "KEY_" + key.ToUpper();
		if (capsLock == true)
			client.SendButtonState("KEY_LEFTSHIFT", "EV_KEY", 1);
		client.SendButtonState(command, "EV_KEY", 1);
		client.SendButtonState(command, "EV_KEY", 0);
		if (capsLock == true)
			client.SendButtonState("KEY_LEFTSHIFT", "EV_KEY", 0);
	}
}
