using Microsoft.Extensions.Configuration;
using Moq;
using RP.Dashboard.API.Business.Helpers;
using Xunit;

namespace RP.Dashboard.API.Tests
{
	public class CryptHelperTests
	{
		// Set up of shared details for tests
		private const string _plainTextDecrypted = "ThisStringIsValidForEncyption";

		private const string _otherPlainTextDecrypted = "ThisStringIsValidForEncyptoon";
		private const string _plainTextEncrypted = "MDk4NzY1NDNyZWxwcm9kNa3qAEeXrRf2ewq7jm0sJsJtOhi7q0uIIDHtuQI0PUUI";
		private const string _otherPlainTextEncrypted = "XXk4NzY1NDNyZWxwcm9kNa3qAEeXrRf2ewq7jm0sJsJtOhi7q0uIIDHtuQI0PUUI";
		private const string _cryptoKey = "1234567890asdfghjkl12345678qwert";
		private const string _cryptoVector = "09876543relprod5";

		#region EncryptString

		[Fact]
		public void GivenAnEmptyString_WhenEncryptStringIsEnvoked_ThenReturnsAnEmptyString()
		{
			// Arrange
			var mockConfiguration = new Mock<IConfiguration>();
			var mockSettings = new Mock<ConfigurationHelper>(mockConfiguration.Object);

			var sut = new CryptHelper(mockSettings.Object);

			// Act
			var result = sut.EncryptString(string.Empty);

			// Assert
			Assert.Equal(string.Empty, result);
		}

		[Fact]
		public void GivenAnWhiteSpaceString_WhenEncryptStringIsEnvoked_ThenReturnsAnEmptyString()
		{
			// Arrange
			var mockConfiguration = new Mock<IConfiguration>();
			var mockSettings = new Mock<ConfigurationHelper>(mockConfiguration.Object);

			var sut = new CryptHelper(mockSettings.Object);

			// Act
			var result = sut.EncryptString(" ");

			// Assert
			Assert.Equal(string.Empty, result);
		}

		[Fact]
		public void GivenAnUnencryptedString_WhenEncryptStringIsEnvoked_ThenReturnsAnCorrectlyEncryptedString()
		{
			// Arrange
			var mockConfiguration = new Mock<IConfiguration>();
			var mockSettings = new Mock<ConfigurationHelper>(mockConfiguration.Object);
			mockSettings.Setup(s => s.GetCryptoKey()).Returns(_cryptoKey);
			mockSettings.Setup(s => s.GetCryptoVector()).Returns(_cryptoVector);

			var sut = new CryptHelper(mockSettings.Object);

			// Act
			var result = sut.EncryptString(_plainTextDecrypted);

			// Assert
			Assert.Equal(0, string.CompareOrdinal(_plainTextEncrypted, result));
		}

		[Fact]
		public void GivenAnInvalidUnencryptedString_WhenEncryptStringIsEnvoked_ThenReturnsAnDifferentEncryptedString()
		{
			// Arrange
			var mockConfiguration = new Mock<IConfiguration>();
			var mockSettings = new Mock<ConfigurationHelper>(mockConfiguration.Object);
			mockSettings.Setup(s => s.GetCryptoKey()).Returns(_cryptoKey);
			mockSettings.Setup(s => s.GetCryptoVector()).Returns(_cryptoVector);

			var sut = new CryptHelper(mockSettings.Object);

			// Act
			var result = sut.EncryptString(_otherPlainTextDecrypted);

			// Assert
			Assert.NotEqual(_plainTextEncrypted, result);
		}

		#endregion EncryptString

		#region EncryptString

		[Fact]
		public void GivenAnEmptyString_WhenDecryptStringIsEnvoked_ThenReturnsAnEmptyString()
		{
			// Arrange
			var mockConfiguration = new Mock<IConfiguration>();
			var mockSettings = new Mock<ConfigurationHelper>(mockConfiguration.Object);

			var sut = new CryptHelper(mockSettings.Object);

			// Act
			var result = sut.DecryptString(string.Empty);

			// Assert
			Assert.Equal(string.Empty, result);
		}

		[Fact]
		public void GivenAnWhiteSpaceString_WhenDecryptStringIsEnvoked_ThenReturnsAnEmptyString()
		{
			// Arrange
			var mockConfiguration = new Mock<IConfiguration>();
			var mockSettings = new Mock<ConfigurationHelper>(mockConfiguration.Object);

			var sut = new CryptHelper(mockSettings.Object);

			// Act
			var result = sut.DecryptString(" ");

			// Assert
			Assert.Equal(string.Empty, result);
		}

		[Fact]
		public void GivenAnValidEncryptedString_WhenDecryptStringIsEnvoked_ThenReturnsAnCorrectlyDecryptedString()
		{
			// Arrange
			var mockConfiguration = new Mock<IConfiguration>();
			var mockSettings = new Mock<ConfigurationHelper>(mockConfiguration.Object);
			mockSettings.Setup(s => s.GetCryptoKey()).Returns(_cryptoKey);
			mockSettings.Setup(s => s.GetCryptoVector()).Returns(_cryptoVector);

			var sut = new CryptHelper(mockSettings.Object);

			// Act
			var result = sut.DecryptString(_plainTextEncrypted);

			// Assert
			Assert.Equal(0, string.CompareOrdinal(_plainTextDecrypted, result));
		}

		[Fact]
		public void GivenAnInValidEncryptedString_WhenDecryptStringIsEnvoked_ThenReturnsAnFalseDecryptedString()
		{
			// Arrange
			var mockConfiguration = new Mock<IConfiguration>();
			var mockSettings = new Mock<ConfigurationHelper>(mockConfiguration.Object);
			mockSettings.Setup(s => s.GetCryptoKey()).Returns(_cryptoKey);
			mockSettings.Setup(s => s.GetCryptoVector()).Returns(_cryptoVector);

			var sut = new CryptHelper(mockSettings.Object);

			// Act
			var result = sut.DecryptString(_otherPlainTextEncrypted);

			// Assert
			Assert.NotEqual(_plainTextDecrypted, result);
		}

		#endregion EncryptString
	}
}