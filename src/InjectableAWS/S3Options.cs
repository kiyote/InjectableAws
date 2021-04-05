using System;

namespace InjectableAWS {

	public sealed class S3Options<T> : IEquatable<S3Options<T>> {

		public string? CredentialsProfile { get; set; }

		public string? RegionEndpoint { get; set; }

		public string? Role { get; set; }

		public bool Equals( S3Options<T>? other ) {
			if( other is null ) {
				return false;
			}

			if( ReferenceEquals( other, this ) ) {
				return true;
			}

			return string.Equals( CredentialsProfile, other.CredentialsProfile, StringComparison.Ordinal )
				&& string.Equals( RegionEndpoint, other.RegionEndpoint, StringComparison.Ordinal )
				&& string.Equals( Role, other.Role, StringComparison.Ordinal );
		}

		public override bool Equals( object? obj ) {
			if( obj is not S3Options<T> target ) {
				return false;
			}

			return Equals( target );
		}

		public override int GetHashCode() {
			return HashCode.Combine( CredentialsProfile, RegionEndpoint, Role );
		}
	}

}
