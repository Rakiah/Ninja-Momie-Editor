using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EMGridMapper : MonoBehaviour 
{
	public static int XGrid = 40;
	public static int YGrid = 40;
	public int PauseEveryCreation = 10;
	public Transform GridHolder;
	public GameObject GridPrefab;
	public GameObject MapHolder;

	public GameObject Player;
	GameObject instantiatedplayer;


	public Grid SpawnPosition;

	public List<Category> Categories = new List<Category>();

	public Grid [,] MapGrids = new Grid[XGrid,YGrid];


	public EMCameraMovement Cam;

	public bool isEditor;

	void Start () 
	{
		SpawnPosition = null;
		StartCoroutine(DrawGrid());
	}

	public void SpawnPlayer ()
	{
		if (SpawnPosition != null)
		{
			Debug.Log("Player Spawned");
			instantiatedplayer = (GameObject)Instantiate(Player);
			instantiatedplayer.transform.localPosition = new Vector3(SpawnPosition.position.x * 10.0f, SpawnPosition.position.y * 10.0f, (1 + 0.5f) * 10.0F);

			if (Cam)
			{
				Cam.playertoFollow = instantiatedplayer;
				Cam.playing = true;
				Cam.transform.localEulerAngles = new Vector3(10,0,0);
			}
		}
	}

	public void DeletePlayer ()
	{
		if (Cam)
		{
			Cam.playing = false;
			Cam.playertoFollow = null;
			Cam.transform.localEulerAngles = new Vector3(0,0,0);
		}
		Destroy(instantiatedplayer);
	}

	IEnumerator DrawGrid ()
	{
		for (int i = 0; i < Categories.Count; i++)
		{
			for (int k = 0; k < Categories[i].ItemListCat.Count; k++)
			{
				Categories[i].ItemListCat[k].ItemID = k;
				Categories[i].ItemListCat[k].CategoryID = i;
			}
		}
		int pauser = 0;
		for (int i = 0; i < XGrid; i++)
		{
			for(int k = 0; k < YGrid; k++)
			{
				CreateNewGrid(i,k);
				CoreGame.m_pCoreGame.m_pDynamicLoader.LoadOne();
				pauser ++;
				if(pauser >= PauseEveryCreation) { pauser = 0; yield return null; }
			}
		}
		if (Cam) Cam.MoveDirection = new Vector3(XGrid * 5,YGrid * 5,0);

		StartCoroutine(LoadMap(CoreGame.m_pCoreGame.m_pDynamicLoader.LoadedMap.UnparsedMap));
	}
	
	void CreateNewGrid (int x, int y)
	{
		GameObject newGrid = Instantiate(GridPrefab,new Vector3(x * (GridPrefab.transform.localScale.x * 10.0f), y * (GridPrefab.transform.localScale.y * 10.0f), 0), GridPrefab.transform.rotation) as GameObject;
		newGrid.transform.SetParent(GridHolder);
		Grid tempGrid = new Grid(newGrid, x, y);
		newGrid.GetComponent<EMGridSingle>().GridSingle = tempGrid;
		MapGrids[x, y] = tempGrid;
	}


	public void CreateOBJ (ItemPrefab item, UnderGrid gridTo, int Plan)
	{
		GameObject TempOBJ = Instantiate(item.PrefabItem) as GameObject;

		if (item.nameItem == "Start Level")
		{
			if (SpawnPosition != null)
			{
				for (int j = 0; j < 4; j++)
				{
					for (int i = 0; i < 3; i++)
					{
						if(SpawnPosition.SousGrid[j].Plans[i] != null)
						{
							if (SpawnPosition.SousGrid[j].Plans[i].name == "0$1")
								Destroy(SpawnPosition.SousGrid[j].Plans[i]);
						}
					}
				}
				SpawnPosition = null;
			}
			SpawnPosition = gridTo.MasterGrid;
		}

		gridTo.AddOBJ(item, TempOBJ, MapHolder, Plan);
	}


	IEnumerator LoadMap (string data)
	{
		if(data != "")
		{
			int pauser = 0;
			string [] CaseCube = data.Split('&');
			for (int i = 0; i < CaseCube.Length - 1; i++)
			{
				string [] positions = CaseCube[i].Split(';');
				//this string should had get everything after the Y position, so destroy everything that come after the plan character separator '!'
				int index = positions[1].IndexOf('!');
				if (index > 0) positions[1] = positions[1].Substring(0, index);
				int [] positionV = new int[2];
				int.TryParse(positions[0], out positionV[0]);
				int.TryParse(positions[1], out positionV[1]);


				string [] plans = CaseCube[i].Split('!');
				//ignore the first case since its the position
				for (int j = 1; j < plans.Length; j++)
				{
					string [] undergridItems = plans[j].Split('|');
					//ignore the last term since its a null |
					for (int k = 0; k < undergridItems.Length - 1; k++)

					{
						string [] ItemsAndCategoriesID = undergridItems[k].Split('$');

						if (ItemsAndCategoriesID[0] != "N" && ItemsAndCategoriesID[1] != "N")
						{
							int itemID = 0;
							int categoryID = 0;

							int.TryParse(ItemsAndCategoriesID[0], out itemID);
							int.TryParse(ItemsAndCategoriesID[1], out categoryID);


							CreateOBJ(Categories[categoryID].ItemListCat[itemID],
						          		MapGrids[positionV[0],positionV[1]].SousGrid[k], j - 1);
							pauser++;
							if (pauser >= PauseEveryCreation) { pauser = 0; yield return null; }
						}
					}
				}
				CoreGame.m_pCoreGame.m_pDynamicLoader.LoadOne();
			}
			
			if (!isEditor)
			{
				SpawnPlayer();
				foreach (Grid temp in MapGrids)
				{
					temp.BGGrid.transform.SetParent(GridHolder);
					temp.MasterGrid.SetActive(false);
				}
			}
		}
	}

	/********************************************************************************************************
	 * remember : TopLeft = 0; TopRight = 1; BotLeft = 2; BotRight = 3;					*
	 * CHARACTER LISTS FOR PARSING THE MAP LISTED RIGHT HERE :                      *			*
	 * DELIMIT EACH CASE WITH THIS CHARACTER '&'								*
	 * DELIMIT POSITION X then Y ON CASE WITH THIS CHARACTER ';'						*
	 * DELIMIT EACH PLANS ON CASE WITH THIS CHARACTER '!' and put each itemprefab sous grid int it		*
	 * DELIMIT EACH SOUSGRID ON CASE WITH THIS CHARACTER '|' and put the itemPrefab with his ID in it	*
	 *													*
	 *													*
	 *													^
	 *													*
	 *													*
	 * EXAMPLE : 24;30!1|1|1|1!1|1|1|1!1|1|1|1								*
	 * this mean you put a red cube on every plan in the position 24;30 if you admit			*
	 * that 1 is the ID of the red cube prefab								*
	 ********************************************************************************************************/
	public string ParseMap ()
	{
		string ParsedMap = "";

		foreach(Grid obj in MapGrids)
		{
			bool nullCase = true;
			int i = 0;
			while(i < 4 && nullCase)
			{
				for(int j = 0; j < 3; j++)
				{
					if(obj.SousGrid[i].Plans[j] != null)
					{
						nullCase = false;
					}
				}
				i++;
			}
			//we check if the case is null if yes, dont even try to write something in the text, just go to the next case
			if(nullCase) continue;
			//otherwise, start writing the case in the string
			//Debug.Log(obj.position.x + ";" + obj.position.y + obj.GetTxtCase() + '&');

			ParsedMap += obj.position.x.ToString() + ';' + obj.position.y.ToString() + obj.GetTxtCase() + '&';
		}
		return ParsedMap;
	}

}

