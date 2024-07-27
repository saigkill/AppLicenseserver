// <copyright file="EntityHelper.cs" company="marcos software">
//   this file may not be redistributed in whole or significant part
//   and is subject to the marcos software license.
//
//   @author: Sascha Manns, <s.manns@marcossoftware.com>
//   @copyright: 2022, marcos-software.de, http://www.marcos-software.de
// </copyright>
#pragma warning disable SA1027 // TabsMustNotBeUsed
#pragma warning disable SA1200 // UsingDirectivesMustBePlacedWithinNamespace
#pragma warning disable SA1309 // FieldNamesMustNotBeginWithUnderscore
#pragma warning disable SA1101 // PrefixLocalCallsWithThis

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AppLicenseserver.Entity
{
	/// <summary>
	/// Helper class for entities.
	/// </summary>
	public static class EntityHelper
	{
		static readonly string _EncryptionKey = "eid729";

		/// <summary>
		/// Encrypts the specified clear text.
		/// </summary>
		/// <param name="clearText">The clear text.</param>
		/// <returns>A Base64 String with the encrypted password.</returns>
		public static string Encrypt(string clearText)
		{
			byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
			using (Aes encryptor = Aes.Create())
			{
				Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(_EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
				encryptor.Key = pdb.GetBytes(32);
				encryptor.IV = pdb.GetBytes(16);
				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
					{
						cs.Write(clearBytes, 0, clearBytes.Length);
						cs.Close();
					}

					clearText = Convert.ToBase64String(ms.ToArray());
				}
			}

			return clearText;
		}

		/// <summary>
		/// Decrypts the specified cipher text.
		/// </summary>
		/// <param name="cipherText">The cipher text.</param>
		/// <returns>Clear Password.</returns>
		public static string Decrypt(string cipherText)
		{
			cipherText = cipherText.Replace(" ", "+");
			byte[] cipherBytes = Convert.FromBase64String(cipherText);
			using (Aes encryptor = Aes.Create())
			{
				Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(_EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
				encryptor.Key = pdb.GetBytes(32);
				encryptor.IV = pdb.GetBytes(16);
				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
					{
						cs.Write(cipherBytes, 0, cipherBytes.Length);
						cs.Close();
					}

					cipherText = Encoding.Unicode.GetString(ms.ToArray());
				}
			}

			return cipherText;
		}
	}
}
