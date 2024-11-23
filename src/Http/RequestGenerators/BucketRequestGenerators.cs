using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using B2Net.Models;

namespace B2Net.Http.RequestGenerators;

public static partial class BucketRequestGenerators
{
	private static readonly JsonSerializerOptions JsonSerializerOptions = new()
	{
		WriteIndented = true,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};

	private static class Endpoints
	{
		public const string List = "b2_list_buckets";
		public const string Create = "b2_create_bucket";
		public const string Delete = "b2_delete_bucket";
		public const string Update = "b2_update_bucket";
	}

	public static HttpRequestMessage GetBucketList(B2Options options)
	{
		var json = JsonSerializer.Serialize(new { accountId = options.AccountId });
		return BaseRequestGenerator.PostRequest(Endpoints.List, json, options);
	}

	public static HttpRequestMessage DeleteBucket(B2Options options, string bucketId)
	{
		var json = JsonSerializer.Serialize(new { accountId = options.AccountId, bucketId });
		return BaseRequestGenerator.PostRequest(Endpoints.Delete, json, options);
	}

	/// <summary>
	/// Create a bucket. Defaults to allPrivate.
	/// </summary>
	/// <param name="options"></param>
	/// <param name="bucketName"></param>
	/// <param name="bucketType"></param>
	/// <returns></returns>
	public static HttpRequestMessage CreateBucket(B2Options options, string bucketName, string bucketType = "allPrivate")
	{
		var allowed = AllowedBucketNameRegex();
		if (bucketName.Length < 6 || bucketName.Length > 50 || !allowed.IsMatch(bucketName) || bucketName.StartsWith("b2-"))
		{
			throw new Exception("""
			                    The bucket name specified does not match the requirements. 
			                    Bucket Name can consist of upper-case letters, lower-case letters, numbers, and " - ", 
			                    must be at least 6 characters long, and can be at most 50 characters long
			                    """);
		}

		var json = JsonSerializer.Serialize(new { accountId = options.AccountId, bucketName, bucketType });
		return BaseRequestGenerator.PostRequest(Endpoints.Create, json, options);
	}

	/// <summary>
	/// Create a bucket. Defaults to allPrivate.
	/// </summary>
	/// <param name="options"></param>
	/// <param name="bucketName"></param>
	/// <param name="bucketOptions"></param>
	/// <returns></returns>
	public static HttpRequestMessage CreateBucket(B2Options options, string bucketName, B2BucketOptions bucketOptions)
	{
		// Check lifecycle rules
		var hasLifecycleRules = bucketOptions.LifecycleRules is { Count: > 0 };
		if (hasLifecycleRules)
		{
			foreach (var rule in bucketOptions.LifecycleRules)
			{
				if (rule.DaysFromHidingToDeleting < 1 || rule.DaysFromUploadingToHiding < 1)
				{
					throw new Exception("The smallest number of days you can set in a lifecycle rule is 1.");
				}
				if (rule.DaysFromHidingToDeleting == null && rule.DaysFromUploadingToHiding == null)
				{
					throw new Exception("You must set either DaysFromHidingToDeleting or DaysFromUploadingToHiding. Both cannot be null.");
				}
			}
		}

		var allowed = AllowedBucketNameRegex();
		if (bucketName.Length < 6 || bucketName.Length > 50 || !allowed.IsMatch(bucketName) || bucketName.StartsWith("b2-"))
		{
			throw new Exception("""
			                    The bucket name specified does not match the requirements. 
			                    Bucket Name can consist of upper-case letters, lower-case letters, numbers, and " - ", 
			                    must be at least 6 characters long, and can be at most 50 characters long
			                    """);
		}

		var body = new B2BucketCreateModel
		{
			AccountId = options.AccountId,
			BucketName = bucketName,
			BucketType = bucketOptions.BucketType.ToString()
		};

		// Add optional options
		if (bucketOptions.CacheControl != 0)
		{
			body.BucketInfo = new Dictionary<string, string>
			{
				{ "Cache-Control", "max-age=" + bucketOptions.CacheControl }
			};
		}
		if (hasLifecycleRules)
		{
			body.LifecycleRules = bucketOptions.LifecycleRules;
		}

		// Has cors rules
		if (bucketOptions.CORSRules is { Count: > 0 })
		{
			body.CorsRules = bucketOptions.CORSRules;
		}

		var json = JsonSerialize(body);
		return BaseRequestGenerator.PostRequest(Endpoints.Create, json, options);
	}

