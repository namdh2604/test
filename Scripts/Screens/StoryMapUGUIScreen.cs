using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Voltage.Witches.Screens
{
    using Voltage.Witches.Controllers;
    using Voltage.Witches.StoryMap;
    using Voltage.Witches.UI;
    using Voltage.Witches.Models;
    using Voltage.Witches.Utilities;
    using Voltage.Story.Configurations;

    public class StoryMapUGUIScreen : BaseUGUIScreen
    {
        private StoryMapUGUIScreenController _controller;
        public Button _button;

        [SerializeField]
        private GameObject _sceneCardPrefab;

        [SerializeField]
        private ScrollView _scrollView;

        [SerializeField]
        private Button _homeButton;

        [SerializeField]
        private ArcDisplayPanelView _arcView;

        public event Action HomeButtonPressed;
        public event Action<int> StorySelected;
        public event Action<int> ArcSelected;

        private List<SceneCardView> _sceneViews;

        private void Awake()
        {
            InitializeButtonHandlers();
        }

        private void InitializeButtonHandlers()
        {
            _homeButton.onClick.AddListener(HandleHomeButton);
        }

        public void Init(StoryMapUGUIScreenController controller)
        {
            _controller = controller;
        }

        protected override IScreenController GetController()
        {
            return _controller;
        }

        public virtual void MakePassive(bool value)
        {
            _homeButton.enabled = !value;
            _arcView.MakePassive(value);
            _scrollView.MakePassive(value);

            foreach (var sceneCard in _sceneViews)
            {
                sceneCard.enabled = !value;
            }
        }

        private void HandleHomeButton()
        {
            if (HomeButtonPressed != null)
            {
                HomeButtonPressed();
            }
        }

        // NOTE -- this interface is a bit deceiving. This could be called multiple times, and rather than replacing the current arcs
        // with those specified, it would just continue to add to the collection of them.
        public void SetArcs(List<ArcData> arcs)
        {
            foreach (var arc in arcs)
            {
                _arcView.Add(arc);
            }

            int i = 0;
            foreach (var button in _arcView.GetButtons())
            {
                button.OnButtonClick += HandleArcSwitch(i);
                button.EnableButton(!arcs[i].Locked);
                i++;
            }
        }


        public void DisplayScenes(List<SceneViewModel> scenes)
        {
            _scrollView.Clear();
            _sceneViews = new List<SceneCardView>();

            List<SceneViewModel> orderedScenes = new List<SceneViewModel>(scenes);
            orderedScenes.Reverse(); // Reverse the list, because the scroll views focuses on the first item in the list, not the last one

            int i = orderedScenes.Count - 1; // reversed list, so count down
            foreach (var scene in orderedScenes)
            {
                GameObject sceneCard = PrefabHelper.Instantiate(_sceneCardPrefab) as GameObject;
                SceneCardView cardView = sceneCard.GetComponent<SceneCardView>();
                cardView.SetTitle(scene.Name);
                cardView.SetDescription(scene.Description);

                if (scene.Completed)
                {
                    cardView.SetCardState(SceneStatus.COMPLETED);
                }
                else if (scene.LockStatus != Voltage.Story.StoryDivisions.LockType.None)
                {
                    cardView.SetCardState(SceneStatus.LOCKED);
                }
                else
                {
                    cardView.SetCardState(SceneStatus.READABLE);
                }

                Texture2D polaroid = Resources.Load<Texture2D>(scene.PolaroidPath);
                cardView.SetPolaroidPicture(polaroid);
                cardView.OnButtonSelected += HandleButtonPress(i);

                _sceneViews.Add(cardView);
                _scrollView.AddElement(sceneCard);
                i--;
            }
        }

        private EventHandler HandleButtonPress(int i)
        {
            return (e, source) => RespondToSceneSelect(i);
        }

        private void RespondToSceneSelect(int i)
        {
            if (StorySelected != null)
            {
                StorySelected(i);
            }
        }

        public void SelectArc(int arcIndex)
        {
            int i = 0;
            foreach (ArcButton button in _arcView.GetButtons())
            {
                bool enable = (i == arcIndex);
                button.HighlightButton(enable);
                ++i;
            }
        }

        private EventHandler HandleArcSwitch(int i)
        {
            return (e, source) => RespondToArcButton(i);
        }

        private void RespondToArcButton(int arcIndex)
        {
            // Toggle all arc buttons
            SelectArc(arcIndex);

            // fire off the event to any listeners
            if (ArcSelected != null)
            {
                ArcSelected(arcIndex);
            }
        }



		public SceneCardView GetSceneCardAt(int index)
		{
			return _scrollView.GetElementAt (index).GetComponent<SceneCardView> ();
		}




    }
}

