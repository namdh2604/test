using System;


namespace Unit.Witches.Crypto
{
    using NUnit.Framework;
    using Moq;

	using Voltage.Witches.Crypto;

    [TestFixture]
    public class RijndaelCryptoService_Test 
    {
        private Mock<ICryptoKeyStore> _mock_CryptoKeyStore;

        [SetUp]
        public void Init()
        {
            byte[] key = {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16};
            byte[] iv = {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16};

            _mock_CryptoKeyStore = new Mock<ICryptoKeyStore>();
			_mock_CryptoKeyStore.Setup(mock => mock.GetEncryptKey()).Returns(key);     	// _mock_CryptoKeyStore.Setup(mock => mock.GetKey(It.IsAny<string>())).Returns(key);
			_mock_CryptoKeyStore.Setup(mock => mock.GetDecryptKey()).Returns(key);
			_mock_CryptoKeyStore.Setup(mock => mock.GetIV()).Returns(iv);      			// _mock_CryptoKeyStore.Setup(mock => mock.GetIV(It.IsAny<string>())).Returns(iv);
        }


        [Test]
        public void Encrypt_ValidByteArray()
        {
            string orig = "hello world!";
            byte[] expected = {0x23, 0xe4, 0xba, 0xc4, 0x07, 0x35, 0xe6, 0x38, 0x17, 0x4b, 0xbb, 0x58, 0x1f, 0x4c, 0xb0, 0x47};

            RijndaelCryptoService cryptoService = new RijndaelCryptoService(_mock_CryptoKeyStore.Object);

            Assert.That(cryptoService.Encrypt(orig), Is.EqualTo(expected));
        }


        [Test]
        public void Decrypt_ValidByteArray()
        {
            byte[] orig = {0x23, 0xe4, 0xba, 0xc4, 0x07, 0x35, 0xe6, 0x38, 0x17, 0x4b, 0xbb, 0x58, 0x1f, 0x4c, 0xb0, 0x47};
            string expected = "hello world!";

            RijndaelCryptoService cryptoService = new RijndaelCryptoService(_mock_CryptoKeyStore.Object);

            Assert.That(cryptoService.Decrypt(orig), Is.EqualTo(expected));
        }


        [Test]
        public void RoundTrip_Successful()
        {
            string orig = "the quick, brown fox jumped over hung 1, 2, 3, 4! [] {} ; : ', 0 $ #& * ()";

            RijndaelCryptoService cryptoService = new RijndaelCryptoService(_mock_CryptoKeyStore.Object);

            byte[] encrypted = cryptoService.Encrypt(orig);
            string loaded = cryptoService.Decrypt(encrypted);

            Assert.That(loaded, Is.EqualTo(orig));          // Is.StringMatching's regex pattern doesn't support brackets well
        }


        // TODO: test failure case and null input



        private string ByteArrayToString(byte[] bytes)
        {
            System.Text.StringBuilder hex = new System.Text.StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes) 
            {
                hex.AppendFormat("0x{0:x2}, ", b);
            }

            return hex.ToString();
        }

    }
}