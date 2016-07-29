using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

//this class make you load and write everything on plain TXT

public class StreamsWriteLoad : MonoBehaviour 
{
	public string Folder = "/Maps";
	public string extension = ".mnm";

	string fullPath;

	void Start () 
	{
		fullPath = Application.dataPath + Folder;
	}


	public List<Map> LoadMaps ()
	{
		List<Map> MapOnFolder = new List<Map>();
		DirectoryInfo di = Directory.CreateDirectory(fullPath);
		
		FileInfo[] mnmFiles = di.GetFiles("*" + extension);
		if (mnmFiles.Length == 0)
			Debug.Log("no files present");
		
		foreach (FileInfo fi in mnmFiles)
		{
			string MapName = fi.Name.Remove(fi.Name.Length - extension.Length);
			string data = File.ReadAllText(fi.FullName);
			MapOnFolder.Add(new Map(MapName, data));
		}

		return MapOnFolder;
	}

	public string CreateMap (string mapName)
	{
		//we say, if the file already exist, check for something that do not, with as much numbers as needed and if you find something free, stop looping and create the file
		if(File.Exists(fullPath + "/" + mapName + extension))
		{
			int i = 0;
			for(i = 0; i < 999999; i++)
			{
				if (!File.Exists(fullPath + "/" + mapName + i.ToString() + extension))
				{
					FileStream MapFile = File.Create(fullPath + "/" + mapName + i.ToString() + extension);
					MapFile.Close();
					CoreGame.m_pCoreGame.m_pDynamicLoader.LoadedMap = new Map (mapName + i.ToString(),"");
					CoreGame.m_pCoreGame.CurrentMap = mapName + i.ToString();
					
					return mapName + i.ToString();
				}
			}
			Debug.LogError("too much files already..");
			return "-1";
		}
		else
		{
			FileStream MapFile = File.Create(fullPath + "/" + mapName + extension);
			MapFile.Close();
			CoreGame.m_pCoreGame.m_pDynamicLoader.LoadedMap = new Map (mapName,"");
			CoreGame.m_pCoreGame.CurrentMap = mapName;

			return mapName;
		}
	}
	
	public void SaveMap (string mapName, string data)
	{
		if(File.Exists(fullPath + "/" + mapName + extension))
		{
			File.WriteAllText(fullPath + "/" + mapName + extension, data);
		}
		else
		{
			Debug.Log("no maps with this name found");
		}
	}


	public void DeleteMap (string mapName)
	{
		if(File.Exists(fullPath + "/" + mapName + extension))
		{
			File.Delete(fullPath + "/" + mapName + extension);
		}
		else
		{
			Debug.Log("tried to delete smth that doesnt exist.");
		}
	}

}






