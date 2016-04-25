
using System;
using System.Collections.Generic;

namespace Voltage.Witches.DI
{
	using Voltage.Story.General;
	using Voltage.Witches.Net;
	using Voltage.Common.Android.ExpansionFile;


	public class AndroidEnvironmentController : EnvironmentController	// TODO: will need to refactor to use AndroidEnvironmentData!
	{
		private readonly IFactory<string,IExpansionFileFetcher> _expansionFetcherFactory;
		
		public AndroidEnvironmentController(IFactory<string,IExpansionFileFetcher> expansionFetcherFactory, 
		                                    EnvironmentDataFetcher dataFetcher, IBaseUrl networkController, IStartupErrorController errorController)
											: base (dataFetcher, networkController, errorController) 
		{
			if(expansionFetcherFactory == null)
			{
				throw new ArgumentNullException();
			}

			_expansionFetcherFactory = expansionFetcherFactory;
		}
		
		protected override void OnFetch(Exception e, EnvironmentData data, Action<Exception> onComplete)
		{
            if (e != null)
            {
                onComplete(e);
                return;
            }

			Action<Exception> onCompletedDownload = (excep) => base.OnFetch(excep, data, onComplete);

			IExpansionFileFetcher obbFetcher = _expansionFetcherFactory.Create(data.OBBPath);
			obbFetcher.Fetch(onCompletedDownload);
		}
	}
    
}




