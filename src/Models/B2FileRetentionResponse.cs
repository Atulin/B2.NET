namespace B2Net.Models;

public class B2FileRetentionResponse
{
	public string fileId { get; set; }
	public string fileName { get; set; }
	public FileRetentionReturn FileRetention { get; set; }
}
