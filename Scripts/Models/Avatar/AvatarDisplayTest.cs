using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using Voltage.Witches.Models.Avatar;
using Voltage.Witches.Unity;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

using Voltage.Witches.Bundles;

public class AvatarDisplayTest : MonoBehaviour
{
    public AvatarGenerator _avatarGenerator;
    public Texture2D _defaultTexture;

    private Dictionary<string, GameObject> _images = new Dictionary<string, GameObject>();

    public Camera renderCamera;
    public Canvas targetCanvas;

    private Outfit _outfit;

    private void Start()
    {
        Outfit outfit = new Outfit() {
                "emerald_tiara_green",
                "witch_hat_black",
                "wavy_long_hair_darkbrown",
                "square_sunglasses_white",
                "pearl_earrings_white",
                "knit_infinity_scarf_teal",
                "faux_leather_purse_teal",
                "fur_lined_coat_lightgreen",
                "bottlecap_bracelet_beige",
                "silky_bathrobe_purple",
                "gloves_black",
                "light_leather_belt_tan",
                "pencil_skirt_maroon",
                "buttonup_blouse_blackwhite",
                "emerald_necklace_green",
                "sporty_tank_top_magenta",
                "witch_dress_black",
                "formal_slacks_khaki",
                "combat_boots_black",
                "capri_pants_grey",
                "ankle_strap_red__heels",
                "ripped_stockings_magenta",
                "floral_lace_underwear_pink",
                "ty_tattoo"
        };

        _outfit = outfit;

        // Test with real asset bundles
        AssetBundleManager assetManager = UnityEngine.Object.FindObjectOfType<AssetBundleManager>();
        var avatarResourceManager = new AvatarResourceManager(assetManager);

//        // Test with loose assets
//        var avatarResourceManager = new EditorAvatarResourceManager();
        _avatarGenerator.Init(avatarResourceManager);
    }

    public void DisplayBot()
    {
		StartCoroutine(_avatarGenerator.DisplayBottom(_outfit, OutfitType.Naked, AvatarType.Fullbody));
    }

    private IEnumerator DisplayBotRoutine()
    {
		yield return StartCoroutine(_avatarGenerator.DisplayBottom(_outfit, OutfitType.Naked, AvatarType.Fullbody));
    }

    public void DisplayTop()
    {
		StartCoroutine(_avatarGenerator.DisplayTop(_outfit, OutfitType.Naked, AvatarType.Fullbody));
    }

    public void DisplayFull()
    {
		StartCoroutine(_avatarGenerator.DisplayAll(_outfit, OutfitType.Default, AvatarType.Fullbody));
    }

	public void Save()
	{
		_avatarGenerator.SaveImage(AvatarType.Fullbody);
	}

    public void UpdateImages()
    {
        StartCoroutine(_avatarGenerator.SaveAll(_outfit));
    }

	public void DisplayAvatarTest()
	{
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
		GameObject go = AvatarStoryImageDisplay.CreateAvatar(OutfitType.Default, "flirty", null);
        go.transform.SetParent(targetCanvas.transform, false);
        sw.Stop();
        UnityEngine.Debug.LogWarning("total time taken to display: " + sw.Elapsed.TotalMilliseconds);
        _images["small"] = go;
	}

    public void DisplayFullScreenAvatar()
    {
        GameObject go = AvatarImageDisplay.CreateAvatar(AvatarType.Fullbody);
        go.transform.SetParent(targetCanvas.transform, false);
    }

    public void DisplayHeadshotAvatar()
    {
        GameObject go = AvatarImageDisplay.CreateAvatar(AvatarType.Headshot);
        go.transform.SetParent(targetCanvas.transform, false);
    }

	public void DisplayAvatarNormal()
	{
		GameObject go = new GameObject("raw avatar");
		var image = go.AddComponent<RawImage> ();

//		WrappedTexture tex = WrappedTexture.Create (TextureFormat.ARGB32, false);
//
//		tex.Texture.LoadImage (File.ReadAllBytes(Application.persistentDataPath + "/dummy.png"));
//		image.texture = tex.Texture;
        image.texture = _defaultTexture;
		image.SetNativeSize();

		RectTransform rt = go.transform as RectTransform;
		rt.localPosition = new Vector3 (-49.0f, -252.25f, 0);
		go.transform.SetParent (transform, false);

        _images["normal"] = go;
	}

    public void DisplayAvatarNew()
    {
        GameObject go = AvatarStoryImageDisplay.CreateAvatar(OutfitType.Default, "angry", null);
        go.transform.SetParent(transform, false);
        _images["small"] = go;
    }

    public void DisplayImage(string type)
    {
        foreach (var image in _images)
        {
            if (image.Key != type)
            {
                image.Value.SetActive(false);
            }
        }

        _images[type].SetActive(true);
    }

}