[System.Serializable]
public class Grid 
{
	public GameObject MasterGrid;

	[System.NonSerialized] public EMGridSingle masterGridBehaviour;

	//the list of under grid is placed like that : TopLeft = 0; TopRight = 1; BotLeft = 2; BotRight = 3;
	[System.NonSerialized] public List<UnderGrid> SousGrid = new List<UnderGrid>();
	[System.NonSerialized] public List<EMUnderGrid> childMonoGrid = new List<EMUnderGrid>();

	[System.NonSerialized] public GameObject BGGrid;
	public Vector2 position;

	public Grid (GameObject newGrid, int x, int y)
	{
		MasterGrid = newGrid;
		masterGridBehaviour = newGrid.GetComponent<EMGridSingle>();

		position.x = x;
		position.y = y;

		for(int incrementer = 0; incrementer < 4; incrementer++)
		{
			//we assign every monobehaviour and under grid class needed
			SousGrid.Add(new UnderGrid(newGrid.transform.GetChild(incrementer).gameObject,this,incrementer));
			childMonoGrid.Add(newGrid.transform.GetChild(incrementer).GetComponent<EMUnderGrid>());

			//here we assign his ID and his behaviour
			childMonoGrid[incrementer].AssignGrids(masterGridBehaviour, incrementer);
		}

		BGGrid = newGrid.transform.GetChild(4).gameObject;
		BGGrid.transform.localPosition = new Vector3(0,Random.Range(-35,-40), 0);
	}

