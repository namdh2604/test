using System;

namespace Voltage.Witches.Controllers
{
    using Voltage.Witches.Services;
    // no interfaces for now -- this currently houses just logic specific to this screen.  No general dialogue stuff is yet handled
    public class OptionsDialogController
    {
        IBuildNumberService _versionService;

        public OptionsDialogController(IBuildNumberService versionService)
        {
            if (versionService == null)
            {
                throw new ArgumentException("versionService cannot be null", "versionService");
            }

            _versionService = versionService;
        }

        public string GetBuildVersion()
        {
            return _versionService.GetBuildVersion();
        }
    }
}

