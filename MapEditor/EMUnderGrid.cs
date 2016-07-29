using UnityEngine;
using System.Collections;

public class EMUnderGrid : MonoBehaviour 
{
	EMGridSingle MyMasterGrid;

	public int IdentifierUnderGrid;

	public void AssignGrids (EMGridSingle grid, int Id)
	{
		MyMasterGrid = grid;
		IdentifierUnderGrid = Id;
	}
	

	void OnMouseEnter ()
	{
		if (MyMasterGrid.Mapper.isEditor)
		{
			renderer.material.color = Color.red;
			MyMasterGrid.ActivatedGrid = true;
			MyMasterGrid.MouseParent.CurrentUnderGrid = MyMasterGrid.GridSingle.SousGrid[IdentifierUnderGrid];
		}
	}
	void OnMouseExit ()
	{
		if(MyMasterGrid.Mapper.isEditor)
		{
			renderer.material.color = Color.black;
			MyMasterGrid.ActivatedGrid = false;
		}
	}
}