	public void DisEnabledGrid ()
	{
		MasterGrid.renderer.enabled = !MasterGrid.renderer.enabled;
		for(int i = 0; i < 4; i++)
		{
			SousGrid[i].DisEnabledUnderGrid();
		}
	}

	public string GetTxtCase ()
	{
	/********************************************************************************
	 * remember : TopLeft = 0; TopRight = 1; BotLeft = 2; BotRight = 3;
	 * CHARACTER LISTS FOR PARSING THE MAP LISTED RIGHT HERE :                      *
	 * DELIMIT EACH CASE WITH THIS CHARACTER '&'
	 * DELIMIT POSITION X then Y ON CASE WITH THIS CHARACTER ';'
	 * DELIMIT EACH PLANS ON CASE WITH THIS CHARACTER '!' and put each itemprefab sous grid int it
	 * DELIMIT EACH SOUSGRID ON CASE WITH THIS CHARACTER '|' and put the itemPrefab with his ID in it
	 * DELIMIT CATEGORY AND ITEM ID ON CASE WITH THIS CHARACTER '$' and put the id in it
	 * 
	 * 
	 * 
	 * 
	 * EXAMPLE : 24;30!1|1|1|1!1|1|1|1!1|1|1|1
	 * this mean you put a red cube on every plan in the position 24;30 if you admit that 1 is the ID of the red cube prefab
	 ********************************************************************************/
		string ParsedCase = string.Empty;

		for (int i = 0; i < 3; i++)
		{
			ParsedCase += '!';
			for (int j = 0; j < 4; j++)
			{
				if (SousGrid[j].Plans[i] != null)	ParsedCase += SousGrid[j].Plans[i].name + '|';
				else					ParsedCase += "N$N" + '|';
			}
		}
		
		
		
		return ParsedCase;
	}
}

[System.Serializable]
public class UnderGrid 
{
	public GameObject GridOBJ;
	public Grid MasterGrid;
	public UnderGridType sousGridPos;
	public List<GameObject> Plans = new List<GameObject>();

	public UnderGrid (GameObject u_Grid, Grid mastGrid, int incrementer)
	{
		GridOBJ = u_Grid;
		MasterGrid = mastGrid;
		sousGridPos = (UnderGridType)incrementer;
		for (int i = 0; i < 3; i++)
		{
			Plans.Add(null);
		}
	}

