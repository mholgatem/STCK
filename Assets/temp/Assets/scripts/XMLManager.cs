using UnityEngine; 
using UnityEngine.UI;
using System.Collections; 
using System.Collections.Generic;
using System.Xml; 
using System.Xml.Serialization; 
using System.IO; 
using System.Text;
using UnityStandardAssets.CrossPlatformInput;


[XmlRoot("data")]
public class DataContainer
{

	[XmlElement("screen")]
	public screenItem screen { get; set; }
	[XmlArray("gameObjects")]
	[XmlArrayItem("gameObject")]
	public xmlItem[] items { get; set; }
}

public class screenItem
{
	public ScreenOrientation orientation;
	public Vector2 size;

	public screenItem(){
		if (Screen.width > Screen.height)
			orientation = ScreenOrientation.Landscape;
		else
			orientation = ScreenOrientation.Portrait;
		size = new Vector2(Screen.width, Screen.height);
	}
	
}
public class xmlItem
{
	public string name;
	public Vector3 anchoredPosition;
	public Vector2 sizeDelta;
	public Vector2 anchors;
	public Vector2 pivot;
	public Vector3 scale;
	public Color objColor;
	public string objText = "";
	public string objImage = "";
	public string objBackground = "";
	public string objCommand = "";
	public dpadCommand objDpad;
	public keyboardLayout layout;

	
	public xmlItem(){}
	
	public xmlItem(GameObject obj)
	{
		name = obj.tag;
		RectTransform rect = obj.GetComponent<RectTransform>();
		anchoredPosition = rect.anchoredPosition3D;
		sizeDelta = rect.sizeDelta;
		anchors = rect.anchorMax;
		pivot = rect.pivot;
		scale = obj.transform.localScale;
		objColor = getColor(obj);

		if (obj.GetComponentInChildren<Text>())
			objText = obj.GetComponentInChildren<Text>().text;

		if (obj.tag == "UI_Button")
		{
			objImage = getIcon(obj,"Button_hitbox");
			objBackground = getIcon(obj,"Image");
		}

		if (obj.tag == "Keyboard")
		{
			layout = new keyboardLayout(obj.GetComponent<propertyContainer>().keyboard);
		}

		if (obj.GetComponent<propertyContainer>() && obj.tag != "Keyboard")
		{
			objCommand = obj.GetComponent<propertyContainer>().command;
			objDpad = obj.GetComponent<propertyContainer>().dpad;
		}
	}

	public Color getColor(GameObject obj){
		
		Renderer[] controlChildren = getMaterialObject (obj);
		foreach(Renderer child in controlChildren)
		{
			return child.material.color;
		}
		if (obj.tag == "UI_Button")
		{
			Image[] icons = obj.transform.GetComponentsInChildren<Image>();
			foreach(Image icon in icons)
			{
				switch (icon.name)
				{
				case "Button_hitbox":
					return icon.color;
				default:
					break;
				}
			}
		}
		return Color.white;
	}

	public void setColor(GameObject obj, Color currentColor){


		Renderer[] controlChildren = getMaterialObject (obj);
		foreach(Renderer child in controlChildren)
		{
			child.material.color = currentColor;
		}
		Image[] hitBoxes = obj.transform.GetComponentsInChildren<Image>();
		foreach(Image hitBox in hitBoxes)
		{
			if (hitBox.name == "hitbox" || hitBox.name == "Button_hitbox")
				hitBox.color = currentColor;
		}
		if (obj.tag == "UI_Button")
		{
			Image[] icons = obj.transform.GetComponentsInChildren<Image>();
			foreach(Image icon in icons)
			{
				switch (icon.name)
				{
				case "Button_hitbox":
					icon.color = currentColor;
					break;
				default:
					icon.color = Color.white;
					break;
				}
			}
		}
	
	}
	
