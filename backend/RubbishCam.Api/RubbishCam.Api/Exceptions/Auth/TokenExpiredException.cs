namespace RubbishCam.Api.Exceptions.Auth;

[Serializable]
public class TokenExpiredException : Exception
{
	public TokenExpiredException() { }
	protected TokenExpiredException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context ) : base( info, context ) { }
}