	public void AddOBJ (ItemPrefab ItemToPlace, GameObject TempOBJ, GameObject MapHolder, int PlanID)
	{
		
		//that will handle full cube object
		if (ItemToPlace.PosType == ItemPosType.Full)
		{
			for(int i = 0; i < 4; i++)
			{
				//if there is an object, destroy it
				if(MasterGrid.SousGrid[i].Plans[PlanID] != null) CoreGame.m_pCoreGame.DestroyItem(MasterGrid.SousGrid[i].Plans[PlanID]);
				//and then assign the position to be free
				MasterGrid.SousGrid[i].Plans[PlanID] = null;
			}
			
			//then we create the new object and put it in his position
			
			TempOBJ.transform.SetParent(MapHolder.transform);
			TempOBJ.transform.localPosition = new Vector3(MasterGrid.position.x * 10.0f,MasterGrid.position.y * 10.0f,(PlanID + 0.5f) * 10.0F);
			
			
			//then we assign it to every under grid since its a full object item
			for(int j = 0; j < 4; j++)
			{
				MasterGrid.SousGrid[j].Plans[PlanID] = TempOBJ;
			}
		}
		
		//to remember object in the list sous grid are placed like this : 	
		//TopLeft = 0; TopRight = 1; BotLeft = 2; BotRight = 3;
		//that will handle half horizontal object like this __
		if (ItemToPlace.PosType == ItemPosType.HalfH)
		{
			TempOBJ.transform.SetParent(MapHolder.transform);
			//we check which position we are top side in case
			if (sousGridPos == UnderGridType.TopLeft || sousGridPos == UnderGridType.TopRight)
			{
				//here we check if there is something already in the topside, if yes, we destroy it
				for (int i = 0; i < 2; i++)
				{
					if (MasterGrid.SousGrid[i].Plans[PlanID] != null) CoreGame.m_pCoreGame.DestroyItem(MasterGrid.SousGrid[i].Plans[PlanID]);
					
					//and then assign the position to be free
					MasterGrid.SousGrid[i].Plans[PlanID] = null;
				}
				
				// we create and position the object
				
				TempOBJ.transform.localPosition = new Vector3(MasterGrid.position.x * 10.0f,(MasterGrid.position.y * 10.0f) + 2.5f,(PlanID + 0.5f) * 10.0F);
				
				
				//then we assign it to every topside under grid since its a half horizontal object item;
				for (int j = 0; j < 2; j++)
				{
					MasterGrid.SousGrid[j].Plans[PlanID] = TempOBJ;
				}
				
			}
			//or bot side
			else
			{
				//here we check if there is something already in the botside, if yes, we destroy it
				for (int i = 2; i < 4; i++)
				{
					if (MasterGrid.SousGrid[i].Plans[PlanID] != null) CoreGame.m_pCoreGame.DestroyItem(MasterGrid.SousGrid[i].Plans[PlanID]);
					
					//and then assign the position to be free
					MasterGrid.SousGrid[i].Plans[PlanID] = null;
				}
				
				// we create and position the object
				TempOBJ.transform.localPosition = new Vector3((MasterGrid.position.x * 10.0f),(MasterGrid.position.y * 10.0f) - 2.5f,(PlanID + 0.5f) * 10.0F);
				
				//then we assign it to every topside under grid since its a half horizontal object item;
				for (int j = 2; j < 4; j++)
				{
					MasterGrid.SousGrid[j].Plans[PlanID] = TempOBJ;
				}
			}
		}
		
		//to remember object in the list sous grid are placed like this : 	
		//TopLeft = 0; TopRight = 1; BotLeft = 2; BotRight = 3;
		//that will handle half vertical object like this |
		if (ItemToPlace.PosType == ItemPosType.HalfV)
		{
			TempOBJ.transform.SetParent(MapHolder.transform);
			//here we check if its left or right, in case left
			if (sousGridPos == UnderGridType.BotLeft || sousGridPos == UnderGridType.TopLeft)
			{
				//here we cant use little loops, since its not in the half vertical so we check them 1 by 1 and destroy them if there is something
				
				//mark here, maybe we can do a seiest thing
				if(MasterGrid.SousGrid[0].Plans[PlanID] != null) CoreGame.m_pCoreGame.DestroyItem(MasterGrid.SousGrid[0].Plans[PlanID]);
				if(MasterGrid.SousGrid[2].Plans[PlanID] != null) CoreGame.m_pCoreGame.DestroyItem(MasterGrid.SousGrid[2].Plans[PlanID]);
				
				MasterGrid.SousGrid[0].Plans[PlanID] = null;
				MasterGrid.SousGrid[2].Plans[PlanID] = null;
				
				TempOBJ.transform.localPosition = new Vector3((MasterGrid.position.x * 10.0f) - 2.5f,(MasterGrid.position.y * 10.0f),(PlanID + 0.5f) * 10.0F);
				MasterGrid.SousGrid[0].Plans[PlanID] = TempOBJ;
				MasterGrid.SousGrid[2].Plans[PlanID] = TempOBJ;
			}
			//and right
			else
			{
				if(MasterGrid.SousGrid[1].Plans[PlanID] != null) CoreGame.m_pCoreGame.DestroyItem(MasterGrid.SousGrid[1].Plans[PlanID]);
				if(MasterGrid.SousGrid[3].Plans[PlanID] != null) CoreGame.m_pCoreGame.DestroyItem(MasterGrid.SousGrid[3].Plans[PlanID]);
				
				MasterGrid.SousGrid[1].Plans[PlanID] = null;
				MasterGrid.SousGrid[3].Plans[PlanID] = null;
				
				TempOBJ.transform.localPosition = new Vector3((MasterGrid.position.x * 10.0f) + 2.5f,(MasterGrid.position.y * 10.0f),(PlanID + 0.5f) * 10.0F);
				MasterGrid.SousGrid[1].Plans[PlanID] = TempOBJ;
				MasterGrid.SousGrid[3].Plans[PlanID] = TempOBJ;
			}
		}
		
		//to remember object in the list sous grid are placed like this : 	
		//TopLeft = 0; TopRight = 1; BotLeft = 2; BotRight = 3;
		//that will handle quarter object like this -
		if (ItemToPlace.PosType == ItemPosType.Quarter)
		{
			TempOBJ.transform.SetParent(MapHolder.transform);
			switch (sousGridPos)
			{
				case UnderGridType.TopLeft :
				{
					//destroy where we need
					if(MasterGrid.SousGrid[0].Plans[PlanID] != null) CoreGame.m_pCoreGame.DestroyItem(MasterGrid.SousGrid[0].Plans[PlanID]);
					MasterGrid.SousGrid[0].Plans[PlanID] = null;
						
						
					//and rebuild
					TempOBJ.transform.localPosition = new Vector3((MasterGrid.position.x * 10.0f) - 2.5f,(MasterGrid.position.y * 10.0f) + 2.5f,(PlanID + 0.5f) * 10.0F);
					MasterGrid.SousGrid[0].Plans[PlanID] = TempOBJ;
				}
				break;
					
				case UnderGridType.TopRight :
				{
					if(MasterGrid.SousGrid[1].Plans[PlanID] != null) CoreGame.m_pCoreGame.DestroyItem(MasterGrid.SousGrid[1].Plans[PlanID]);
					MasterGrid.SousGrid[1].Plans[PlanID] = null;
						
						
						//and rebuild
					TempOBJ.transform.localPosition = new Vector3((MasterGrid.position.x * 10.0f) + 2.5f,(MasterGrid.position.y * 10.0f) + 2.5f,(PlanID + 0.5f) * 10.0F);
					MasterGrid.SousGrid[1].Plans[PlanID] = TempOBJ;
				}
				break;
					
				case UnderGridType.BotLeft :
				{
					if(MasterGrid.SousGrid[2].Plans[PlanID] != null) CoreGame.m_pCoreGame.DestroyItem(MasterGrid.SousGrid[2].Plans[PlanID]);
					MasterGrid.SousGrid[2].Plans[PlanID] = null;
						
						
						//and rebuild
					TempOBJ.transform.localPosition = new Vector3((MasterGrid.position.x * 10.0f) - 2.5f,(MasterGrid.position.y * 10.0f) - 2.5f,(PlanID + 0.5f) * 10.0F);
					MasterGrid.SousGrid[2].Plans[PlanID] = TempOBJ;
				}
				break;
					
				case UnderGridType.BotRight :
				{
					if(MasterGrid.SousGrid[3].Plans[PlanID] != null) CoreGame.m_pCoreGame.DestroyItem(MasterGrid.SousGrid[3].Plans[PlanID]);
					MasterGrid.SousGrid[3].Plans[PlanID] = null;
						
						
						//and rebuild
					TempOBJ.transform.localPosition = new Vector3((MasterGrid.position.x * 10.0f) + 2.5f,(MasterGrid.position.y * 10.0f) - 2.5f,(PlanID + 0.5f) * 10.0F);
					MasterGrid.SousGrid[3].Plans[PlanID] = TempOBJ;
				}
				break;
			}
		}
		
		TempOBJ.name = ItemToPlace.ItemID + "$" + ItemToPlace.CategoryID;
	}
	

	public void DisEnabledUnderGrid ()
	{
		GridOBJ.renderer.enabled = !GridOBJ.renderer.enabled;
	}
}

public enum UnderGridType { TopLeft, TopRight, BotLeft, BotRight }