	public string getIcon(GameObject obj, string type){


		Image[] icons = obj.transform.GetComponentsInChildren<Image>();
		foreach(Image icon in icons)
		{

			if (icon.name == type)
				return "sprites/" + icon.sprite.name.Split ('-')[0] + "/" + icon.sprite.name;
				
		}
		return "sprites/" + icons[0].sprite.name.Split ('-')[0] + "/" + icons[0].sprite.name;
	}

	public Renderer[] getMaterialObject(GameObject control)
	{
		switch (control.tag)
		{
		case "Dpad":
			return control.transform.GetComponentsInChildren<Renderer>();
		case "Button":
			return control.transform.GetComponentsInChildren<Renderer>();
		case "UI_Button":
			return control.transform.GetComponentsInChildren<Renderer>();
		case "Joystick":
			return control.transform.GetChild(0).FindChild("joystick").GetComponents<Renderer>();
		case "Wheel":
			return control.transform.GetComponentsInChildren<Renderer>();
		default:
			return control.transform.GetComponentsInChildren<Renderer>();
		}	
	}
}

[System.Serializable]
public class editorObjects{
	public GameObject joystick;
	public GameObject dpad;
	public GameObject button;
	public GameObject ui_button;
	public GameObject wheel;
	public GameObject tiltcontrol;
	public GameObject keyboard;
}

public class XMLManager: MonoBehaviour { 
	
	// This is our local private members
	 
	public Transform _Canvas; 
	public bool useTestPath = false;
	//public bool showUtilities;
	public Transform layoutPanel;
	public Button saveButton;
	public string saveName;
	public editorObjects editorObj;
	public editorObjects controlPrefabs;
	public List<string> registeredAxis;
	public List<string> registeredButtons;

	
	private string _FileLocation,_FileName;
	private Dictionary<string, Transform> _prefabByName;
	private Dictionary<string, Transform> _editorByName;
	private string _data;
	DataContainer myData;


	void Start () 
	{ 
		// Where we want to save and load to and from 
		_FileLocation=useTestPath ? "C:/Users/MrFancyPantsSR/Desktop/test" : Application.persistentDataPath;
		_FileName="UnNamed.xml"; 
		myData = new DataContainer();
		_prefabByName = new Dictionary<string, Transform>(){
			{"Joystick", controlPrefabs.joystick.transform},
			{"Dpad", controlPrefabs.dpad.transform},
			{"Button", controlPrefabs.button.transform},
			{"UI_Button", controlPrefabs.ui_button.transform},
			{"Wheel", controlPrefabs.wheel.transform},
			{"TiltControl", controlPrefabs.tiltcontrol.transform},
			{"Keyboard", controlPrefabs.keyboard.transform}
		};

		_editorByName = new Dictionary<string, Transform>(){
			{"Joystick", editorObj.joystick.transform},
			{"Dpad", editorObj.dpad.transform},
			{"Button", editorObj.button.transform},
			{"UI_Button", editorObj.ui_button.transform},
			{"Wheel", editorObj.wheel.transform},
			{"TiltControl", controlPrefabs.tiltcontrol.transform},
			{"Keyboard", editorObj.keyboard.transform}
		};
	} 


	public void SetSaveName(string sn)
	{
		saveName = sn;
		if (saveName.Length >= 0)
		{
			saveButton.interactable = true;
		}else{
			saveButton.interactable = false;
		}

	}


	public void SavewithName()
	{
		SaveData (saveName);
	}


	public void SaveData(string fileName)
	{
		_FileName = fileName + ".xml";


		GameObject[] gObjs = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		DataContainer data = new DataContainer();
		data.items = new xmlItem[_Canvas.childCount];


		data.screen = new screenItem();
		int p = 0;
		//iterate items in reverse so that we load them in the order that they were placed
		for (var i = gObjs.Length - 1; i >= 0 ; i--)
		{
			if (gObjs[i].transform.parent == _Canvas &&
			    gObjs[i].transform.name != "LayoutPanel")
			{
				data.items[p] = new xmlItem(gObjs[i]);
				p++;
			}
		}
		XmlSerializer serial = new XmlSerializer(typeof(DataContainer));
		FileStream fs = new FileStream(Path.Combine(_FileLocation,_FileName), FileMode.Create);
		using (TextWriter t = new StreamWriter(fs, new UTF8Encoding()))
		       {
			serial.Serialize(t, data);
		}
		LoadXML(Path.Combine(_FileLocation,_FileName));
	}


