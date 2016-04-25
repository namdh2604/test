using System;
using Voltage.Witches.UI;
using Voltage.Witches.Shop;
using Voltage.Witches.Configuration;
﻿using System.Collections.Generic;



// Clothing Requirement Dependencies
using Voltage.Witches.Models.MissionRequirements;
using System.Linq;
using Voltage.Witches.Controllers.DressCode;
using Voltage.Witches.Metrics;
using Voltage.Common.Metrics;

// Affinity Requirement Depedencies
using Voltage.Witches.Controllers.Favorability;
using Voltage.Witches.Controllers.Factories;



namespace Voltage.Witches.Controllers
{
    using Voltage.Witches.Screens;
    using Voltage.Witches.Story;
    using Voltage.Story.StoryDivisions;
    using Voltage.Witches.Models;
    using Voltage.Story.Configurations;
    using Voltage.Common.Logging;
    using Voltage.Witches.Exceptions;
    using Voltage.Witches.User;

	using Voltage.Witches.Screens.Dialogues;
	using Voltage.Witches.Bundles;

    public class StoryMapUGUIScreenController : ScreenController
    {
        protected readonly IScreenFactory _screenFactory;
        private readonly IStoryLoaderFacade _storyLoader;
        private readonly ISceneHeaderFactory _headerFactory;
        private readonly ISceneViewModelFactory _sceneFactory;
        protected readonly Player _player;

        protected StoryMapUGUIScreen _screen;
        
		protected UIRibbonController _ribbonController;

        private List<string> _countryArcs;

        private Dictionary<string, List<SceneHeader>> _headerMap;
        private Dictionary<string, List<SceneViewModel>> _countryArcMap;

        private List<SceneHeader> _headers;
        private MasterConfiguration _masterConfig;
        private MasterStoryData _masterStory;

        private readonly INoStaminaController _noStaminaController;
		private readonly IAvatarThumbResourceManager _thumbResourceManager;

        private string _activeArc;
        private static Dictionary<string, string> ARC_MAP;

        // a queue of actions to execute as soon as the screen becomes visible
        private Queue<Action> _queuedActions;

        private readonly IShopDialogueController _shopDialogController;

        // HACK -- REMOVE. Short-term fix to get the dependencies for clothing requirements, etc. When those get moved to their own controller, inject that dependency instead
        private readonly IControllerRepo _repo;

        private readonly FavorabilityMilestoneController _favorabilityController;

        static StoryMapUGUIScreenController()
        {
            ARC_MAP = new Dictionary<string, string>() {
                {"czech", "Prague"},
                {"prologue", "Prologue"},
                {"ireland", "Ireland"},
                {"germany", "Germany"},
                {"salem", "Salem"}
            };
        }

            // FIXME: ThumbResourceManager should be in thew view, not the controller
		public StoryMapUGUIScreenController(ScreenNavigationManager screenManager, IScreenFactory screenFactory, IShopDialogueController shopDialogController,
                                            INoStaminaController noStaminaController, IControllerRepo repo,
		                                    IStoryLoaderFacade storyLoader, ISceneHeaderFactory headerFactory, 
            								ISceneViewModelFactory sceneFactory,  Player player, MasterConfiguration masterConfig, MasterStoryData masterStory, Action<int> storyMapLoaded,
                                            IAvatarThumbResourceManager thumbResourceManager, FavorabilityMilestoneController favorabilityController)
			: base(screenManager)

        {
            _screenFactory = screenFactory;
            _storyLoader = storyLoader;
            _headerFactory = headerFactory;
            _sceneFactory = sceneFactory;
            _player = player;
            _masterStory = masterStory;
            _masterConfig = masterConfig;

            _repo = repo;

			_noStaminaController = noStaminaController;
            _shopDialogController = shopDialogController;
			_thumbResourceManager = thumbResourceManager;
            _favorabilityController = favorabilityController;

            _queuedActions = new Queue<Action>();

            CreateSceneHeaders(_headerFactory);
            RefreshScenes();

            InitializeView();
            // initialize Ribbon must be call after InitizeView since Initialize view is initializing the screen.
            _ribbonController = new UIRibbonController (_player, shopDialogController, _screenFactory, masterConfig);       // would prefer to pass this dependency in or not at all.

            SubscribeToEvents();
            if (storyMapLoaded != null)
            {
                storyMapLoaded(0);
            }
        }