	/// <summary>
	/// Used to modify the bucket type of the provided bucket.
	/// </summary>
	/// <param name="options"></param>
	/// <param name="bucketId"></param>
	/// <param name="bucketType"></param>
	/// <returns></returns>
	public static HttpRequestMessage UpdateBucket(B2Options options, string bucketId, string bucketType)
	{
		var json = JsonSerializer.Serialize(new { accountId = options.AccountId, bucketId, bucketType });
		return BaseRequestGenerator.PostRequest(Endpoints.Update, json, options);
	}

	/// <summary>
	/// Used to modify the bucket type of the provided bucket.
	/// </summary>
	/// <param name="bucketOptions"></param>
	/// <param name="revisionNumber">(optional) When set, the update will only happen if the revision number stored in the B2 service matches the one passed in. This can be used to avoid having simultaneous updates make conflicting changes. </param>
	/// <param name="options"></param>
	/// <param name="bucketId"></param>
	/// <returns></returns>
	public static HttpRequestMessage UpdateBucket(B2Options options, string bucketId, B2BucketOptions bucketOptions, int? revisionNumber = null)
	{
		// Check lifecycle rules
		var hasLifecycleRules = bucketOptions.LifecycleRules is { Count: > 0 };
		if (hasLifecycleRules)
		{
			foreach (var rule in bucketOptions.LifecycleRules)
			{
				if (rule.DaysFromHidingToDeleting < 1 || rule.DaysFromUploadingToHiding < 1)
				{
					throw new Exception("The smallest number of days you can set in a lifecycle rule is 1.");
				}
				if (rule.DaysFromHidingToDeleting == null && rule.DaysFromUploadingToHiding == null)
				{
					throw new Exception("You must set either DaysFromHidingToDeleting or DaysFromUploadingToHiding. Both cannot be null.");
				}
			}
		}

		var body = new B2BucketUpdateModel
		{
			AccountId = options.AccountId,
			BucketId = bucketId,
			BucketType = bucketOptions.BucketType.ToString()
		};

		// Add optional options
		if (bucketOptions.CacheControl != 0)
		{
			body.BucketInfo = new Dictionary<string, string>
			{
				{ "Cache-Control", "max-age=" + bucketOptions.CacheControl }
			};
		}
		if (hasLifecycleRules)
		{
			body.LifecycleRules = bucketOptions.LifecycleRules;
		}

		// Has cors rules
		if (bucketOptions.CORSRules is { Count: > 0 })
		{
			if (bucketOptions.CORSRules.Any(x => x.AllowedOperations is not { Length: not 0 }))
			{
				throw new Exception("You must set allowedOperations on the bucket CORS rules.");
			}
			if (bucketOptions.CORSRules.Any(x => x.AllowedOrigins is not { Length: not 0 }))
			{
				throw new Exception("You must set allowedOrigins on the bucket CORS rules.");
			}
			if (bucketOptions.CORSRules.Any(x => string.IsNullOrEmpty(x.CorsRuleName)))
			{
				throw new Exception("You must set corsRuleName on the bucket CORS rules.");
			}
			body.CorsRules = bucketOptions.CORSRules;
		}

		if (revisionNumber.HasValue)
		{
			body.IfRevisionIs = revisionNumber.Value;
		}

		var json = JsonSerialize(body);
		return BaseRequestGenerator.PostRequest(Endpoints.Update, json, options);
	}

	private static string JsonSerialize(object data)
	{
		return JsonSerializer.Serialize(data, JsonSerializerOptions);
	}

	[GeneratedRegex("^[a-zA-Z0-9-]+$")]
	private static partial Regex AllowedBucketNameRegex();
}

internal class B2BucketCreateModel
{
	public string AccountId { get; set; }
	public string BucketName { get; set; }
	public string BucketType { get; set; }
	public Dictionary<string, string> BucketInfo { get; set; }
	public List<B2BucketLifecycleRule> LifecycleRules { get; set; }
	public List<B2CORSRule> CorsRules { get; set; }
}

internal class B2BucketUpdateModel
{
	public string AccountId { get; set; }
	public string BucketId { get; set; }
	public string BucketType { get; set; }
	public Dictionary<string, string> BucketInfo { get; set; }
	public List<B2BucketLifecycleRule> LifecycleRules { get; set; }
	public List<B2CORSRule> CorsRules { get; set; }
	public int? IfRevisionIs { get; set; }
}
