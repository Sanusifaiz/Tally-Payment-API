// Authored by Tade samson find link to github here: https://github.com/TadeSamson/RavePaymentDataEncryption
using System;

namespace Tally_Payment_API.Services
{
    public interface IPaymentDataEncryption
    {
        string GetEncryptionKey(string secretKey);
        string EncryptData(string encryptionKey, string data);
        string DecryptData(string encryptedData, string encryptionKey);

    }
}