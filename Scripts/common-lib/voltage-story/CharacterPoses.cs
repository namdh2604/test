using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections;
using Voltage.Witches.AssetManagement;
using Voltage.Witches.Models.Avatar;
using Voltage.Witches.Bundles;

public class CharacterPoses : MonoBehaviour
{
	[SerializeField]
	private string _character, _pose, _outfit, _expression;

	public string Character { get { return _character; } }
	public string Pose { get { return _pose; } }
	public string Outfit { get { return _outfit; } }
	public string Expression { get { return _expression; } }

    public static void DisplayPose(GameObject anchor, string charName, string poseName, string outfitName, string expressionName, ICharacterBundleManager manager)
    {
        removeAllChildren(anchor);

		string posePath = poseName;

        GameObject poseGO = Instantiate(manager.GetAsset<GameObject>(charName, posePath + ".prefab")) as GameObject;
        poseGO.name = "Pose";
        poseGO.transform.SetParent(anchor.transform, false);

        expressionName = TruncateExpression(expressionName);

        GameObject backgroundGO = Instantiate(manager.GetAsset<GameObject>(charName, posePath + "/Background.prefab")) as GameObject;
        backgroundGO.name = "Background";
        backgroundGO.transform.SetParent(poseGO.transform, false);

        GameObject outfitBotGO = Instantiate(manager.GetAsset<GameObject>(charName, posePath + "/OutfitsBot/" + outfitName + ".prefab")) as GameObject;
        outfitBotGO.name = outfitName + "Bottom";
        outfitBotGO.transform.SetParent(poseGO.transform, false);

        GameObject expressionGO = Instantiate(manager.GetAsset<GameObject>(charName, posePath + "/Expressions/" + expressionName + ".prefab")) as GameObject;
        expressionGO.name = expressionName;
        expressionGO.transform.SetParent(poseGO.transform, false);

        GameObject outfitTopGO = Instantiate(manager.GetAsset<GameObject>(charName, posePath + "/OutfitsTop/" + outfitName + ".prefab")) as GameObject;
        outfitTopGO.name = outfitName + "Top";
        outfitTopGO.transform.SetParent(poseGO.transform, false);
    }

    public void SetPoseInfo(string charName, string poseName, string outfitName, string expressionName, ICharacterBundleManager manager, AvatarResourceManager avatarManager)
	{
		_character = charName;
		_pose = poseName;
		_outfit = outfitName;
		_expression = expressionName;

		RefreshDisplay(manager, avatarManager);
	}

    protected void RefreshDisplay(ICharacterBundleManager manager, AvatarResourceManager avatarManager)
	{
		DisplayPose(_character, _pose, _outfit, _expression, manager, avatarManager);
	}

    public void DisplayPose(string charName, string poseName, string outfitName, string expressionName, ICharacterBundleManager manager, IAvatarResourceManager avatarManager)
    {
        removeAllChildren(gameObject);

        if (AvatarNameUtility.IsAvatarName(charName))
        {
            DisplayAvatar(outfitName, expressionName, avatarManager);
        }
        else
        {
            DisplayNormalCharacter(charName, poseName, outfitName, expressionName, manager);
        }
    }

    private void DisplayAvatar(string outfitName, string expression, IAvatarResourceManager resourceManager)
    {
        Dictionary<string, OutfitType> outfitMapping = new Dictionary<string, OutfitType>() {
            { "default", OutfitType.Default },
            { "naked", OutfitType.Naked }
        };

        OutfitType outfitType = outfitMapping[outfitName.ToLower()];


        GameObject avatarGO = AvatarStoryImageDisplay.CreateAvatar(outfitType, expression, resourceManager);
        avatarGO.transform.SetParent(transform, false);
    }

    private void DisplayNormalCharacter(string charName, string poseName, string outfitName, string expressionName, ICharacterBundleManager manager)
    {
        string posePath = poseName;

        GameObject poseGO = Instantiate(manager.GetAsset<GameObject>(charName, posePath + ".prefab")) as GameObject;
        poseGO.name = "Pose";
        poseGO.transform.SetParent(transform, false);

        GameObject backgroundGO = Instantiate(manager.GetAsset<GameObject>(charName, posePath + "/Background.prefab")) as GameObject;
        backgroundGO.name = "Background";
        backgroundGO.transform.SetParent(poseGO.transform, false);

        GameObject outfitBotGO = Instantiate(manager.GetAsset<GameObject>(charName, posePath + "/OutfitsBot/" + outfitName + ".prefab")) as GameObject;
        outfitBotGO.name = outfitName + "Bottom";
        outfitBotGO.transform.SetParent(poseGO.transform, false);

        GameObject expressionGO = Instantiate(manager.GetAsset<GameObject>(charName, posePath + "/Expressions/" + expressionName + ".prefab")) as GameObject;
        expressionGO.name = expressionName;
        expressionGO.transform.SetParent(poseGO.transform, false);

        GameObject outfitTopGO = Instantiate(manager.GetAsset<GameObject>(charName, posePath + "/OutfitsTop/" + outfitName + ".prefab")) as GameObject;
        outfitTopGO.name = outfitName + "Top";
        outfitTopGO.transform.SetParent(poseGO.transform, false);
    }

    public void HideCharacter()
    {
        gameObject.SetActive(false);
    }

    public static string TruncateExpression(string expression)
    {
        int index = expression.LastIndexOf('_');
        if (index == -1)
        {
            return expression;
        }

        return expression.Substring(0, index);
    }

    private void DisplayChildren(JToken root, GameObject parent, string basePath)
    {
        foreach (var child in root.Children())
        {
            string name = child.Value<string>("name");
            JToken coords = child["absolute"];
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent.transform);
            RectTransform rt = go.AddComponent<RectTransform>();
            rt.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            rt.localRotation = Quaternion.identity;

            rt.anchoredPosition = new Vector2(coords.Value<float>("x"), coords.Value<float>("y"));
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, coords.Value<float>("width"));
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, coords.Value<float>("height"));

            bool isImage = child.Value<bool>("isImage");
            if (isImage)
            {
                Image image = go.AddComponent<Image>();
                image.sprite = Resources.Load<Sprite>(basePath + name);
            }
        }
    }

    private static void removeAllChildren(GameObject go)
    {
        foreach (Transform child in go.transform)
        {
			if (Application.isEditor && !Application.isPlaying)
			{
				GameObject.DestroyImmediate(child.gameObject);
			}
			else
			{
	            GameObject.Destroy(child.gameObject);
			}
        }
    }
	
}
