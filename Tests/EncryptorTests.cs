using MerkleFileServer.Helpers;
using NUnit.Framework;

namespace MerkleFileServerTests
{
    public class EncryptorTests
    {
        [Test]
        public void EncryptorByteArray_Should_ReturnTrueBackAndForth()
        {
            var exmapleString = "956bf86d100b2f49a8d057ebafa85b8db89a0f19d5627a1226fea1cb3e23d3f3";
            var byteArrayExmapleString = Encryptor.StringToByteArray(exmapleString);
            var byteArrayExmapleStringReverse = Encryptor.ByteArrayToString(byteArrayExmapleString);

            Assert.That(exmapleString, Is.EqualTo(byteArrayExmapleStringReverse));
        }
    }
}