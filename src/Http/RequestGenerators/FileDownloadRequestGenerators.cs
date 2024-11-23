using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using B2Net.Models;

namespace B2Net.Http.RequestGenerators;

public static class FileDownloadRequestGenerators {
	private static readonly JsonSerializerOptions JsonSerializerOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
	};

	private const string DownloadByIdEndpoint = "b2_download_file_by_id";
	private const string GetDownloadAuthorizationEndpoint = "b2_get_download_authorization";
	private const string DownloadByNameEndpoint = "file";
	
	public static HttpRequestMessage DownloadById(B2Options options, string fileId, string byteRange = "") {
		var uri = new Uri($"{options.DownloadUrl}/b2api/{Constants.Version}/{DownloadByIdEndpoint}");

		var json = JsonSerializer.Serialize(new { fileId });
		var request = new HttpRequestMessage {
			Method = HttpMethod.Post,
			RequestUri = uri,
			Content = new StringContent(json)
		};

		request.Headers.TryAddWithoutValidation("Authorization", options.AuthorizationToken);

		// Add byte range header if we have it
		if (!string.IsNullOrEmpty(byteRange)) {
			request.Headers.Add("Range", $"bytes={byteRange}");
		}

		return request;
	}

	public static HttpRequestMessage DownloadByName(B2Options options, string bucketName, string fileName, string byteRange = "") {
		var uri = new Uri(options.DownloadUrl + "/" + DownloadByNameEndpoint + "/" + bucketName + "/" + fileName.B2UrlEncode());
		var request = new HttpRequestMessage {
			Method = HttpMethod.Get,
			RequestUri = uri
		};

		request.Headers.TryAddWithoutValidation("Authorization", options.AuthorizationToken);

		// Add byte range header if we have it
		if (!string.IsNullOrEmpty(byteRange)) {
			request.Headers.Add("Range", $"bytes={byteRange}");
		}

		return request;
	}

	public static HttpRequestMessage GetDownloadAuthorization(B2Options options, string fileNamePrefix, int validDurationInSeconds, string bucketId, string? b2ContentDisposition = null) {
		var uri = new Uri($"{options.ApiUrl}/b2api/{Constants.Version}/{GetDownloadAuthorizationEndpoint}");

		var message = new RequestMessage(bucketId, fileNamePrefix, validDurationInSeconds);

		if (!string.IsNullOrEmpty(b2ContentDisposition))
		{
			message.B2ContentDisposition = b2ContentDisposition;
		}

		var request = new HttpRequestMessage {
			Method = HttpMethod.Post,
			RequestUri = uri,
			Content = JsonContent.Create(message, options: JsonSerializerOptions)
		};

		request.Headers.TryAddWithoutValidation("Authorization", options.AuthorizationToken);

		return request;
	}

	private sealed record RequestMessage(string BucketId, string FileNamePrefix, int ValidDurationInSeconds)
	{
		public string? B2ContentDisposition { get; set; }
	}
}
