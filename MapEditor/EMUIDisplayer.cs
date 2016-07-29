using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum ItemPosType { Full, Quarter, HalfH, HalfV}
public enum ItemType { Trigger, Props, Door, Trap }

public class EMUIDisplayer : MonoBehaviour 
{
	public Text TextPosition;
	[System.NonSerialized] public EMMouseHolder MouseHolder;
	[System.NonSerialized] public EMGridMapper GridMapper;
	

	public Text HeaderCategory;
	public RawImage PrefabImg;
	public int CurrentCategory = 1;

	public Text PanelInfoName;
	public Text PanelCategoryName;
	public Text PanelTypeName;

	public List<Animator> PlanLevelAnim = new List<Animator>();
	

	public ItemPrefab CurrentItem;
	public int PlanLevel;
	[System.NonSerialized] public Camera MainCam;

	public Text ReductButton;

	RectTransform GroupItems;


	float CurrentGUICameraRectX;
	float CurrentMainCameraRectX;

	float currentXPanelItemPosition;

	bool reductedPanelItem;

	bool isPlaying;

	bool gridEnabled = true;

	void Start () 
	{
		MouseHolder = GetComponent<EMMouseHolder>();
		GridMapper = GetComponent<EMGridMapper>();
		MainCam = GameObject.Find("MainCamera").GetComponent<Camera>();
		SetEveryObjects();
		SetPlanLevel(1);
		MoveCategory(false);
		PressedAnItemID(0);
		GroupItems = HeaderCategory.transform.root.GetComponent<RectTransform>();
	}

	void SetEveryObjects ()
	{
		for (int i = 0; i < GridMapper.Categories.Count; i++)
		{
			int PosX = -100;
			int posY = 350;
			int UiInstantiated = 0;
			for (int k = 0; k < GridMapper.Categories[i].ItemListCat.Count; k++)
			{
				GridMapper.Categories[i].ItemListCat[k].ItemID = k;
				GridMapper.Categories[i].ItemListCat[k].CategoryID = i;
				int ItemID = k;

				RawImage img = Instantiate(PrefabImg) as RawImage;
				img.texture = GridMapper.Categories[i].ItemListCat[k].textItem;
				img.rectTransform.SetParent(GridMapper.Categories[i].RTScrollMaskUI);
				img.rectTransform.localScale = new Vector3(1,1,1);
				img.rectTransform.transform.localPosition = new Vector3(PosX,posY,0);

				img.GetComponent<Button>().onClick.AddListener(() => { PressedAnItemID(ItemID);});

				UiInstantiated++;
				PosX += 100;
				if (UiInstantiated > 2)
				{
					posY -= 100;
					PosX = -100;
					UiInstantiated = 0;
				}
			}
		}
	}
	void SetInformations (int i)
	{
		CurrentItem = GridMapper.Categories[CurrentCategory].ItemListCat[i];
		PanelInfoName.text = CurrentItem.nameItem;
		PanelCategoryName.text = GridMapper.Categories[CurrentCategory].NameCate;
		PanelTypeName.text = CurrentItem.PosType.ToString();
		MouseHolder.CurrentObjectToPlace = CurrentItem;
	}

	void Update () 
	{
		if (MouseHolder.CurrentMasterGrid != null && MouseHolder.CurrentUnderGrid != null)
		{
			if (MouseHolder.CurrentUnderGrid.GridOBJ != null)
			{
				TextPosition.text =	"Positions : " + MouseHolder.CurrentMasterGrid.position.x + " ; " +
							MouseHolder.CurrentMasterGrid.position.y + " \n Under Position : " +
							MouseHolder.CurrentUnderGrid.GridOBJ.tag;
			}
		}

		if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
			SetPlanLevel(0);
		if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
			SetPlanLevel(1);
		if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
			SetPlanLevel(2);


		CurrentGUICameraRectX = Mathf.Lerp(CurrentGUICameraRectX, reductedPanelItem == true ? 0.98f : 0.78f, Time.deltaTime * 3.0f);
		CurrentMainCameraRectX = Mathf.Lerp(CurrentMainCameraRectX, reductedPanelItem == true ? -0.02f : -0.22f, Time.deltaTime * 3.0F);
		currentXPanelItemPosition = Mathf.Lerp(currentXPanelItemPosition, reductedPanelItem == true ? 42.8f : 40.7f, Time.deltaTime * 3.0f);

		MouseHolder.CameraGUI.camera.rect = new Rect (CurrentGUICameraRectX, MouseHolder.CameraGUI.camera.rect.y, MouseHolder.CameraGUI.camera.rect.width, MouseHolder.CameraGUI.camera.rect.height);
		MainCam.rect = new Rect (CurrentMainCameraRectX, 0, MainCam.rect.width,MainCam.rect.height);
		GroupItems.transform.position = new Vector3(currentXPanelItemPosition, GroupItems.transform.position.y,GroupItems.transform.position.z);
	}
	

