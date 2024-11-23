using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace B2Net.Http;

public static class ResponseParser {
	private static JsonSerializerOptions _jsonSerializerOptions = new()
	{
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
	};
	
	public static async Task<T?> ParseResponse<T>(HttpResponseMessage response, string callingApi = "") {
		var jsonResponse = await response.Content.ReadAsStringAsync();

		await Utilities.CheckForErrors(response, callingApi);

		_jsonSerializerOptions = new JsonSerializerOptions() {
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};
		return JsonSerializer.Deserialize<T>(jsonResponse, _jsonSerializerOptions);
	}
}
