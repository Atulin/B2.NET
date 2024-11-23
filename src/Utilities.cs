using B2Net.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace B2Net;

public static class Utilities
{
	/// <summary>
	/// Create the B2 Authorization header. Base64 encoded accountId:applicationKey.
	/// </summary>
	public static string CreateAuthorizationHeader(string accountId, string applicationKey)
	{
		const string authHeader = "Basic ";
		var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{accountId}:{applicationKey}"));
		return authHeader + credentials;
	}

	public static async Task CheckForErrors(HttpResponseMessage response, string callingApi = "")
	{
		if (!response.IsSuccessStatusCode)
		{
			// Should retry
			var retry = response.StatusCode is (HttpStatusCode)429 or HttpStatusCode.RequestTimeout or HttpStatusCode.ServiceUnavailable;

			var content = await response.Content.ReadAsStringAsync();

			B2Error? b2Error;
			try
			{
				b2Error = JsonSerializer.Deserialize<B2Error>(content);
			}
			catch (Exception ex)
			{
				throw new Exception("Seralization of the response failed. See inner exception for response contents and serialization error.", ex);
			}
			
			if (b2Error is not null)
			{
				// If calling API is supplied, append to the error message
				if (!string.IsNullOrEmpty(callingApi) && b2Error.Code == "401")
				{
					b2Error.Message = $"Unauthorized error when operating on {callingApi}. Are you sure the key you are using has access? {b2Error.Message}";
				}
				throw new B2Exception(b2Error.Code, b2Error.Status, b2Error.Message ?? "Unknown error", retry);
			}
		}
	}

	public static string GetSha1Hash(byte[] fileData) => HexStringFromBytes(SHA1.HashData(fileData));

	public static string DetermineBucketId(B2Options options, string bucketId)
	{
		// Check for a persistent bucket
		if (!options.PersistBucket && string.IsNullOrEmpty(bucketId))
		{
			throw new ArgumentNullException(nameof(bucketId), "You must either Persist a Bucket or provide a BucketId in the method call.");
		}

		// Are we persisting buckets? If so use the one from settings
		return options.PersistBucket ? options.BucketId : bucketId;
	}

	private static string HexStringFromBytes(byte[] bytes)
	{
		var sb = new StringBuilder();
		foreach (var b in bytes)
		{
			var hex = b.ToString("x2");
			sb.Append(hex);
		}
		return sb.ToString();
	}

	internal class B2Error
	{
		public string? Code { get; set; }
		public string? Message { get; set; }
		public string? Status { get; set; }
	}
}

public static class B2StringExtension
{
	public static string B2UrlEncode(this string str)
	{
		return str == "/" 
			? str 
			// Decode / back to un-encoded value
			: Uri.EscapeDataString(str).Replace("%2F", "/");
	}

	public static string B2UrlDecode(this string str)
	{
		return str == "+" 
			? " " 
			: Uri.UnescapeDataString(str);
	}
}
