namespace B2Net.Models {
	public class FileRetentionReturn {
		public bool IsClientAuthorizedToRead { get; set; }
		public B2FileRetentionSettings Value { get; set; }
	}
}
