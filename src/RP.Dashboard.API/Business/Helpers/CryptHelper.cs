using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RP.Dashboard.API.Business.Helpers
{
	// Based on: https://stackoverflow.com/questions/46533454/specified-padding-mode-is-not-valid-for-this-algorithm-c-sharp-system-securi
	public class CryptHelper
	{
		private ConfigurationHelper _configuration { get; }

		public CryptHelper(ConfigurationHelper configuration)
		{
			_configuration = configuration;
		}

		public virtual string EncryptString(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				return string.Empty;
			}
			try
			{
				var key = Encoding.UTF8.GetBytes(_configuration.GetCryptoKey());
				byte[] IV = Encoding.ASCII.GetBytes(_configuration.GetCryptoVector());

				using (var aesAlg = Aes.Create())
				{
					using (var encryptor = aesAlg.CreateEncryptor(key, IV))
					{
						using (var msEncrypt = new MemoryStream())
						{
							using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
							using (var swEncrypt = new StreamWriter(csEncrypt))
							{
								swEncrypt.Write(text);
							}

							var decryptedContent = msEncrypt.ToArray();

							var result = new byte[IV.Length + decryptedContent.Length];

							Buffer.BlockCopy(IV, 0, result, 0, IV.Length);
							Buffer.BlockCopy(decryptedContent, 0, result, IV.Length, decryptedContent.Length);

							return Convert.ToBase64String(result);
						}
					}
				}
			}
			catch (Exception e)
			{
				// We might want some form of logging here
				throw e;
			}
		}

		public virtual string DecryptString(string cipherText)
		{
			if (string.IsNullOrWhiteSpace(cipherText))
			{
				return string.Empty;
			}
			try
			{
				var fullCipher = Convert.FromBase64String(cipherText);

				byte[] IV = Encoding.ASCII.GetBytes(_configuration.GetCryptoVector());
				var cipher = new byte[fullCipher.Length - IV.Length];

				Buffer.BlockCopy(fullCipher, 0, IV, 0, IV.Length);
				Buffer.BlockCopy(fullCipher, IV.Length, cipher, 0, fullCipher.Length - IV.Length);
				var key = Encoding.UTF8.GetBytes(_configuration.GetCryptoKey());

				using (var aesAlg = Aes.Create())
				{
					using (var decryptor = aesAlg.CreateDecryptor(key, IV))
					{
						string result;
						using (var msDecrypt = new MemoryStream(cipher))
						{
							using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
							{
								using (var srDecrypt = new StreamReader(csDecrypt))
								{
									result = srDecrypt.ReadToEnd();
								}
							}
						}

						return result;
					}
				}
			}
			catch (Exception e)
			{
				// We might want some form of logging here
				throw e;
			}
		}
	}
}