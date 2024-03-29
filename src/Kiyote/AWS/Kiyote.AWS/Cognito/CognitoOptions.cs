﻿namespace Kiyote.AWS.Cognito;

public sealed record CognitoOptions<T> where T: class {

	public string? ClientId { get; set; }
	public string? RegionEndpoint { get; set; }
	public string? CredentialsProfile { get; set; }
	public string? Role { get; set; }

}