        private HashSet<string> GetAvailableArcs(Player player)
        {
            HashSet<string> availableArcs = new HashSet<string>();

            List<string> playedScenes = new List<string> (player.CompletedScenes);
            playedScenes.AddRange (player.AvailableScenes);

            foreach(string scene in playedScenes)
            {
                string arc = GetArcFromSceneId(scene);
                availableArcs.Add (arc);
            }
            
            return availableArcs;
        }

        protected override IScreen GetScreen()
        {
            if (_screen == null)
            {
                _screen = _screenFactory.GetScreen<StoryMapUGUIScreen>();
            }

            return _screen;
        }

        public override void Dispose()
        {
            UnsubscribeToEvents();

			if (_ribbonController != null) 
			{
				_ribbonController.Dispose();
			}

            // TODO: Not calling the base because the base is calling GetScreen, which will force us to generate another screen then delete and then set to null.
            // We need to find out if we can safely turn all GetScreen to return the screen and not make one if one doesn't exist.
            if (_screen != null)
            {
                _screen.Dispose();
                _screen = null;
            }
        }

        public override void Hide()
        {
            _ribbonController.Hide();
            base.Hide();
        }

        public override void Show()
        {
            base.Show();
            _ribbonController.Show();

            HandleNextQueuedAction();
        }


        protected virtual void InitializeView()
        {
            _screen = GetScreen() as StoryMapUGUIScreen;
            _screen.Init(this);

            List<ArcData> copiedArcs = new List<ArcData>();
            var availableArcs = GetAvailableArcs(_player);

            foreach (var existingArc in _masterStory.Arcs)
            {
                ArcData arcCopy = new ArcData(existingArc);
                arcCopy.Locked = !availableArcs.Contains(existingArc.Country);
                copiedArcs.Add(arcCopy);
            }

//            _screen.SetArcs(_masterStory.Arcs);
            _screen.SetArcs(copiedArcs);

            string arc = GetLatestAvailableArc(_player);
			int index = _countryArcs.IndexOf(arc);
            SetArc(index);
        }

        protected virtual void SubscribeToEvents()
        {
            _screen.HomeButtonPressed += HandleHomeButton;
            _screen.StorySelected += HandleSceneSelected;
            _screen.ArcSelected += HandleArcSelected;

			_ribbonController.OnShopOpen += OnShopOpen;
			_ribbonController.OnShopClosed += OnShopClose;
        }

		private void OnShopOpen()
		{
			MakePassive (true);
		}

		private void OnShopClose()
		{
			MakePassive (false);
		}


        protected virtual void UnsubscribeToEvents()
        {
			if (_screen != null) 
			{
				_screen.HomeButtonPressed -= HandleHomeButton;
				_screen.StorySelected -= HandleSceneSelected;
				_screen.ArcSelected -= HandleArcSelected;
			}

			if (_ribbonController != null) 
			{
				_ribbonController.OnShopOpen -= OnShopOpen;
				_ribbonController.OnShopClosed -= OnShopClose;
			}
        }

        private void HandleHomeButton()
        {
            if (Manager.GetCurrentPath().Contains("/Home"))
            {
                Manager.GoToExistingScreen("/Home");
            }
            else
            {
                // TODO: Need to grab a new home screen controller -- add a home screen factory to this controller to handle this
                throw new System.NotImplementedException();
            }
        }

        // this will enqueue a new arc dialog
        public void TriggerArcAlert(string arc)
        {
            var dialog = _screenFactory.GetDialog<iGUISmartPrefab_ArcUnlockedDialog>();
            dialog.SetUnlockImage(arc);
            (dialog as BaseScreen).Hide(false);
            EnqueueDialog(dialog, HandleArcResponse);
        }

