﻿using Amazon;
using Amazon.Runtime;
using InjectableAWS.Credentials;
using InjectableAWS.Tests;
using Microsoft.Extensions.Options;

namespace InjectableAWS.S3.UnitTests;

[TestFixture]
public sealed class S3ContextTests {

	private Mock<ICredentialsProvider>? _credentialsProvider;
	private Mock<IOptions<S3Options<S3ContextTests>>>? _options;
	private S3Context<S3ContextTests>? _context;

	[SetUp]
	public void SetUp() {
		_options = new Mock<IOptions<S3Options<S3ContextTests>>>( MockBehavior.Strict );
		_credentialsProvider = new Mock<ICredentialsProvider>( MockBehavior.Strict );
	}

	[TearDown]
	public void TearDown() {
		_credentialsProvider?.VerifyAll();
		_options?.VerifyAll();

		_context?.Dispose();
		_context = null;
	}

	[Test]
	public void Ctor_NullOptions_ThrowsException() {
		Assert.Throws<ArgumentException>( () => new S3Context<S3ContextTests>(
			_credentialsProvider!.Object,
			new NullOptions<S3Options<S3ContextTests>>()
		) );
	}

	[Test]
	public void Ctor_NoProfile_DefaultContextCreated() {
		BasicAWSCredentials credentials = new BasicAWSCredentials( "key", "secret" );
		_credentialsProvider!
			.Setup( cp => cp.GetCredentials( null ) )
			.Returns( credentials );
		AWSConfigs.AWSRegion = "us-east-1";
		SetupContext();

		Assert.IsNotNull( _context!.Client );
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

		Assert.IsNotNull( _context!.Client );
	}

	private void SetupContext(
		string? credentialsProfile = null,
		string? regionEndpoint = null,
		string? role = null
	) {
		S3Options<S3ContextTests> options = new S3Options<S3ContextTests> {
			CredentialsProfile = credentialsProfile,
			RegionEndpoint = regionEndpoint,
			Role = role
		};
		_options!
			.Setup( o => o.Value )
			.Returns( options );

		_context = new S3Context<S3ContextTests>(
			_credentialsProvider!.Object,
			_options!.Object
		);
	}
}
