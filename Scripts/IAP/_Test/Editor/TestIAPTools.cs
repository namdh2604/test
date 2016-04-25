
using System;

namespace Unit.Common.IAP.Tools
{
    using NUnit.Framework;

	using Voltage.Common.IAP.Tools;

    [TestFixture]
    public class TestIAPTools
    {

		[Test]
        public void StringTransformXOR_ValidTransform()
        {
			string input = "foobar";
			int salt = 39;

			string output = IAPTools.StringTransformXOR (input, salt);

			Assert.That (output, Is.StringMatching ("AHHEFU"));
        }

		[Test]
		public void StringTransformXOR_ValidTransformBack()
		{
			string input = "foobar";
			int salt = 39;
			
			string output = IAPTools.StringTransformXOR (input, salt);

			string transformedBack = IAPTools.StringTransformXOR (output, salt);

			Assert.That (transformedBack, Is.StringMatching (input));
		}





    }
}