        public void TriggerBookAlert()
        {
            var dialog = _screenFactory.GetDialog<iGUISmartPrefab_NewBookUnlocked>();
            var books = _player.GetBooks();
            dialog.SetBook(books[books.Count - 1]);
            (dialog as BaseScreen).Hide(false);
            EnqueueDialog(dialog, HandleBookResponse);
            
        }

        private void EnqueueDialog(IDialog dialog, Action<int> callback)
        {
            Action<int> wrappedCallback = delegate(int buttonID) {
                callback(buttonID);
                HandleNextQueuedAction();
            };

            _queuedActions.Enqueue(() => { MakePassive(true); dialog.Display(wrappedCallback); });
        }

        private void HandleNextQueuedAction()
        {
            if (_queuedActions.Count > 0)
            {
                Action next = _queuedActions.Dequeue();
                next();
            }
        }

        private void HandleArcResponse(int response)
        {
            MakePassive(false);
        }

        private void HandleBookResponse(int response)
        {
            if ((DialogResponse)response == DialogResponse.OK)
            {
                GoToBookshelf();
            }

            MakePassive(false);
        }

        public override void MakePassive(bool value)
        {
			if (_screen != null) 
			{
				_screen.MakePassive (value);
			}

			_ribbonController.MakePassive(value);		// FIXME: ribboncontroller and how it handles enabling buttons (shop & toggle need to be independent) and need to handle open/close (automatically calls enable toggle)
        }

        public void ShowInformationForScene(LoadStatus status)
        {
            if ((status.lockType & LockType.Favorability) == LockType.Favorability)
            {
                DisplayFavorabilityDialog(status.header);
            }
            else if ((status.lockType & LockType.Clothing) == LockType.Clothing)
            {
                DisplayDressCodeDialog(status.header);
            }
            else if (status.needsStamina)
            {
                ShowNoStaminaDialog();
            }
        }

        private void ShowNoStaminaDialog()
        {
            if ((_screen != null) && (_noStaminaController != null))
            {
                MakePassive(true);
                _noStaminaController.Show(() => MakePassive(false));
            }
        }

        private void ShowStarstoneShop()
        {
            MakePassive(true);
            _shopDialogController.Show(ShopDisplayType.STARSTONES, () => MakePassive(false));
        }

        private void HandleSceneSelected(string scenePath)
        {
            _selectedScenePath = scenePath;

            LoadStatus status = _storyLoader.GetLoadStatus(scenePath);
            if (status.IsReady())
            {
                _screen.MakePassive(true); // Need to make the screen passive so the user can't spam other buttons.  This is only done when going into the story player.
                // ignore errors for now
                Action<Exception> missingErrorHandler = null;
                var loading = DisplayLoadingMarquee ();
                Action<int> loadedCallback = (loading as MainLoadingScreenController).ProcessAction;

                _storyLoader.Load(status.settings, loadedCallback, missingErrorHandler);
            }
            else
            {
                ShowInformationForScene(status);
            }
        }

        private string _selectedScenePath = string.Empty;

        private void HandleSceneSelected(int i)
        {
            var scene = _countryArcMap[_activeArc][i];

            string scenePath = scene.Route + "/" + scene.Arc + "/" + scene.Name;
            HandleSceneSelected(scenePath);
        }

        private ScreenController DisplayLoadingMarquee ()
        {
            MainLoadingScreenControllerFactory factory = _repo.Get<MainLoadingScreenControllerFactory> ();
            MainLoadingScreenController loading = factory.Create ((int)MainLayoutType.LOADING_MARQUEE, null, null);
            loading.Show ();
            return loading;
        }

        private bool IsCurrentlyInAScene {
            get
            {
                return !string.IsNullOrEmpty (_player.CurrentScene);
            }
        }

        private void HandleArcSelected(int i)
        {
            _activeArc = _countryArcs[i];
            _screen.DisplayScenes(_countryArcMap[_activeArc]);
        }

