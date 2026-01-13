using System;
using System.Collections.Generic;

namespace B2Net.Models {
	public class B2FileUploadContext {
		/// <summary>
		/// Required. Name of your file
		/// </summary>
		public string FileName { get; set; }
		
		/// <summary>
		/// Un-used for upload requests, used in responses. Providing a BucketID here for upload will do nothing. The UploadUrl determines Bucket.
		/// </summary>
		public string BucketId { get; set; }
		/// <summary>
		/// Optional.
		/// </summary>
		public Dictionary<string, string> AdditionalFileInfo { get; set; }
		/// <summary>
		/// Required. This can be retreived using GetUploadUrl.
		/// </summary>
		public B2UploadUrl B2UploadUrl { get; set; }
		public bool AutoRetry { get; set; }

		/// <summary>
		/// Optional.
		/// </summary>
		public string ContentType { get; set; }
		/// <summary>
		/// SHA1 Hash of the file. If provided, this will be appended to the end of the file bytes for B2 hash verification on upload.
		/// </summary>
		public string ContentSHA { get; set; }
		/// <summary>
		/// Optional.
		/// Follow the convention specified here: https://www.backblaze.com/b2/docs/b2_upload_file.html
		/// The value should be a base 10 number which represents a UTC time when the original source file was last modified. It is a base 10 number of milliseconds since midnight, January 1, 1970 UTC.
		/// </summary>
		public long SrcLastModifiedMillis { get; set; }
		/// <summary>
		/// Optional.
		/// Follow the grammar specified in RFC 2616 https://datatracker.ietf.org/doc/html/rfc2616#section-19.5.1
		/// </summary>
		public string ContentDisposition { get; set; }
		/// <summary>
		/// Optional.
		/// Follow the grammar specified in RFC 2616 https://datatracker.ietf.org/doc/html/rfc2616#section-19.5.1
		/// </summary>
		public string ContentLanguage { get; set; }
		/// <summary>
		/// Optional.
		/// Follow the grammar specified in RFC 2616 https://datatracker.ietf.org/doc/html/rfc2616#section-19.5.1
		/// </summary>
		public DateTime? Expires { get; set; }
		/// <summary>
		/// Optional.
		/// Follow the grammar specified in RFC 2616 https://datatracker.ietf.org/doc/html/rfc2616#section-19.5.1
		/// </summary>
		public string CacheControl { get; set; }
		/// <summary>
		/// Optional.
		/// Follow the grammar specified in RFC 2616 https://datatracker.ietf.org/doc/html/rfc2616#section-19.5.1
		/// </summary>
		public ContentEncoding? ContentEncoding { get; set; }
		/// <summary>
		/// Optional.
		/// </summary>
		public bool LegalHold { get; set; }
		/// <summary>
		/// Optional.
		/// </summary>
		public RetentionMode? RetentionMode { get; set; }
		/// <summary>
		/// Optional.
		/// This header value must be specified as a base 10 number of milliseconds since midnight, January 1, 1970 UTC.
		/// </summary>
		public long RetainUntilTimestamp { get; set; }
	}
}
