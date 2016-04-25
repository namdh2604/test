using UnityEngine;
using UnityEngine.UI;
using Voltage.Witches.Layout;
using Voltage.Witches.AssetManagement;
using Voltage.Witches.Bundles;

using System.IO;

public class CharacterDisplayController : MonoBehaviour
{
    public CharacterPoses _leftPose;
    public CharacterPoses _rightPose;

	private ICharacterBundleManager _assetManager;
    private IAvatarResourceManager _avatarResourceManager;

    public enum CharacterPosition
    {
        Left = 0,
        Right
    }

	//HACK For some reason, these scripts keep getting ditched on push, so trying to ensure they are added to ther necessary objects
	void Awake()
	{
		if(_leftPose.gameObject.GetComponent<CharacterPoses>() == null)
		{
			_leftPose.gameObject.AddComponent<CharacterPoses>();
		}
		if(_rightPose.gameObject.GetComponent<CharacterPoses>() == null)
		{
			_rightPose.gameObject.AddComponent<CharacterPoses>();
		}
	}

	void Start()
	{
		DialogueNodeDisplay display = gameObject.GetComponentInParent<DialogueNodeDisplay>();
		_assetManager = display._assetManager;
        _avatarResourceManager = display._avatarResourceManager;
	}

	//HACK This is using the temporary sprite loader class, only here to get the build showing sprites
//	public void DisplayCharacter(CharacterPosition position, string name, string expression)
//	{
//		Debug.Log("Character Display DisplayCharacter() called");
//		if(string.IsNullOrEmpty(name))
//		{
//			Debug.Log("Name is null, but has pos " + position.ToString());
////			if(position != null)
////			{
//				name = "Avatar";
////			}
//		}
//
//		CharacterPoses characterDisplay = (position == CharacterPosition.Left) ? _leftPose : _rightPose;
//		Debug.Log((characterDisplay == null).ToString() + " : characterDisplay is NULL");
//		if(characterDisplay != null)
//		{
//			characterDisplay.gameObject.SetActive(true);
//		}
//		if(TemporaryCharacterSpriteLoader.SpriteLoader != null)
//		{
//			var characterBank = TemporaryCharacterSpriteLoader.SpriteLoader;
//			if(name == "USER_FIRST")
//			{
//				name = name.Replace("USER_FIRST","Avatar");
//			}
//			if(!string.IsNullOrEmpty(expression))
//			{
//				expression = expression.ToLower();
//			}
//
//			GameObject character = characterBank.GetCharacter(name,expression);
//			character.GetComponent<RectTransform>().SetParent(characterDisplay.GetComponent<RectTransform>(),false);
//		}
//		else
//		{
//			Debug.Log("Problem loading " + name);
//		}
//	}

    public void DisplayCharacter(CharacterPosition position, string character, string pose, string outfit, string expression)
    {
        CharacterPoses characterDisplay = (position == CharacterPosition.Left) ? _leftPose : _rightPose;
	    characterDisplay.gameObject.SetActive(true);
		characterDisplay.DisplayPose(character, pose, outfit, expression, _assetManager, _avatarResourceManager);
    }

    public void HideCharacter(CharacterPosition position)
    {
        CharacterPoses characterDisplay = (position == CharacterPosition.Left) ? _leftPose : _rightPose;
      	characterDisplay.HideCharacter();
//		Debug.Log("Hide " + characterDisplay.Character);
    }
}

