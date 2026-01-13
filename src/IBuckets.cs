using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using B2Net.Models;

namespace B2Net;

public interface IBuckets
{
	Task<B2Bucket?> Create(string bucketName, B2BucketOptions options, CancellationToken cancelToken = default);
	Task<B2Bucket?> Create(string bucketName, BucketTypes bucketType, CancellationToken cancelToken = default);
	Task<B2Bucket?> Delete(string bucketId = "", CancellationToken cancelToken = default);
	Task<List<B2Bucket>> GetList(CancellationToken cancelToken = default);
	Task<B2Bucket?> Update(BucketTypes bucketType, string bucketId = "", CancellationToken cancelToken = default);
	Task<B2Bucket?> Update(B2BucketOptions options, string bucketId = "", CancellationToken cancelToken = default);
	Task<B2Bucket?> Update(B2BucketOptions options, int revisionNumber, string bucketId = "", CancellationToken cancelToken = default);
}