	public void ClearCanvas()
	{
		foreach (Transform child in _Canvas) { 
			if (child.name != "LayoutPanel")
			{
				GameObject.Destroy(child.gameObject);
			}
		}

		foreach (Transform child in GameObject.Find ("settingsMenu").transform) { 
			if (child.tag == "Keyboard")
			{
				GameObject.Destroy(child.gameObject);
			}
		}
	}

	public void unregisterAll()
	{
		foreach(string axis in registeredAxis)
		{
			CrossPlatformInputManager.UnRegisterVirtualAxis(axis +"-H");
			CrossPlatformInputManager.UnRegisterVirtualAxis(axis +"-V");
		}

		foreach(string button in registeredButtons)
		{
			CrossPlatformInputManager.UnRegisterVirtualButton(button);
		}

		registeredAxis.Clear ();
		registeredButtons.Clear ();
	}

	public void instantiateDraggable(GameObject obj){

		Transform newObj = Instantiate(obj.transform);
		newObj.rotation = Quaternion.identity;
		newObj.SetParent(transform, false);
		newObj.localScale = Vector3.one * 1.5F;

		newObj.name = newObj.name.Replace("_drag(Clone)","");
		RectTransform newRect = newObj.GetComponent<RectTransform>();

		newRect.anchorMin = Vector2.one / 2;
		newRect.anchorMax = Vector2.one / 2;

	}

