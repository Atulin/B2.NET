using B2Net.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace B2Net;

public interface ILargeFiles {
	Task<B2CancelledFile?> CancelLargeFile(string fileId, CancellationToken cancelToken = default);
	Task<B2File?> FinishLargeFile(string fileId, string[] partSHA1Array, CancellationToken cancelToken = default);
	Task<B2UploadPartUrl?> GetUploadPartUrl(string fileId, CancellationToken cancelToken = default);
	Task<B2IncompleteLargeFiles?> ListIncompleteFiles(string bucketId, string startFileId = "", string maxFileCount = "", CancellationToken cancelToken = default);
	Task<B2LargeFileParts?> ListPartsForIncompleteFile(string fileId, int startPartNumber, int maxPartCount, CancellationToken cancelToken = default);
	Task<B2File?> StartLargeFile(string fileName, string contentType = "", string bucketId = "", Dictionary<string, string> fileInfo = null, CancellationToken cancelToken = default);
	Task<B2UploadPart?> UploadPart(byte[] fileData, int partNumber, B2UploadPartUrl uploadPartUrl, CancellationToken cancelToken = default);
	//Task<B2LargeFilePart> CopyPart(string sourceFileId, string destinationLargeFileId, int destinationPartNumber, string range = "", CancellationToken cancelToken = default(CancellationToken));
}