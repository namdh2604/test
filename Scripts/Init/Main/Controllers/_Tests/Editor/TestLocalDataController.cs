
using System;
using System.Collections.Generic;

namespace Unit.Witches.DI.Controllers
{
    using NUnit.Framework;
	using Moq;

	using Voltage.Witches.DI;
	using Voltage.Witches.Configuration;

    [TestFixture]
    public class TestLocalDataController
    {
        private Mock<ILocalDataFetcher> _mock_fetcher;

		[SetUp]
		public void Init()
		{
            _mock_fetcher = new Mock<ILocalDataFetcher>();
		}

        [Test]
        public void Constructor()
        {
			var controller = new LocalDataController (_mock_fetcher.Object);

			Assert.That (controller, Is.TypeOf<LocalDataController> ());
        }

		[Test]
		public void Execute_FetchSuccess_Callbacks()
		{
			LocalDataController controller = new LocalDataController (new StubCallbackFetcher(true));

//			controller.Start (_mock_action.Object);
//
//			_mock_action.Verify (c => c (), Times.Once());

			bool called = false;

			controller.Execute ((e) => called = true);

			Assert.That (called, Is.True);
		}


		[Test]
		public void Execute_FetchFailed_DoesNotCallback()
		{
			LocalDataController controller = new LocalDataController (new StubCallbackFetcher(false));
			
//			controller.Start (_mock_action.Object);
//
//			_mock_action.Verify (c => c (), Times.Once());
			
			bool called = false;
			
			controller.Execute ((e) => called = true);
			
			Assert.That (called, Is.False);
		}



        private class StubCallbackFetcher : ILocalDataFetcher
		{
			private readonly bool _succeeds;

			public StubCallbackFetcher(bool succeeds)
			{
				_succeeds = succeeds;
			}

			public void Fetch(Action<Exception, LocalData> callback)
			{
				if(_succeeds)
				{
					callback(null, new LocalData());
				}
			}
		}


	
    }
}