	public void LoadXML(string path, bool editCurrent = false) 
	{ 
		StreamReader r = File.OpenText(path); 
		string _info = r.ReadToEnd(); 
		r.Close(); 
		_data=_info; 

		if(_data.Length != 0) 
		{ 
			myData = (DataContainer)DeserializeObject(_data); 
			ClearCanvas();
			unregisterAll();
			int joyCount = 0;
			int buttonCount = 0;
			int wheelCount = 0;
			int tiltCount = 0;
			float relativeScale = new Vector2(Screen.width, Screen.height).magnitude / myData.screen.size.magnitude;
			Debug.Log (relativeScale);

			for (var i = 0; i < myData.items.Length; i++)
			{
				if (myData.items[i].name == "Wheel" && wheelCount > 0)
					continue;
				if (myData.items[i].name == "TiltControl" && tiltCount > 0)
					continue;
				Transform j = Instantiate((editCurrent ? _editorByName[myData.items[i].name] : 
				                           		_prefabByName[myData.items[i].name]));

				//Track registered buttons and axis for cleanup
				if (j.tag == "Joystick")
				{
					j.name = myData.items[i].name + joyCount.ToString();
					if (!editCurrent)
						registeredAxis.Add (j.name);
					joyCount++;
				}
				else if (j.tag == "Wheel")
				{
					j.name = myData.items[i].name;
					if (!editCurrent)
						registeredAxis.Add (j.name);
					wheelCount++;
				}
				else if (j.tag == "TiltControl")
				{
					j.name = myData.items[i].name;
					if (!editCurrent)
						registeredAxis.Add (j.name);
					tiltCount++;
				}
				else if (j.tag == "Keyboard")
				{
					j.name = "Keyboard";
				}
				else
				{
					j.name = myData.items[i].name + buttonCount.ToString();
					if (!editCurrent)
						registeredButtons.Add (j.name);
					buttonCount++;
				}
				j.SetParent(_Canvas, false);
				RectTransform jr = j.GetComponent<RectTransform>();

				if (j.tag == "Keyboard"){
					if (!editCurrent)
						jr = j.Find("toggle").GetComponent<RectTransform>();
					else
						jr = j.GetComponent<RectTransform>();
				}

				//position + scale
				jr.anchorMax = myData.items[i].anchors;
				jr.sizeDelta = myData.items[i].sizeDelta;
				jr.anchorMin = myData.items[i].anchors;
				jr.pivot = myData.items[i].pivot;
				jr.anchoredPosition3D = myData.items[i].anchoredPosition;
				jr.localScale = myData.items[i].scale * relativeScale;

				//color
				myData.items[i].setColor(j.gameObject, myData.items[i].objColor);

				//text
				string temp = myData.items[i].objText;
				Text txt = j.GetComponentInChildren<Text>();
				if (txt != null)
					txt.text = temp;

				//commands
				temp = myData.items[i].objCommand;
				if (temp.Length > 0)
				{
					if (!editCurrent)
						j.GetComponentInChildren<ButtonHandler>().command = temp;
					j.GetComponent<propertyContainer>().command = temp;
				}

				if (j.tag == "Dpad")
				{
					j.GetComponent<propertyContainer>().dpad = myData.items[i].objDpad;
					ButtonHandler[] assets = j.GetComponentsInChildren<ButtonHandler>();

					foreach(ButtonHandler button in assets)
					{
						switch (button.transform.parent.name)
						{
						case "up":
							button.command = myData.items[i].objDpad.up;
							break;
						case "down":
							button.command = myData.items[i].objDpad.down;
							break;
						case "left":
							button.command = myData.items[i].objDpad.left;
							break;
						case "right":
							button.command = myData.items[i].objDpad.right;
							break;

						}
					}
				}

				//icon images
				if (j.tag == "UI_Button")
				{

					Sprite sprImage = Resources.Load<Sprite>(myData.items[i].objImage);
					Sprite sprBackground = Resources.Load<Sprite>(myData.items[i].objBackground);

					//Sprite sprImage = AssetDatabase.LoadAssetAtPath<Sprite>(myData.items[i].objImage);
					//Sprite sprBackground = AssetDatabase.LoadAssetAtPath<Sprite>(myData.items[i].objBackground);
					Image[] icons = j.transform.GetComponentsInChildren<Image>();
					foreach(Image icon in icons)
					{
						switch (icon.name)
						{
						case "Button_hitbox":
							icon.overrideSprite = sprImage;
							break;
						default:
							icon.overrideSprite = sprBackground;
							break;
						}
					}
				}

				if (j.tag == "Keyboard")
				{
					if (!editCurrent){
						j.SetParent(GameObject.Find("settingsMenu").transform, false);
						j.Find("interface").GetComponent<RectTransform>().anchorMin = myData.items[i].layout.anchorMin;
						j.Find("interface").GetComponent<RectTransform>().anchorMax = myData.items[i].layout.anchorMax;
					}else{
						j.GetComponent<propertyContainer>().keyboard.anchorMin = myData.items[i].layout.anchorMin;
						j.GetComponent<propertyContainer>().keyboard.anchorMax = myData.items[i].layout.anchorMax;
					}

				}

			}

			GameObject go = GameObject.Find("settingsMenu");
			createSettingsList listScript = (createSettingsList)go.GetComponent(typeof(createSettingsList));
			listScript.LayoutPanelSlide(setVisible:false);
			Screen.orientation = myData.screen.orientation;
			listScript.currentScreen = Screen.orientation;
			myData = null;
		} 
	} 



	// Here we deserialize it back into its original form 
	object DeserializeObject(string pXmlizedString) 
	{ 
		XmlSerializer xs = new XmlSerializer(typeof(DataContainer)); 
		MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString)); 
		//XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8); 
		return xs.Deserialize(memoryStream); 
	}


	byte[] StringToUTF8ByteArray(string pXmlString) 
	{ 
		UTF8Encoding encoding = new UTF8Encoding(); 
		byte[] byteArray = encoding.GetBytes(pXmlString); 
		return byteArray; 
	} 
 
} 