	public void PressedAnItemID (int ID)
	{
		for(int i = 0; i < GridMapper.Categories[CurrentCategory].ItemListCat.Count; i++)
		{
			if(i == ID) SetInformations(ID);
		}
	}

	public void ReductPanelItem ()
	{
		reductedPanelItem = !reductedPanelItem;

		ReductButton.text = reductedPanelItem == true ? "<" : ">";
	}
	public void ChangeCameraMode ()
	{
		MainCam.orthographic = !MainCam.isOrthoGraphic;
	}

	public void DisEnableGrid ()
	{
		foreach(Grid temp in GridMapper.MapGrids)
		{
			temp.DisEnabledGrid();
		}

		gridEnabled = !gridEnabled;
	}

	public void SetPlanLevel (int planLevel)
	{
		for(int i = 0; i < PlanLevelAnim.Count; i++)
		{
			PlanLevelAnim[i].SetBool("Selected",false);
		}
		PlanLevel = planLevel;
		MouseHolder.CurrentPlan = planLevel;
		PlanLevelAnim[planLevel].SetBool("Selected",true);
	}
	public void Play (Text textModify)
	{
		if(!isPlaying)
		{
			//instantiate character and disable editor camera, also reduce panel item if we didnt already check the bouton Play to Stop
			if(GridMapper.SpawnPosition != null)
			{
				GridMapper.SpawnPlayer();

				if(MainCam.orthographic) ChangeCameraMode ();
				if(gridEnabled) DisEnableGrid();
				if(!reductedPanelItem) ReductPanelItem ();
				isPlaying = true;
				textModify.text = "Stop";
			}
		}

		else
		{
			//destroy the character, enable editor camera and un reduce panel item if we didnt already check the bouton Stop to Play
			GridMapper.DeletePlayer();

			if(!gridEnabled) DisEnableGrid();
			if(reductedPanelItem) ReductPanelItem();
			isPlaying = false;
			textModify.text = "Play";
		}
	}

	public void MoveCategory (bool Next)
	{
		CurrentCategory = (CurrentCategory + (Next == true ? 1 : -1)) % GridMapper.Categories.Count;

		for(int i = 0; i < GridMapper.Categories.Count; i++)
			GridMapper.Categories[i].DisableCategory();
		GridMapper.Categories[CurrentCategory].EnableCategory();
		HeaderCategory.text = GridMapper.Categories[CurrentCategory].NameCate;
	}

	public void Save ()
	{
		CoreGame.m_pCoreGame.m_pStreamsWriterLoader.SaveMap(CoreGame.m_pCoreGame.CurrentMap, GridMapper.ParseMap());
	}

	public void Quit ()
	{
		CoreGame.m_pCoreGame.m_pDynamicLoader.LoadALevel(0);
	}
}
[System.Serializable]
public class ItemPrefab
{
	public string nameItem;
	[System.NonSerialized] public int ItemID;
	[System.NonSerialized] public int CategoryID;
	public Texture2D textItem;
	public GameObject PrefabItem;
	public ItemPosType PosType = ItemPosType.Full;
}

[System.Serializable]
public class Category
{
	public string NameCate;
	public CanvasGroup CatUI;
	public RectTransform RTScrollMaskUI;
	public List<ItemPrefab> ItemListCat = new List<ItemPrefab>();
	public ItemType IType = ItemType.Props;

	public void DisableCategory ()
	{
		CatUI.alpha = 0;
		CatUI.blocksRaycasts = false;
		CatUI.interactable = false;
	}
	public void EnableCategory ()
	{
		CatUI.alpha = 1;
		CatUI.blocksRaycasts = true;
		CatUI.interactable = true;
	}
}
