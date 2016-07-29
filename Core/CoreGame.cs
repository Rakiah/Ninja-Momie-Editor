using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoreGame : MonoBehaviour 
{
	public static CoreGame m_pCoreGame;
	public DynamicLoader m_pDynamicLoader;
	public StreamsWriteLoad m_pStreamsWriterLoader;
	public DatabaseManager m_pDatabaseManager;


	public List<Map> Maps = new List<Map>();
	
	public List<Map> CommunityMaps = new List<Map>();

	public List<Map> SoloMaps = new List<Map>();

	public string CurrentMap;
	

	void Awake () 
	{
		DontDestroyOnLoad(this);
		m_pCoreGame = this;
		m_pDynamicLoader = this.GetComponent<DynamicLoader>();
		m_pDatabaseManager = this.GetComponent<DatabaseManager>();
		m_pStreamsWriterLoader = this.GetComponent<StreamsWriteLoad>();
	}

	public void DestroyItem (GameObject toDestroy)
	{
		Destroy(toDestroy);
	}
}
