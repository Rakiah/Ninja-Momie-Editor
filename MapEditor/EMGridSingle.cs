using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EMGridSingle : MonoBehaviour
{
	public bool ActivatedGrid;
	public EMMouseHolder MouseParent;
	public EMGridMapper Mapper;
	public Grid GridSingle;
	

	void Start ()
	{
		Mapper = GameObject.Find("Editor Manager").GetComponent<EMGridMapper>();
		if(Mapper.isEditor) MouseParent = GameObject.Find("Editor Manager").GetComponent<EMMouseHolder>();
	}
	

	void Update () 
	{
		if(Mapper.isEditor)
		{
			if(ActivatedGrid)
			{
				renderer.material.color = Color.red;
				MouseParent.CurrentMasterGrid = GridSingle;
			}
			else
			{
				renderer.material.color = Color.black;
			}
		}
	}
}
