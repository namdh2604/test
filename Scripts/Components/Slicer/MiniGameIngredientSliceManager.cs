using iGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGameIngredientSliceManager : MonoBehaviour 
{
	enum INGREDIENT_TYPE
	{
		CIRCLE,
		OVAL,
		RECTANGLE,
		RECTANGLE_THIN,
		SQUARE,
		DEFAULT
	}

	enum MOVE_TYPE
	{
		FROM_TOP,
		FROM_BOTTOM,
		DEFAULT
	}

	enum LATERAL_DIRECTION
	{
		MOVINGLEFT,
		MOVINGRIGHT,
		NONE
	}

}
