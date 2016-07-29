using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DatabaseManager : MonoBehaviour 
{
	public string URL = "http://Rakiah.com/MY_SECRET_PATH";
	public void UploadDatabase (string mapName, string data)
	{
		WWWForm form = new WWWForm();
		form.AddField("MapName",mapName);
		form.AddField("Data", data);
		form.AddField("Security", "MY_SECRET_CODE");

		WWW request = new WWW(URL + "MY_SECRET_SCRIPT_TO_UPLOAD_MAP", form);

		StartCoroutine(UploadDatabaseAnswer(request));
	}

	public void GetCommunityMap ()
	{
		WWW request = new WWW(URL + "MY_SECRET_SCRIPT_TO_GET_MAPS");

		StartCoroutine(GetCommunityMapAnswer(request));
	}

	public void GetSpecificMap (int databaseID)
	{
		WWWForm form = new WWWForm();
		form.AddField("MapID",databaseID);

		WWW request = new WWW(URL + "MY_SECRET_SCRIPT_TO_GET_MAP",form);

		StartCoroutine(GetSpecificMapAnswer(request));
	}

	IEnumerator UploadDatabaseAnswer (WWW request)
	{
		yield return request;

		if (request.error == null) GameObject.Find("Main Camera").GetComponent<MainMenu>().RefreshCommunityMaps();
		else Debug.Log(request.error);
	}


	IEnumerator GetCommunityMapAnswer (WWW request)
	{
		yield return request;

		if (request.error == null)
		{
			string [] ParsedMaps = request.text.Split('#');

			for (int i = 0; i < ParsedMaps.Length - 1; i++)
			{
				string [] parseIDName = ParsedMaps[i].Split('_');
				int ID = 0;
				try
				{
					int.TryParse(parseIDName[0], out ID);
					CoreGame.m_pCoreGame.CommunityMaps.Add(new Map (ID, parseIDName[1]));
				}
				catch
				{
					Debug.Log("ERROR PARSING MAP : " + ID.ToString());
				}
			}
			GameObject.Find("Main Camera").GetComponent<MainMenu>().DrawCommunityMaps();
		}
		else Debug.Log(request.error);
	}


	IEnumerator GetSpecificMapAnswer (WWW request)
	{
		yield return request;

		if (request.error == null)
		{
			if (request.text.Length > 0)
			{
				string [] parsedMap = request.text.Split('_');
				string mapName = CoreGame.m_pCoreGame.m_pStreamsWriterLoader.CreateMap(parsedMap[0]);
				CoreGame.m_pCoreGame.m_pStreamsWriterLoader.SaveMap(mapName,parsedMap[1]);

				GameObject.Find("Main Camera").GetComponent<MainMenu>().RefreshOwnEditMaps();
			}
			else Debug.Log("Error 0 characters length request");
		}
		else Debug.Log("Error with get specific map");
	}

}
