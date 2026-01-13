using System.Net.Http;
using B2Net.Http.RequestGenerators;
using B2Net.Models;

namespace B2Net.Http;

public static class FileDeleteRequestGenerator
{

	public static HttpRequestMessage Delete(B2Options options, string fileId, string fileName)
	{
		var json = Utilities.JsonSerialize(new { fileId, fileName });
		return BaseRequestGenerator.PostRequest(Endpoints.Delete, json, options);
	}

	private static class Endpoints
	{
		public const string Delete = "b2_delete_file_version";
	}
}
