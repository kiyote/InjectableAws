﻿using Amazon;
using Amazon.Runtime;
using InjectableAWS.Credentials;
using InjectableAWS.Tests;
using Microsoft.Extensions.Options;

namespace InjectableAWS.Cognito.UnitTests;

[TestFixture]
public sealed class CognitoContextTests {

	public const string ClientId = "client_id";

	private Mock<ICredentialsProvider>? _credentialsProvider;
	private Mock<IOptions<CognitoOptions<CognitoContextTests>>>? _options;
	private CognitoContext<CognitoContextTests>? _context;

	[SetUp]
	public void SetUp() {
		_credentialsProvider = new Mock<ICredentialsProvider>( MockBehavior.Strict );
		_options = new Mock<IOptions<CognitoOptions<CognitoContextTests>>>( MockBehavior.Strict );
	}

	[TearDown]
	public void TearDown() {
		_credentialsProvider?.VerifyAll();
		_options?.VerifyAll();

		_context?.Dispose();
		_context = null;
	}

	[Test]
	public void Ctor_NullOptions_ThrowsArgumentException() {
		Assert.Throws<ArgumentException>( () => new CognitoContext<CognitoContextTests>(
			_credentialsProvider!.Object,
			new NullOptions<CognitoOptions<CognitoContextTests>>()
		) );
	}

	[Test]
	public void Ctor_UseProfileAndRegion_ContextWithAssumedRoleCreated() {
		string profile = "profile";
		string role = "role";
		string region = "us-east-1";
		BasicAWSCredentials creds1 = new BasicAWSCredentials( "1", "1" );
		_credentialsProvider!
			.Setup( cp => cp.GetCredentials( profile ) )
			.Returns( creds1 );
		BasicAWSCredentials creds2 = new BasicAWSCredentials( "2", "2" );
		_credentialsProvider!
			.Setup( cp => cp.AssumeRole( creds1, role ) )
			.Returns( creds2 );

		SetupContext(
			credentialsProfile: profile,
			regionEndpoint: region,
			role: role
		);

		Assert.IsNotNull( _context!.Provider );
	}

	[Test]
	public void Ctor_DefaultCredentials_ProviderCreated() {

		BasicAWSCredentials credentials = new BasicAWSCredentials( "accessKey", "secretKey" ) {			
		};
		AWSConfigs.AWSRegion = "us-east-1";
		_credentialsProvider!
			.Setup( cp => cp.GetCredentials( null ) )
			.Returns( credentials );

		SetupContext();

		Assert.IsNotNull( _context!.Provider );
	}

	private void SetupContext(
		string clientId = ClientId,
		string? regionEndpoint = null,
		string? credentialsProfile = null,
		string? role = null
	) {
		CognitoOptions<CognitoContextTests> options = new CognitoOptions<CognitoContextTests> {
			ClientId = clientId,
			RegionEndpoint = regionEndpoint,
			CredentialsProfile = credentialsProfile,
			Role = role
		};
		_options!
			.Setup( o => o.Value )
			.Returns( options );

		_context = new CognitoContext<CognitoContextTests>(
			_credentialsProvider!.Object,
			_options.Object
		);
	}
}
