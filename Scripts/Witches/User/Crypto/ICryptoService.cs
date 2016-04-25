
// maybe move to Voltage.Common namespace
namespace Voltage.Witches.Crypto
{
    // ICryptoService is an interface for encrypting/decrypting from string to byte[]
    // agnostic to symmetric or asymmetric encryption
	public interface ICryptoService 
	{
		byte[] Encrypt(string str);
		string Decrypt(byte[] bytes);
	}

}