        private string GetFullScenePath(SceneViewModel sceneModel)
        {
            return sceneModel.Route + "/" + sceneModel.Arc + "/" + sceneModel.Name;
        }

        private void CreateSceneHeaders(ISceneHeaderFactory headerFactory)
        {
            _headers = new List<SceneHeader>();

            List<string> allDisplayableScenes = new List<string>(_player.CompletedScenes);
            allDisplayableScenes.AddRange(_player.AvailableScenes);

            foreach (string scene in allDisplayableScenes)
            {
                if (SceneExists(scene))
                {
                    _headers.Add(headerFactory.Create(scene));
                }
                else
                {
                    AmbientLogger.Current.Log(string.Format("'{0}' does not exist, ignoring scene", scene), LogLevel.WARNING);
                }
            }
        }

        private void RefreshScenes()
        {
            CreateArcs();
            AddHeadersToCountryArcs();
        }

        private void CreateArcs()
        {
            _countryArcs = new List<string>();
            _headerMap = new Dictionary<string, List<SceneHeader>>();
            _countryArcMap = new Dictionary<string, List<SceneViewModel>>();
            foreach (var arc in _masterStory.Arcs)
            {
                CountryArc countryArc = new CountryArc(arc);
                _countryArcs.Add(countryArc.Name);
                _headerMap[countryArc.Name] = new List<SceneHeader>();
                _countryArcMap[countryArc.Name] = new List<SceneViewModel>();
            }
        }

        private void AddHeadersToCountryArcs()
        {
            foreach (var scene in _headers)
            {
                var arc = GetBaseArc(scene.Arc);
                if (!_countryArcMap.ContainsKey(arc))
                {
                    throw new WitchesException("Invalid arc: " + arc + " specified in scene: " + scene.Path);
                }

                bool isCompleted = (_player.CompletedScenes.Contains(scene.Path));
                int progress = (_player.CurrentScene == scene.Path) ? _player.CurrentBitProgress : 0;

                SceneViewModel viewScene = _sceneFactory.Generate(scene, isCompleted, progress);
                _headerMap[arc].Add(scene);
                _countryArcMap[arc].Add(viewScene);
            }
        }

        // determines whether or not a given scene is part of the actual story.
        // A scene may not exist/be valid if production switches the story configuration after launch
        private bool SceneExists(string scene)
        {
            return _masterStory.SceneToFileMap.ContainsKey(scene);
        }

        private string GetLatestAvailableArc(Player player)
        {
        	string scene = player.AvailableScenes[0];
            return GetArcFromSceneId(scene);
        }

        private string GetBaseArc(string rawArc)
        {
            string[] split = rawArc.Split(' ');
            string token = (split.Length > 1 ? split[1] : split[0]);

            return ARC_MAP[token.ToLower()];
        }

        private string GetArcFromSceneId(string sceneID)
        {
            string arc = sceneID.Split('/')[1];             // scene format "Nik-Ana Main Story/NA Ireland/Difficult Choices" or "Prologue/Prologue/Salem Spell"
            return GetBaseArc(arc);
        }

        private void SetArc(int arcIndex)
        {
            _activeArc = _countryArcs[arcIndex];
            _screen.SelectArc(arcIndex);
            _screen.DisplayScenes(_countryArcMap[_activeArc]);
        }




