using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour 
{
	public int currentCategory;
	public List<MainMenuPanels> Panels = new List<MainMenuPanels>();

	public List<ScrollMaps> Scrollers = new List<ScrollMaps>();

	void Start () 
	{
		for (int i = 0; i < Scrollers.Count; i++)
			Scrollers[i].ScrollBar.GetComponent<Scrollbar>().value = 1;
		GoToCategory(Panels.Count - 1);
		RefreshOwnEditMaps();
		RefreshCommunityMaps();
		DrawSoloMaps();
	}

	public void RefreshCommunityMaps ()
	{
		CoreGame.m_pCoreGame.CommunityMaps.Clear();
		CoreGame.m_pCoreGame.m_pDatabaseManager.GetCommunityMap();
	}

	public void DrawCommunityMaps ()
	{
		for (int j = 0 ; j < Scrollers[2].MapsInstantiated.Count; j++)
			if (Scrollers[2].MapsInstantiated[j])
				Destroy(Scrollers[2].MapsInstantiated[j]);

		if (CoreGame.m_pCoreGame.CommunityMaps.Count < 11) Scrollers[2].ScrollBar.SetActive(false);
		else
		{
			Scrollers[2].ScrollContent.sizeDelta = new Vector2(Scrollers[2].ScrollContent.rect.width,(CoreGame.m_pCoreGame.CommunityMaps.Count * 75) + 75);
			Scrollers[2].ScrollBar.SetActive(true);
		}
		
		float StartPos = (Scrollers[2].ScrollContent.sizeDelta.y / 2) - 75;
		for (int i = 0; i < CoreGame.m_pCoreGame.CommunityMaps.Count; i ++)
		{
			GameObject tempobj = (GameObject) Instantiate(Scrollers[2].PrefabItem);
			RectTransform transformObj = tempobj.GetComponent<RectTransform>();
			transformObj.SetParent(Scrollers[2].ScrollContent.transform);
			transformObj.localScale = new Vector3(1, 1, 1);
			transformObj.localPosition = new Vector3(0, StartPos, 0);
			int newI = i;
			
			tempobj.transform.GetChild(0).GetComponent<Text>().text = CoreGame.m_pCoreGame.CommunityMaps[i].Name;
			
			tempobj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { DownloadMap(newI);});
			
			
			Scrollers[2].MapsInstantiated.Add(tempobj);
			
			
			StartPos -= 75;
		}
	}


	public void RefreshOwnEditMaps ()
	{
		CoreGame.m_pCoreGame.Maps.Clear();
		CoreGame.m_pCoreGame.Maps = CoreGame.m_pCoreGame.m_pStreamsWriterLoader.LoadMaps();
		
		DrawOwnEditMaps();
		DrawOwnPlayMaps();
	}

	public void DrawOwnEditMaps ()
	{
		for (int j = 0 ; j < Scrollers[1].MapsInstantiated.Count; j++)
			if(Scrollers[1].MapsInstantiated[j])
				Destroy(Scrollers[1].MapsInstantiated[j]);

		if(CoreGame.m_pCoreGame.Maps.Count < 11) Scrollers[1].ScrollBar.SetActive(false);
		else
		{
			Scrollers[1].ScrollContent.sizeDelta = new Vector2(Scrollers[1].ScrollContent.rect.width,(CoreGame.m_pCoreGame.Maps.Count * 75) + 75);
			Scrollers[1].ScrollBar.SetActive(true);
		}
		
		float StartPos = (Scrollers[1].ScrollContent.sizeDelta.y / 2) - 75;
		for(int i = 0; i < CoreGame.m_pCoreGame.Maps.Count; i ++)
		{
			GameObject tempobj = (GameObject) Instantiate(Scrollers[1].PrefabItem);
			RectTransform transformObj = tempobj.GetComponent<RectTransform>();
			transformObj.SetParent(Scrollers[1].ScrollContent.transform);
			transformObj.localScale = new Vector3(1,1,1);
			transformObj.localPosition = new Vector3(0,StartPos,0);
			int newI = i;
			tempobj.transform.GetChild(0).GetComponent<Text>().text = CoreGame.m_pCoreGame.Maps[i].Name;
			tempobj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { EditMap(newI);});
			tempobj.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { ShareMap(newI);});
			tempobj.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => { DeleteMap(newI);});
			
			
			Scrollers[1].MapsInstantiated.Add(tempobj);
			
			
			StartPos -= 75;
		}
	}

	public void DrawOwnPlayMaps ()
	{
		for (int j = 0 ; j < Scrollers[0].MapsInstantiated.Count; j++)
			if(Scrollers[0].MapsInstantiated[j])
				Destroy(Scrollers[0].MapsInstantiated[j]);
		
		if(CoreGame.m_pCoreGame.Maps.Count < 11) Scrollers[0].ScrollBar.SetActive(false);
		else
		{
			Scrollers[0].ScrollContent.sizeDelta = new Vector2(Scrollers[0].ScrollContent.rect.width,(CoreGame.m_pCoreGame.Maps.Count * 75) + 75);
			Scrollers[0].ScrollBar.SetActive(true);
		}
		
		float StartPos = (Scrollers[0].ScrollContent.sizeDelta.y / 2) - 75;
		for (int i = 0; i < CoreGame.m_pCoreGame.Maps.Count; i ++)
		{
			GameObject tempobj = (GameObject) Instantiate(Scrollers[0].PrefabItem);
			RectTransform transformObj = tempobj.GetComponent<RectTransform>();
			transformObj.SetParent(Scrollers[0].ScrollContent.transform);
			transformObj.localScale = new Vector3(1,1,1);
			transformObj.localPosition = new Vector3(0,StartPos,0);
			int newI = i;

			tempobj.transform.GetChild(0).GetComponent<Text>().text = CoreGame.m_pCoreGame.Maps[i].Name;
			tempobj.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Play";
			tempobj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { PlayMap(newI);});
			
			
			Scrollers[0].MapsInstantiated.Add(tempobj);
			
			
			StartPos -= 75;
		}
	}

	public void DrawSoloMaps ()
	{
		for(int j = 0 ; j < Scrollers[3].MapsInstantiated.Count; j++)
			if(Scrollers[3].MapsInstantiated[j])
				Destroy(Scrollers[3].MapsInstantiated[j]);
		
		if (CoreGame.m_pCoreGame.SoloMaps.Count < 11) Scrollers[3].ScrollBar.SetActive(false);
		else
		{
			Scrollers[3].ScrollContent.sizeDelta = new Vector2(Scrollers[3].ScrollContent.rect.width,(CoreGame.m_pCoreGame.SoloMaps.Count * 75) + 75);
			Scrollers[3].ScrollBar.SetActive(true);
		}
		
		float StartPos = (Scrollers[3].ScrollContent.sizeDelta.y / 2) - 75;
		for (int i = 0; i < CoreGame.m_pCoreGame.SoloMaps.Count; i++)
		{
			GameObject tempobj = (GameObject) Instantiate(Scrollers[3].PrefabItem);
			RectTransform transformObj = tempobj.GetComponent<RectTransform>();
			transformObj.SetParent(Scrollers[3].ScrollContent.transform);
			transformObj.localScale = new Vector3(1, 1, 1);
			transformObj.localPosition = new Vector3(0, StartPos, 0);
			int newI = i;
			
			tempobj.transform.GetChild(0).GetComponent<Text>().text = CoreGame.m_pCoreGame.Maps[i].Name;
			tempobj.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Play";
			tempobj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { PlaySoloMap(newI);});
			
			
			Scrollers[3].MapsInstantiated.Add(tempobj);
			
			
			StartPos -= 75;
		}
	}



	public void DownloadMap (int i)
	{
		CoreGame.m_pCoreGame.m_pDatabaseManager.GetSpecificMap(CoreGame.m_pCoreGame.CommunityMaps[i].DatabaseID);
	}

	public void EditMap (int i)
	{
		CoreGame.m_pCoreGame.CurrentMap = CoreGame.m_pCoreGame.Maps[i].Name;
		CoreGame.m_pCoreGame.m_pDynamicLoader.LoadedMap = CoreGame.m_pCoreGame.Maps[i];
		CoreGame.m_pCoreGame.m_pDynamicLoader.LoadALevel(1);
	}
	
	public void ShareMap (int i)
	{
		CoreGame.m_pCoreGame.m_pDatabaseManager.UploadDatabase(CoreGame.m_pCoreGame.Maps[i].Name,CoreGame.m_pCoreGame.Maps[i].UnparsedMap);
	}

	public void DeleteMap (int i)
	{
		CoreGame.m_pCoreGame.m_pStreamsWriterLoader.DeleteMap(CoreGame.m_pCoreGame.Maps[i].Name);

		RefreshOwnEditMaps();
	}

	public void PlayMap (int i)
	{
		CoreGame.m_pCoreGame.CurrentMap = CoreGame.m_pCoreGame.Maps[i].Name;
		CoreGame.m_pCoreGame.m_pDynamicLoader.LoadedMap = CoreGame.m_pCoreGame.Maps[i];
		CoreGame.m_pCoreGame.m_pDynamicLoader.LoadALevel(2);
	}

	public void PlaySoloMap(int i)
	{
		CoreGame.m_pCoreGame.CurrentMap = CoreGame.m_pCoreGame.SoloMaps[i].Name;
		CoreGame.m_pCoreGame.m_pDynamicLoader.LoadedMap = CoreGame.m_pCoreGame.SoloMaps[i];
		CoreGame.m_pCoreGame.m_pDynamicLoader.LoadALevel(2);
	}

	public void CreateNewMap (Object mapname)
	{
		GameObject tempobj = (GameObject) mapname;
		CoreGame.m_pCoreGame.m_pStreamsWriterLoader.CreateMap(tempobj.GetComponent<Text>().text);
		CoreGame.m_pCoreGame.m_pDynamicLoader.LoadALevel(1);
	}

	public void GoToCategory (int category)
	{
		currentCategory = category;

		for (int i = 0; i < Panels.Count; i++)
		{
			if (i != currentCategory)
				Panels[i].animator.SetBool("Show",false);
		}

		Panels[currentCategory].animator.SetBool("Show",true);
	}

	public void Quit ()
	{
		Application.Quit();
	}

}
public enum MenuCategory {Play, Community, Options, Credits, MapEdit, Maps, AskMapName, Main }

