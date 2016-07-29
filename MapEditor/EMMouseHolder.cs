using UnityEngine;
using System.Collections;

public class EMMouseHolder : MonoBehaviour 
{
	public ItemPrefab CurrentObjectToPlace;
	public EMGridMapper GridMapper;

	public Grid CurrentMasterGrid;
	public UnderGrid CurrentUnderGrid;

	[System.NonSerialized] public GameObject CameraGUI;
	GameObject Eventsys;

	public int CurrentPlan;



	
	RaycastHit HitGUI;
	void Start () 
	{
		GridMapper = GetComponent<EMGridMapper>();
		CameraGUI = GameObject.Find("CameraGUIItems");
		Eventsys = GameObject.Find("EventSystem");
	}

	void Update ()
	{
		if (!CoreGame.m_pCoreGame.m_pDynamicLoader.IsLoading)
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			Ray ray2 = CameraGUI.camera.ScreenPointToRay(Input.mousePosition);


			if (!Physics.Raycast(ray, Mathf.Infinity))
			{
				CurrentMasterGrid = null;
				CurrentUnderGrid = null;
			}

			if (Physics.Raycast(ray2, out HitGUI, Mathf.Infinity))
			{
				if(HitGUI.collider.gameObject.tag == "UIElement")
				{
					Debug.Log("Currently On GUI");
					Eventsys.SetActive(true);
					CurrentMasterGrid = null;
					CurrentUnderGrid = null;
				}
				else
				{
					Eventsys.SetActive(false);
				}
			}
			else
			{
				Eventsys.SetActive(false);
			}

			if (Input.GetMouseButton(0) && CurrentMasterGrid != null)
			{
				GridMapper.CreateOBJ(CurrentObjectToPlace, CurrentUnderGrid, CurrentPlan);
			}

			if (Input.GetMouseButton(1) && Input.GetKey(KeyCode.LeftControl) && CurrentMasterGrid != null)
			{
				for (int i = 0; i < 3; i++)
				{
					if (CurrentUnderGrid.Plans[i] != null)
					{
						if (CurrentUnderGrid.Plans[i].name == "0$1") GridMapper.SpawnPosition = null;

						Destroy(CurrentUnderGrid.Plans[i]);
					}

				}
			}

			else if (Input.GetMouseButton(1) && CurrentMasterGrid != null)
			{
				if (CurrentUnderGrid.Plans[CurrentPlan] != null)
				{
					if (CurrentUnderGrid.Plans[CurrentPlan].name == "0$1") GridMapper.SpawnPosition = null;

					Destroy(CurrentUnderGrid.Plans[CurrentPlan]);
				}
			}
		}
	}
}