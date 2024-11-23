using System;

namespace B2Net.Models;

public class AuthorizationException(string response) : Exception("There was an error during authorization. See inner exception for details.", new Exception(response));

public class NotAuthorizedException(string message) : Exception(message);

public class CopyReplaceSetupException(string message) : Exception(message);

public class B2Exception(string? code, string? status, string message, bool shouldRetry)
	: Exception(message)
{
	public string? Status { get; set; } = status;
	public string? Code { get; set; } = code;
	public bool ShouldRetryRequest { get; set; } = shouldRetry;

}