        // Clothing Requirements -- separate out into its own controller
        private void DisplayDressCodeDialog (SceneHeader scene)
        {
            ClothingRequirement req = scene.Requirements.Select (x => (x as ClothingRequirement)).Where (y => y.GetType () == typeof(ClothingRequirement)).First ();

            var items = _masterConfig.Avatar_Items_Configuration.Avatar_Items.Values.ToList ();
            var match = items.First<AvatarItemData> (itemData => (itemData.layer_name == req.Piece));
            Voltage.Witches.Configuration.JSON.ItemRawParser parser = new Voltage.Witches.Configuration.JSON.ItemRawParser (_masterConfig);
            var clothing = parser.CreateAvatarItem (match) as Clothing;
            var outfit = _player.GetOutfit ();
            IItemFactory factory = _repo.Get<IItemFactory> ();
            Inventory inventory = new Inventory (factory, _player);

            DressCodeMissionDialogViewModel model = new DressCodeMissionDialogViewModel (clothing, outfit, inventory);
			var dialog = _screenFactory.GetDialog<DressCodeDialogue>();
            dialog.Hide();
			dialog.Init(model, _thumbResourceManager);
            MakePassive(true); // TODO - link the make passive true/false together
            dialog.Display(HandleDressCodeResponse);

            SendStoryMapDresscodeMetric(model.Type, scene, clothing);
        }

        void HandleDressCodeResponse(int answer)
        {
            switch ((DressCodeResponse)answer)
            {
                case DressCodeResponse.CHANGE:
                    GoToCloset ();
                    break;
                case DressCodeResponse.BUY:
                    GoToAvatarShop ();
                    break;
            }

            MakePassive(false);
        }

        private void SendStoryMapDresscodeMetric (DressCodeDialogType type, SceneHeader header, IClothing clothing) // TODO: eventually move out to its own class
        {
            IDictionary<DressCodeDialogType,string> typeToEventMap = new Dictionary<DressCodeDialogType,string>
            {
                { DressCodeDialogType.RESUME, MetricEvent.STORYMAP_DISPLAY_DRESSCODE_RESUME },
                { DressCodeDialogType.CHANGE, MetricEvent.STORYMAP_DISPLAY_DRESSCODE_GETCHANGED },
                { DressCodeDialogType.BUY, MetricEvent.STORYMAP_DISPLAY_DRESSCODE_SHOP },
            };
            
            string metricEvent = typeToEventMap [type]; // can throw exception
            
            IDictionary<string,object> data = new Dictionary<string,object> 
            {
                {"route_id", header.Route},
                {"scene_id", header.Scene},
                {"scene_dress_id", clothing.Name},
            };
            
            AmbientMetricManager.Current.LogEvent (metricEvent, data);
        }

        public void GoToCloset ()
        {
            NewClosetScreenControllerFactory closetScreenFactory = _repo.Get<NewClosetScreenControllerFactory>();
            NewClosetScreenController nextScreen = closetScreenFactory.Create(_player);
            Manager.Add (nextScreen);
        }
        
        public void GoToAvatarShop ()
        {
            AvatarShopScreenControllerFactory shopScreenFactory = _repo.Get<AvatarShopScreenControllerFactory>();
            IScreenController nextScreen = shopScreenFactory.Create();
            Manager.Add (nextScreen);
        }

        private void DisplayFavorabilityDialog(SceneHeader scene)
        {
            IEnumerable<AffinityRequirement> affinityReqs 
                = scene.Requirements.Select(x => x as AffinityRequirement).Where(y => y.GetType () == typeof(AffinityRequirement));
            MakePassive(true);
            _favorabilityController.Display(affinityReqs, scene, HandleFavorabilityResponse);
        }

        void HandleFavorabilityResponse(int answer)
        {
            switch ((MileStoneDialogResponse)answer)
            {
                case MileStoneDialogResponse.CLOSE:
                    MakePassive(false);
                    break;
                case MileStoneDialogResponse.BUY_POTION:
                    ShowStarstoneShop();
                    break;
                case MileStoneDialogResponse.BREW_POTION:
                    GoToBookshelf();
                    break;
                case MileStoneDialogResponse.RESUME:
                    MakePassive(false);
                    HandleSceneSelected(_selectedScenePath);
                    break;
            }
        }

        private void GoToBookshelf()
        {
            IScreenController nextScreen = _repo.Get<BookshelfScreenController>();
			Manager.OpenScreenAtPath(nextScreen, "/Home");		// could also use Manager.ReplaceCurrent...but making it explicit to match path that TracingScreenController expects
        }
    }
}
