namespace RubbishCam.Api.Exceptions.Auth;

[Serializable]
public class TokenInvalidException : Exception
{
	public TokenInvalidException() { }
	protected TokenInvalidException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context ) : base( info, context ) { }
}
