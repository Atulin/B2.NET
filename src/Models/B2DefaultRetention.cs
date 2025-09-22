namespace B2Net.Models {
	/// <summary>
	/// This is used for Default Bucket Retention Settings and should not be used when updating per-file retention settings.
	/// </summary>
	public class B2DefaultRetention {
		public RetentionMode? Mode { get; set; }
		public Period? Period { get; set; }
	}
}
