using UnityEngine;
using System.Collections;

[System.Serializable]
public class dpadCommand : System.Object
{
	public string up;
	public string down;
	public string left;
	public string right;
	
}

[System.Serializable]
public class keyboardLayout : System.Object
{
	public Vector2 anchorMin;
	public Vector2 anchorMax;

	public keyboardLayout(){}

	public keyboardLayout(RectTransform r){
		anchorMin = r.anchorMin;
		anchorMax = r.anchorMax;
	}
}

public class propertyContainer : MonoBehaviour {

	public string command;
	public RectTransform keyboard;
	public dpadCommand dpad;

}
