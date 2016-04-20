using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
	float deltaTime = 0.0f;
	public Color fontColor = new Color (0.0f, 0.0f, 0.5f, 1.0f);
	int w = Screen.width, h = Screen.height;
	GUIStyle style = new GUIStyle();
	Rect rect;


	void Start(){
		rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = fontColor;
	}


	void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}

	string GameFPS(){
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		return text;
	}

	string ScreenshotFPS(){
		return "";
	}

	void OnGUI()
	{

		string text = GameFPS();
		GUI.Label(rect, text, style);
	}
}