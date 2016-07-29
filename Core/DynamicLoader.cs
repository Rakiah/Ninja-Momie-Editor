using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//this code make you show the GUI and load scenes
public class DynamicLoader : MonoBehaviour 
{
	[System.NonSerialized] public Map LoadedMap;

	[System.NonSerialized] public int GridDraw = 1600;
	[System.NonSerialized] public int ALLOBJProg = 0;

	[System.NonSerialized] public float progress;
	float PercentageToAdd;

	[System.NonSerialized] public bool IsLoading;

	public Texture2D EmptyBar;
	public Texture2D FullBar;
	
	public Rect BarProgression;

	public GUISkin skin;
	
	
	public float NativeWidth = 1920;
	public float NativeHeight = 1080;
	
	public List<string> Tips = new List<string>();
	public List<Icon> AdditionalUI = new List<Icon>();
	public List<Texte> AdditionalText = new List<Texte>();

	int RandomTip;

	public void LoadOne ()
	{
		progress += PercentageToAdd;
		if(progress >= 99f)
		{
			IsLoading = false;
		}
	}

	void OnGUI ()
	{
		if(IsLoading)
		{
			float rx = Screen.width / NativeWidth;
			float ry = Screen.height / NativeHeight;
			GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3(rx, ry, 1));

			if (progress > 99f) progress = 100.0f;
			int progressInt = (int)progress;


			AdditionalText[0].TextToDisplay = Tips[RandomTip];
			
			for (int j = 0; j < AdditionalUI.Count; j++)
				AdditionalUI[j].Draw();
			for (int i = 0; i < AdditionalText.Count; i++)
				AdditionalText[i].Draw(skin);

			if(EmptyBar)
			{
				GUI.DrawTexture(new Rect(BarProgression.x, BarProgression.y, BarProgression.width * (progress / 100.0f), BarProgression.height), FullBar);
				GUI.DrawTexture(BarProgression,EmptyBar);
				GUI.Label(new Rect((BarProgression.x),(BarProgression.y), BarProgression.width,BarProgression.height),progressInt+ " %", skin.customStyles[0]);
			}
		}
	}

	public void LoadALevel (int level)
	{
		Application.LoadLevel(level);
	}


	void OnLevelWasLoaded (int levelLoaded)
	{
		if (levelLoaded > 0)
		{
			RandomTip = Random.Range(0,Tips.Count - 1);
			ALLOBJProg = 0;
			progress = 0.0f;
			PercentageToAdd = 0;


			IsLoading = true;
			ALLOBJProg += GridDraw;
			ALLOBJProg += OBJOnMap();
			PercentageToAdd = (100.0f / ALLOBJProg);
		}
	}
	
	
	int OBJOnMap ()
	{
		string [] OBJMap = LoadedMap.UnparsedMap.Split('&');

		return OBJMap.Length - 1;
	}
}

[System.Serializable]
public class Icon
{
	public string NameElement;
	public Texture2D texture;
	public Rect RectPosition;
	
	
	public void Draw ()
	{
		if(texture) GUI.DrawTexture(RectPosition,texture);
	}
}

[System.Serializable]
public class Texte
{
	public string NameElement;
	public string TextToDisplay;
	public bool UseSkin;
	public Rect RectPosition;
	
	
	public void Draw (GUISkin skin)
	{
		if(TextToDisplay != null)
		{
			if(UseSkin) GUI.Label(RectPosition,TextToDisplay,skin.label);
			else GUI.Label(RectPosition,TextToDisplay);
		}
	}
}


