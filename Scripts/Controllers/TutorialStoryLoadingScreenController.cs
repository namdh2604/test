using Voltage.Witches.Screens;


namespace Voltage.Witches.Controllers
{
    public class TutorialStoryLoadingScreenController
    {
        private readonly IScreenFactory _factory;
        private TutorialStoryLoadingDialog _screen;

        public bool Visible { get; protected set; }
        public bool ShowBG { get; set; }
        public bool ShowText { get; set; }

        public TutorialStoryLoadingScreenController(IScreenFactory factory)
        {
            _factory = factory;
            ShowBG = true;
            ShowText = true;
        }

        public void Show()
        {
            if (_screen == null)
            {
                _screen = _factory.GetDialog<TutorialStoryLoadingDialog>();
                if (!ShowBG)
                {
                    _screen.loading_bg.enabled = false;
                }
                if (!ShowText)
                {
                    _screen.prologue_text.enabled = false;
                }
            }
            _screen.Show();

            Visible = true;
        }

        public void Hide()
        {
            if (_screen != null)
            {
                _screen.Hide();
            }
            Visible = false;
        }

        public void Dispose()
        {
            if (_screen != null)
            {
                _screen.Dispose();
                _screen = null;
            }
            Visible = false;
        }
    }
}

