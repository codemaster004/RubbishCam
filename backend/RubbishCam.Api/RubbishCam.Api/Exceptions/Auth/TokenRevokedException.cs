namespace RubbishCam.Api.Exceptions.Auth;

[Serializable]
public class TokenRevokedException : Exception
{
	public TokenRevokedException() { }
	protected TokenRevokedException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context ) : base( info, context ) { }
}
