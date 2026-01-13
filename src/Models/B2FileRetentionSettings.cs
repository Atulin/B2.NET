namespace B2Net.Models {
	public class B2FileRetentionSettings {
		public RetentionMode? Mode { get; set; }
		public long? RetainUntilTimestamp { get; set; }
	}
}
