using UnityEngine;
using System.Collections;

public class Screenshot : MonoBehaviour {
	
	private CanvasRenderer imageTexture;
	private float startTime = 0f;
	public float fps = 0;
	public float otherfps = 0;
	private float lastTimer = 0;
	public float timer = 0;
	public int imageCount = 0;

	public void OnEnable(){
		imageTexture = GetComponent<CanvasRenderer>();
	}

	public void LoadImage (byte[] image) {
		// Create a texture. Texture size does not matter, since
		// LoadImage will replace it with incoming image size.
		var tex = new Texture2D(320, 240);
		imageCount++;
		if (startTime == 0f)
			startTime = Time.time;

		fps = imageCount / (Time.time - startTime);
		timer = Time.time - lastTimer;
		lastTimer = Time.time;
		otherfps = 1 / timer;
		// Load data into the texture.
		tex.LoadImage(image);

		// Assign texture to renderer's material.
		imageTexture.SetTexture(tex);

	}
}
