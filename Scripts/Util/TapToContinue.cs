using UnityEngine;
using System.Collections;
using iGUI;
using Voltage.Witches.Controllers;

using Voltage.Common.Logging;

public class TapToContinue : MonoBehaviour
{
	private IEnumerator _prompt;
	public iGUIContainer tapToContinue;

	public void Start()
	{
		_prompt = CountDownToPrompt(1f); 
		StartCoroutine(_prompt);
	}

	IEnumerator CountDownToPrompt(float baseTime)
	{
		var time = baseTime;
		while(time > 0f)
		{
			yield return new WaitForSeconds(1f);
			time -= 1f;
		}

		DisplayPrompt();
	}

	public void DisplayPrompt()
	{	
		_prompt = DisplayPromptRoutine ();
		StartCoroutine (_prompt);
	}
	
	IEnumerator DisplayPromptRoutine ()
	{
		while (true && tapToContinue != null) 
		{
			// SETUP
			tapToContinue.setOpacity (0f);
			tapToContinue.setEnabled (true);

			// PHASE 1
			tapToContinue.fadeTo (1f, 0.5f, iTweeniGUI.EaseType.linear);
			tapToContinue.scaleTo (1.5f, 1.5f, iTweeniGUI.EaseType.easeOutElastic);

			yield return new WaitForSeconds (1.5f);

			// PHASE 2
			tapToContinue.fadeTo (0f, 0.5f, iTweeniGUI.EaseType.linear);
			tapToContinue.scaleTo (1f, 0.5f, iTweeniGUI.EaseType.easeOutElastic);
			yield return new WaitForSeconds (1f);

			// TEARDOWN
			tapToContinue.setEnabled (false);
		}
	}
	
	public void KillPrompt ()
	{
		if (_prompt != null) {
			StopCoroutine (_prompt);
			_prompt = null;
            if (tapToContinue != null)
            {
                StopiTween (tapToContinue);
                tapToContinue.setScale (1f);
                tapToContinue.setOpacity (0f);
                tapToContinue.setEnabled (false);
            }
		}
	}
	
	void StopiTween (iGUIElement element)
	{
		if(element != null && element.gameObject != null)
		{
			iTweeniGUI.Stop (element.gameObject);
		}
		else
		{
			AmbientLogger.Current.Log ("TapToContinue StopiTween element is Null!", LogLevel.WARNING);
		}
	}
}
