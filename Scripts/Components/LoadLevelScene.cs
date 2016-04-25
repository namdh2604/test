using iGUI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class LoadLevelScene : MonoBehaviour 
{
	public iGUIButton _loadScene = null;
	public iGUIButton _addTexture = null;
	public iGUIContainer textureHolder = null;
	private int _numberOfTextures = 0;
	private static string spritePath = "Textures/Results_Characters_Sprite";
	private Dictionary<string, Texture> generatedTextures = new Dictionary<string, Texture>();

	void Start()
	{
		Debug.Log(System.GC.GetTotalMemory(true).ToString());
		_loadScene.clickCallback += ClickHandler;
		_addTexture.clickCallback += ClickHandler;
	}

	void ClickHandler (iGUIElement target)
	{
		if(target == _loadScene)
		{
			SceneManager.LoadScene("CauldronRunes", LoadSceneMode.Additive);
			_loadScene.setEnabled(false);
			StartCoroutine(WaitToClose());
		}
		else if(target == _addTexture)
		{
			CreateNewTexture();
		}
	}

	void CreateNewTexture ()
	{
		iGUIImage newImage = AddAndAssignNameToGUIElement<iGUIImage>(textureHolder, "NEW_IMAGE_" + _numberOfTextures.ToString());
		newImage.scaleMode = ScaleMode.ScaleToFit;
		newImage.setPositionAndSize (new Rect(0.5f,1.0f, (Screen.width * 0.25f), Screen.height));
		//		newImage.setPositionAndSize (new Rect(0.5f,0.5f, 512.0f, 500.0f));
		//		Sprite[] sprites = Resources.LoadAll<Sprite>("Textures/Test/512x500Test");
		int index = UnityEngine.Random.Range(0,4);
		Sprite[] sprites = Resources.LoadAll<Sprite>(spritePath);
		
		if(sprites == null)
		{
			return;
		}
		Texture appliedTexture = null;
		
		if(!generatedTextures.ContainsKey(sprites[index].name))
		{
			Texture newTexture = GetTextureFromSprite(sprites[index]);
			appliedTexture = newTexture;
			generatedTextures[newTexture.name] = newTexture;
		}
		else
		{
			appliedTexture = generatedTextures[sprites[index].name];
		}
		
		//		Texture2D newTexture = Resources.Load<Texture2D> ("Textures/Test/512x500Test");
		//		var colors = new Color32[1];
		//		colors[0] = Color.black;
		//		colors[1] = Color.blue;
		//		colors[2] = Color.cyan;
		//		colors[3] = Color.green;
		
		//		newTexture.SetPixel(512, 500, Color.red);
		//		newTexture.Apply();
		newImage.image = appliedTexture;
		//		int index = UnityEngine.Random.Range (0, _spriteDictionary.Count);
		//		newImage.image = GetTextureFromSprite(_spriteDictionary[index]);
		newImage.order = _numberOfTextures;
		++_numberOfTextures;
		textureHolder.refreshStyle();
	}
	
	private T AddAndAssignNameToGUIElement<T>(iGUIContainer container, string variableName) where T : iGUIElement
	{
		T newElement = container.addElement<T>();
		newElement.name = variableName;
		newElement.setVariableName(variableName);
		
		return newElement;
	}
	
	Texture GetTextureFromSprite(Sprite sprite)
	{
		Texture returnTexture = null;
		int adjustedHeight = Convert.ToInt32(sprite.textureRect.height / 1.2f);
		int adjustedSpriteHeight = Convert.ToInt32(sprite.rect.height / 1.2f);
		int adjustedY = Convert.ToInt32(sprite.rect.height - adjustedSpriteHeight);
		//		Texture2D croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
		Texture2D croppedTexture = new Texture2D((int)sprite.rect.width, adjustedSpriteHeight);
		croppedTexture.hideFlags = HideFlags.None;
		
		Color[] pixels = sprite.texture.GetPixels((int)sprite.textureRect.x, adjustedY, (int)sprite.textureRect.width, adjustedHeight);
		//		Color[] pixels = sprite.texture.GetPixels((int)sprite.textureRect.x, (int)sprite.textureRect.y, (int)sprite.textureRect.width, (int)sprite.textureRect.height);
		croppedTexture.SetPixels(pixels);
		croppedTexture.Apply();
		croppedTexture.name = sprite.name;
		returnTexture = (Texture)croppedTexture;
		
		return returnTexture;
	}

	IEnumerator WaitToClose()
	{
		yield return new WaitForSeconds(1.0f);
		_loadScene.clickCallback -= ClickHandler;
		gameObject.GetComponent<iGUIContainer>().setEnabled(false);
	}
}
