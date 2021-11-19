using System;
using System.Net.Http;

namespace Client.Authentication
{
    public class ServiceUnauthorizedException : HttpRequestException {
        public ServiceUnauthorizedException() {
        }

        public ServiceUnauthorizedException(string message, Uri? challengeUri)
            : base(message) {
            ChallengeUri = challengeUri;
        }

        public ServiceUnauthorizedException(string message, Exception innerException)
            : base(message, innerException) {
        }

        public Uri? ChallengeUri { get; }
    }
}
