using System.Net.Http;
using System.Text.Json;
using B2Net.Models;

namespace B2Net.Http.RequestGenerators;

public static class FileDeleteRequestGenerator {
	private static class Endpoints {
		public const string Delete = "b2_delete_file_version";
	}

	public static HttpRequestMessage Delete(B2Options options, string fileId, string fileName) {
		var json = JsonSerializer.Serialize(new { fileId, fileName });
		return BaseRequestGenerator.PostRequest(Endpoints.Delete, json, options);
	}
}
