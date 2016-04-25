//
//using System;
//using System.Collections.Generic;
//
//namespace Unit.Common.Net
//{
//    using NUnit.Framework;
//	using Moq;
//
//	using Voltage.Witches.Net;
//	using Voltage.Common.Net;
//
//
//    [TestFixture]
//    public class TestWitchesNetworkRetryController
//    {
//
//		private Mock<INetworkTimeoutController<NetworkPayload>> _mockDecoratedNetworkController;
//		private Mock<INetworkPayload> _mockNetworkPayload;
//
//		[SetUp]
//		public void Init()
//		{
//			_mockDecoratedNetworkController = new Mock<INetworkTimeoutController<INetworkPayload>> ();
//			_mockNetworkPayload = new Mock<INetworkPayload> ();
//		}
//
//
//		private INetworkRetryController<INetworkPayload> CreateController(int attempts=3)
//		{
//			return new WitchesNetworkRetryController<INetworkPayload>(_mockDecoratedNetworkController.Object, attempts);
//		}
//
//        [Test]
//        public void Constructor()
//        {
//			var controller = CreateController ();
//
//			Assert.That (controller, Is.InstanceOf<INetworkController<INetworkPayload>> ());
//        }
//
//		[Test]
//		public void Send_Attempts_ProperRetryCount()
//		{
//			Action<INetworkPayload>> onFail = ((payload) => Console.WriteLine("failed"));
//			_mockDecoratedNetworkController.Setup (c => c.Send (It.IsAny<string> (), It.IsAny<IDictionary<string,string>> (), It.IsAny<Action<INetworkPayload>> (), It.IsAny<Action<INetworkPayload>>)).Callback(() => onFail(_mockNetworkPayload.Object));
//
//			var controller = CreateController (4);
//
//			controller.Send ("url", new Dictionary<string,string> (), (payload) => Console.WriteLine ("success"), onFail);
//
//			_mockDecoratedNetworkController.Verify (c => c.Send (It.IsAny<string> (), It.IsAny<IDictionary<string,string>> (), It.IsAny<Action<INetworkPayload>> (), It.IsAny<Action<INetworkPayload>>), Times.Exactly (4));
//		}
//
//		[Test]
//		public void Send_RetryTwice_Succeed()
//		{
//			
//		}
//
//		[Test]
//		public void Send_MaxRetry_Fail()
//		{
//
//		}
//
//
//
//    }
//}