[System.Serializable]
public class MainMenuPanels
{
	public string name;
	public Animator animator;
	public CanvasGroup group;
	public MenuCategory category;
}

[System.Serializable]
public class ScrollMaps
{
	public string nameElement;
	public RectTransform ScrollContent;
	public GameObject ScrollBar;
	[System.NonSerialized] public List<GameObject> MapsInstantiated = new List<GameObject>();
	public GameObject PrefabItem;
}

[System.Serializable]
public class Map
{
	/********************************************************************************
	 * remember : TopLeft = 0; TopRight = 1; BotLeft = 2; BotRight = 3;
	 * CHARACTER LISTS FOR PARSING THE MAP LISTED RIGHT HERE :                      *
	 * DELIMIT EACH CASE WITH THIS CHARACTER '&'
	 * DELIMIT POSITION X then Y ON CASE WITH THIS CHARACTER ';'
	 * DELIMIT EACH PLANS ON CASE WITH THIS CHARACTER '!' and put each itemprefab sous grid int it
	 * DELIMIT EACH SOUSGRID ON CASE WITH THIS CHARACTER '|' and put the itemPrefab with his ID in it
	 * 
	 * 
	 * 
	 * 
	 * 
	 * EXAMPLE : 24;30!1|1|1|1!1|1|1|1!1|1|1|1
	 * this mean you put a red cube on every plan in the position 24;30 if you admit that 1 is the ID of the red cube prefab
	 ********************************************************************************/
	public int DatabaseID;
	public string Name;
	public string UnparsedMap;
	public GameObject ButtonMap;
	
	public Map (string MapName, string MapParse)
	{
		Name = MapName;
		UnparsedMap = MapParse;
	}

	public Map (int ID, string MapName)
	{
		Name = MapName;
		DatabaseID = ID;
	}
}




















