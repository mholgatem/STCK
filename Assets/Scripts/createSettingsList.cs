using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class createSettingsList : MonoBehaviour {


	public GameObject sampleButton;
	
	public List<settingButton> buttonList;
	public GameObject menuPanel;
	public RectTransform LayoutPanel;
	public RectTransform contentPanel;
	public GameObject savePanel;
	public GameObject controlEditor;
	public GameObject saveNameInputField;
	public GameObject background;
	public GameObject keyboardLayout;
	public static GameObject _keyboardLayout;
	public Toggle menuToggle;

	[HideInInspector] public ScreenOrientation currentScreen; //set by xmlmanager loadxml()
	
	private bool menuOpen = false;
	private TouchScreenKeyboard keyboard;
	private Canvas settingsMenu;
	private Rect menuSpriteRect;
	private string currentLayoutName = "";
	private RectTransform mpRect;
	private settingButton lastClicked;


	int menuSpriteSize = (int)(Screen.width / 100) * 4;

	void Start () {

		path = Application.persistentDataPath;

		mpRect = menuPanel.GetComponent<RectTransform>();
		mpRect.anchoredPosition = new Vector2(0, -menuToggle.GetComponent<RectTransform>().rect.height);

		menuPanel.SetActive(menuToggle.isOn);
		//this fixes 5.2 bug with blocking mask

		_keyboardLayout = keyboardLayout;


	}
	

	void Update(){

	}

	public void PopulateList (bool openMenu) {
		//menuOpen = menuToggle;
		mpRect.anchoredPosition = new Vector2(0, -menuToggle.GetComponent<RectTransform>().rect.height);
		if (Screen.orientation != ScreenOrientation.AutoRotation)
			currentScreen = Screen.orientation;

		foreach (Transform child in contentPanel) 
		{
			GameObject.Destroy(child.gameObject);
		}
		var info = new DirectoryInfo(Application.persistentDataPath);
		var fileInfo = info.GetFiles();
		buttonList.Clear();
		foreach (var file in fileInfo){
			if (Path.GetExtension(file.ToString()).ToUpper() == ".XML"){
				//item currentItem = new item();
				//buttonList.Add (currentItem);
				GameObject layoutNameButton = Instantiate(sampleButton) as GameObject;
				settingButton b = layoutNameButton.GetComponent<settingButton>();
				b.labelText.text = Path.GetFileNameWithoutExtension(file.ToString());
				b.path = file.FullName;
				b.layoutName = Path.GetFileNameWithoutExtension(file.Name);
				b.button.onClick.AddListener(() => LoadControlFormat(b));
				layoutNameButton.transform.SetParent (contentPanel, false);
				buttonList.Add (b);
			}
		}


		if (Screen.orientation != ScreenOrientation.AutoRotation){
			GameObject rotationButton = Instantiate(sampleButton) as GameObject;
			settingButton rb = rotationButton.GetComponent<settingButton>();
			if (Screen.orientation == ScreenOrientation.LandscapeLeft)
				rb.labelText.text = "» Screen Rotate (Portrait)";
			else
				rb.labelText.text = "» Screen Rotate (Landscape)";
			rb.labelText.alignment = TextAnchor.MiddleLeft;
			rb.button.onClick.AddListener( () => toggleRotation() );
			rotationButton.transform.SetParent (contentPanel, false);
			buttonList.Add (rb);
		}

		GameObject newButton = Instantiate(sampleButton) as GameObject;
		settingButton nb = newButton.GetComponent<settingButton>();
		nb.labelText.text = "» Create New";
		nb.labelText.alignment = TextAnchor.MiddleLeft;
		nb.button.onClick.AddListener( () => NewLayout() );
		newButton.transform.SetParent (contentPanel, false);
		buttonList.Add (nb);

		GameObject editButton = Instantiate(sampleButton) as GameObject;
		settingButton eb = editButton.GetComponent<settingButton>();
		eb.labelText.text = "» Edit Current";
		eb.labelText.alignment = TextAnchor.MiddleLeft;
		eb.button.onClick.AddListener( () => EditLayout() );
		editButton.transform.SetParent (contentPanel, false);
		buttonList.Add (eb);

		GameObject quitButton = Instantiate(sampleButton) as GameObject;
		settingButton qb = quitButton.GetComponent<settingButton>();
		qb.labelText.text = "» Quit";
		qb.labelText.alignment = TextAnchor.MiddleLeft;
		qb.button.onClick.AddListener( () => Application.Quit() );
		quitButton.transform.SetParent (contentPanel, false);
		buttonList.Add (qb);


	}

	public void LoadControlFormat(settingButton btn, bool editCurrent = false){
		LayoutPanelSlide (setVisible:false);
		currentLayoutName = btn.layoutName;
		Debug.Log (currentLayoutName);
		GameObject go = GameObject.FindWithTag("ControlCanvas");
		XMLManager xmlscript = (XMLManager) go.GetComponent(typeof(XMLManager));
		xmlscript.LoadXML(btn.path, editCurrent);
		menuToggle.isOn = false;
		lastClicked = btn;
		//menuPanel.SetActive(menuToggle.isOn);

	}

	public void toggleRotation()
	{
		if (Screen.orientation == ScreenOrientation.LandscapeLeft)
		{
			Screen.orientation = ScreenOrientation.Portrait;
		}else{
			Screen.orientation = ScreenOrientation.LandscapeLeft;
		}

	}
	public void EditLayout()
	{
		Screen.orientation = ScreenOrientation.AutoRotation;
		if (currentLayoutName.Length != 0)
		{
			LoadControlFormat(lastClicked, true);
			LayoutPanelSlide (setVisible:true);
		}else{
			NewLayout();
		}

		//menuPanel.SetActive(menuToggle.isOn);

	}

	public void LayoutPanelSlide(bool setVisible)
	{
		if (setVisible)
		{
			LayoutPanel.GetComponent<SlideOutMenu>().SetState("open");

		}
		else
		{
			LayoutPanel.GetComponent<SlideOutMenu>().SetState("close");
		}

	}

	public void NewLayout()
	{
		Screen.orientation = ScreenOrientation.AutoRotation;
		LayoutPanelSlide (setVisible:true);
		menuToggle.isOn = false;
		//menuPanel.SetActive(menuToggle.isOn);

		GameObject go = GameObject.FindWithTag("ControlCanvas");
		XMLManager xmlscript = (XMLManager) go.GetComponent(typeof(XMLManager));
		xmlscript.ClearCanvas();
	}

	public static void setKeyboardLayoutState(bool enabled, GameObject keyboard){
		_keyboardLayout.SetActive(enabled);
		keyboard.GetComponent<propertyContainer>().keyboard = _keyboardLayout.GetComponent<RectTransform>();

	}


}
