using UnityEngine;
using System;
using System.Collections.Generic;

namespace Voltage.Witches.StoryMap
{
	using Voltage.Story.General;
	using Voltage.Story.Configurations;


    public class ArcButtonFactory : MonoBehaviour, IFactory<ArcData, ArcButton>
    {
		[SerializeField]
		private GameObject _buttonPrefab;

		[SerializeField]
		private List<Sprite> _normalIcons;
		[SerializeField]
		private List<Sprite> _highlightIcons;

		private const string SPRITE_NAME_FORMAT = "icon_{0}_{1}";


		private void Awake()
		{
			if(_buttonPrefab == null || _normalIcons == null || _normalIcons.Count == 0 || _highlightIcons == null || _highlightIcons.Count == 0)
			{
				throw new NullReferenceException();
			}
		}

		public ArcButton Create(ArcData arc)
		{
			GameObject button = Instantiate(_buttonPrefab) as GameObject;	// could use factory
			ArcButton arcButton = button.GetComponent<ArcButton> ();
			
			if(arcButton != null)
			{
				arcButton.SetName (GetName(arc));
				arcButton.SetImages(GetSprite(arc.ImageName, Type.NORMAL), GetSprite(arc.ImageName, Type.HIGHLIGHT));
				arcButton.HighlightButton(false);
				arcButton.EnableButton(true);
//				arcButton.OnButtonClick

				arcButton.transform.localPosition = Vector3.zero;

				return arcButton;
			}
			else
			{
				throw new NullReferenceException("No ArcButton Component");
			}
		}

		private string GetName(ArcData arc)
		{
			return (!string.IsNullOrEmpty(arc.CountryAlias) ? arc.CountryAlias : arc.Country);
		}

		private Sprite GetSprite(string arc, Type type)		// note great...relies on naming convention 'icon_arc_regular' and the sprite existing in list
		{
			string suffix = (type == Type.NORMAL ? "regular" : "selected");
			string spriteName = string.Format (SPRITE_NAME_FORMAT, arc.ToLower(), suffix);

			List<Sprite> spriteList = (type == Type.NORMAL ? _normalIcons : _highlightIcons);

			return spriteList.Find (sprite => sprite.name == spriteName);	// can throw exception if not in list
		}

		private enum Type
		{
			NORMAL,
			HIGHLIGHT
		}


    }

}


