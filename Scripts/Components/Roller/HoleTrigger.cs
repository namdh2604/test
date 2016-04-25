using iGUI;
using UnityEngine;
using System.Collections;

public class HoleTrigger : MonoBehaviour
{
	private MiniGameRollerManager _manager = null;

	void Start()
	{
		iGUIRoot root = iGUIRoot.instance;
		for(int i = 0; i < root.itemCount; ++i)
		{
			if(root.items[i].name.Contains ("Minigame_Container"))
			{
				_manager = root.items[i].gameObject.GetComponent<MiniGameRollerManager>();
			}
		}
	}

	void TriggerEndLevel ()
	{
		Debug.Log ("Ingredient is in the hole, level complete!");
		_manager.ResetTheMiniGame();
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Ingredient")
		{
			TriggerEndLevel();
		}
	}
}
