
using System;
using System.Collections.Generic;

namespace Unit.Witches.DI.Controllers
{
    using NUnit.Framework;
	using Moq;

	using Voltage.Witches.DI;
	using Voltage.Common.Net;
	using Voltage.Witches.Net;
	using Voltage.Witches.User;
	using Voltage.Story.General;


    [TestFixture]
    public class TestRestorePlayerController
    {

		private Mock<INetworkTimeoutController<WitchesRequestResponse>> _mock_networkCtrl;
		private Mock<IParser<PlayerDataStore>> _mock_parser;
        private Mock<IPlayerWriter> _mock_writer;
//		private Mock<IStartupErrorController> _mock_errorCtrl;

		[SetUp]
		public void Init()
		{
			_mock_networkCtrl = new Mock<INetworkTimeoutController<WitchesRequestResponse>> ();
			_mock_parser = new Mock<IParser<PlayerDataStore>> ();
            _mock_writer = new Mock<IPlayerWriter>();
//			_mock_errorCtrl = new Mock<IStartupErrorController> ();
		}


		private RestorePlayerController CreateController()
		{
            return new RestorePlayerController (_mock_networkCtrl.Object, _mock_parser.Object, _mock_writer.Object);
		}

		[Test]
		public void Constructor()
		{
			var controller = CreateController ();

			Assert.That (controller, Is.TypeOf<RestorePlayerController>());
		}

//		[Ignore("invalid")]
		[Test]
		public void RequestRestore_Success()
		{
			bool succeeded = false;

			Action onSuccess = () => succeeded = true;
			Action onFailure = () => succeeded = false;

			_mock_networkCtrl.Setup (net => net.Send (It.IsAny<string> (), It.IsAny<IDictionary<string,string>> (), It.IsAny<Action<WitchesRequestResponse>> (), It.IsAny<Action<WitchesRequestResponse>>(), It.IsAny<int>())).Callback (onSuccess);

			var controller = CreateController ();

			controller.RequestRestore ("id", "password", onSuccess, onFailure);	// INVALID

			Assert.That (succeeded, Is.True);
		}


//		[Ignore("invalid")]
		[Test]
		public void RequestRestore_Failed()
		{
			bool succeeded = false;
			
//			Action onSuccess = () => succeeded = true;
			Action onFailure = () => succeeded = false;
			
			_mock_networkCtrl.Setup (net => net.Send (It.IsAny<string> (), It.IsAny<IDictionary<string,string>> (), It.IsAny<Action<WitchesRequestResponse>> (), It.IsAny<Action<WitchesRequestResponse>>(), It.IsAny<int>())).Callback (onFailure);	// invalid?
			
			var controller = CreateController ();
			
			controller.RequestRestore ("id", "password", () => {}, () => {});	// INVALID
			
			Assert.That (succeeded, Is.False);
		}


		[Test]
		public void RequestRestore_NullID_Failed()
		{
			bool succeeded = false;
			
			Action onSuccess = () => succeeded = true;
			Action onFailure = () => succeeded = false;

			var controller = CreateController ();
			
			controller.RequestRestore (null, "password", onSuccess, onFailure);
			
			Assert.That (succeeded, Is.False);
		}

		[Test]
		public void RequestRestore_EmptyID_Failed()
		{
			bool succeeded = false;
			
			Action onSuccess = () => succeeded = true;
			Action onFailure = () => succeeded = false;

			var controller = CreateController ();
			
			controller.RequestRestore (string.Empty, "password", onSuccess, onFailure);
			
			Assert.That (succeeded, Is.False);
		}

		[Test]
		public void RequestRestore_NullPassword_Failed()
		{
			bool succeeded = false;
			
			Action onSuccess = () => succeeded = true;
			Action onFailure = () => succeeded = false;
			
			var controller = CreateController ();
			
			controller.RequestRestore ("id", null, onSuccess, onFailure);
			
			Assert.That (succeeded, Is.False);
		}

		[Test]
		public void RequestRestore_EmptyPassword_Failed()
		{
			bool succeeded = false;
			
			Action onSuccess = () => succeeded = true;
			Action onFailure = () => succeeded = false;
			
			var controller = CreateController ();
			
			controller.RequestRestore ("id", string.Empty, onSuccess, onFailure);
			
			Assert.That (succeeded, Is.False);
		}

		[Test]
		public void RequestRestore_SuccessfulRequest_InternalParserCalled()
		{
//			_mock_networkCtrl.Setup(n => n.Send

			var controller = new RestorePlayerController (new StubNetworkController(true), _mock_parser.Object, _mock_writer.Object);

			controller.RequestRestore ("id", "password", () => {}, () => {});

			_mock_parser.Verify(p => p.Parse (It.IsAny<string> ()), Times.Once());
		}

		[Test]
		public void RequestRestore_SuccessfulRequest_InternalSerializerCalled()
		{
//			_mock_networkCtrl.Setup(n => n.Send
			
			var controller = new RestorePlayerController (new StubNetworkController(true), _mock_parser.Object, _mock_writer.Object);
			
			controller.RequestRestore ("id", "password", () => {}, () => {});
			
            _mock_writer.Verify(p => p.Save(It.IsAny<PlayerDataStore>()), Times.Once());
		}

		[Test]
		public void RequestRestore_FailedRequest_InternalParserNotCalled()
		{
//			_mock_networkCtrl.Setup(n => n.Send
		
			var controller = new RestorePlayerController (new StubNetworkController(false), _mock_parser.Object, _mock_writer.Object);
			
			controller.RequestRestore ("id", "password", () => {}, () => {});
			
			_mock_parser.Verify(p => p.Parse (It.IsAny<string> ()), Times.Never());
		}
		
		[Test]
		public void RequestRestore_FailedRequest_InternalSerializerNotCalled()
		{
//			_mock_networkCtrl.Setup(n => n.Send


			var controller = new RestorePlayerController (new StubNetworkController(false), _mock_parser.Object, _mock_writer.Object);
			
			controller.RequestRestore ("id", "password", () => {}, () => {});
			
            _mock_writer.Verify(p => p.Save(It.IsAny<PlayerDataStore>()), Times.Never());
		}


		private class StubNetworkController : INetworkTimeoutController<WitchesRequestResponse>
		{
			private readonly bool _succeeds;

			public StubNetworkController (bool succeeds)
			{
				_succeeds = succeeds;
			}

			public INetworkTransportLayer Send (string url, IDictionary<string,string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout=30)
			{
				if(_succeeds)
				{
					onSuccess(new WitchesRequestResponse());
				}
				else
				{
					onFailure(new WitchesRequestResponse());
				}

				return default(INetworkTransportLayer);
			}

			public INetworkTransportLayer Receive (string url, IDictionary<string,string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout=30)
			{
				return default(INetworkTransportLayer);
			}
		}

    }
}