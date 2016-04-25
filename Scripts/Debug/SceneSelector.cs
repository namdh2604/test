
using System;
using System.Collections.Generic;

namespace Voltage.Witches.DebugTool
{
	using Voltage.Common.Logging;

	using UnityEngine;
	using Voltage.Common.DebugTool;

	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	using Voltage.Story.StoryDivisions;
	using Voltage.Story.Models.Nodes;
	using Voltage.Story.Models.Nodes.Extensions;

	using Voltage.Story.Configurations;
	using Voltage.Witches.DI;


	public class SceneSelector : MonoBehaviour
    {

//		private IList<string> _routes;
//		private IList<string> _arcs;
//		private IList<string> _scenes;

//		private IList<string> _availableScenes = new List<string>
//		{
//			"",
//			"Rhys-Ty Main Story/RT Salem/Happy Fools",
//			"Rhys-Ty Main Story/RT Salem/Won't Want For Love",
//			"Nik-Ana Main Story/NA Salem/Laying Low",
//			"Nik-Ana Main Story/NA Salem/Legacy",
//			"Nik-Ty Main Story/NT Salem/Carte Blanche",
//			"Nik-Ty Main Story/NT Salem/The New Normal",
//			"Rhys-Ana Main Story/RA Salem/Clear Skies",
//			"Rhys-Ana Main Story/RA Salem/Home Away from Home",
//			"Prologue/Prologue/Mending Luna",
//		};

//		private enum routeMap
//		{
//			NONE=0
//			RT=1,
//			NA=2,
//			NT=3,
//			RA=4,
//			PROLOGUE=5
//		}

		private IList<string> _routeList = new List<string>
		{
			"",
			"Rhys-Ty Main Story",
			"Nik-Ana Main Story",
			"Nik-Ty Main Story",
			"Rhys-Ana Main Story",
			"Prologue",
		};


		private IList<string> _arcSuffixList = new List<string>
		{
			"",
			"Salem",
			"Ireland",
			"Germany",
			"Czech Republic",
			"Prologue",
		};

		private bool _displayToggle;


		private GUIContent[] _routeBoxList;
		private ComboBox _routeBoxControl;

		private GUIContent[] _arcBoxList;
		private ComboBox _arcBoxControl;

		private GUIStyle _listStyle = new GUIStyle();

		private Rect _submitButtonRect = new Rect();
		private Rect _textFieldRect = new Rect ();
		private string selectedScene = string.Empty;

//		private MasterStoryData _storyData;

		private void Awake()
		{
			if(!Debug.isDebugBuild)
			{
//				Destroy(gameObject);
				Destroy (this);
			}
		}

		private void Start()
		{
			_displayToggle = true;

			ConfigureComboBox ();
			ConfigureStyle ();
			InitComboBox ();

//			_storyData = GetStoryData ();
		}

		private void ConfigureComboBox()
		{
			List<GUIContent> routeContent = new List<GUIContent> ();
			foreach(string route in _routeList)
			{
				routeContent.Add(new GUIContent(route));
			}
			
			_routeBoxList = routeContent.ToArray ();


			List<GUIContent> arcContent = new List<GUIContent> ();
			foreach(string arc in _arcSuffixList)
			{
				arcContent.Add(new GUIContent(arc));
			}
			
			_arcBoxList = arcContent.ToArray ();
		}

		private void ConfigureStyle()
		{
			_listStyle.normal.textColor = Color.white; 
			_listStyle.onHover.background = new Texture2D (1, 1);
			_listStyle.hover.background = new Texture2D(2, 2);
			_listStyle.padding.left = 4;
			_listStyle.padding.right = 4;
			_listStyle.padding.top = 4;
			_listStyle.padding.bottom = 4;
		}

		private void InitComboBox()
		{
			float width = 200;
			float height = 50;

			float leftRoute = Screen.width * 0.2f;
			float top = Screen.height * 0.05f;

			float leftArc = leftRoute + width;
			float leftScene = leftArc + width;
			float textWidth = width * 1.5f;

			_routeBoxControl = new ComboBox(new Rect(leftRoute, top, width, height), _routeBoxList[0], _routeBoxList, "button", "box", _listStyle);
			_arcBoxControl = new ComboBox (new Rect (leftArc, top, width, height), _arcBoxList[0], _arcBoxList, "button", "box", _listStyle);
			_textFieldRect = new Rect (leftScene, top, textWidth, height);

			float buttonWidth = 100f;
			_submitButtonRect = new Rect (leftScene + textWidth - buttonWidth, top + height, buttonWidth, height);
		}


		private MasterStoryData GetStoryData()
		{
			string json = Resources.Load<TextAsset>("JSON/STORY/masterData").text;	
			return new MasterStoryDataParser ().Parse (json);
		}



		private void OnGUI()
		{
			GUI.depth = -100;


			if(_routeBoxControl != null && _arcBoxControl != null && DebugAccessor.Instance.Player != null)
			{
				ShowToggle ();

				if(_displayToggle)
				{

					int selectedRoute = _routeBoxControl.Show();
					int selectedArc = _arcBoxControl.Show();

					 selectedScene = GUI.TextField (_textFieldRect, selectedScene, 150);

					if(GUI.Button(_submitButtonRect, "Submit Scene"))
					{
						string route = _routeList[selectedRoute];
						string arc = _arcSuffixList[selectedArc];

						if(route == "Rhys-Ty Main Story")
						{
							arc = "RT " + arc;
						}
						else if (route == "Nik-Ana Main Story")
						{
							arc = "NA " + arc;
						}
						else if (route == "Nik-Ty Main Story")
						{
							arc = "NT " + arc;
						}
						else if (route == "Rhys-Ana Main Story")
						{
							arc = "RA " + arc;
						}

						string scenePath = string.Format("{0}/{1}/{2}", route, arc, selectedScene);


						if(DebugAccessor.Instance.StoryData.SceneToFileMap.ContainsKey(scenePath))
						{
							AmbientLogger.Current.Log (string.Format("SceneSelector >> Setting Scene To '{0}'", scenePath), LogLevel.INFO);
							selectedScene = string.Format("Success '{0}'!", scenePath);

							DebugAccessor.Instance.Player.DebugClearAvailableScenes();	// until DebugPlayer decorator works
                            DebugAccessor.Instance.Player.AddAvailableScene(scenePath);
                            DebugAccessor.Instance.Player.StartScene(scenePath);
						}
						else
						{
							AmbientLogger.Current.Log (string.Format("SceneSelector >> No Scene '{0}'", scenePath), LogLevel.WARNING);
							selectedScene = string.Format("FAILED '{0}' !!!!", scenePath);
						}

					}
				}

			}
		}


		private void ShowToggle()
		{
			if(GUI.Button(new Rect(Screen.width/2, Screen.height*0.9f, 100f, 50f), "Toggle Selector"))
			{
				_displayToggle = !_displayToggle;
			}
		}

    }
    
}










