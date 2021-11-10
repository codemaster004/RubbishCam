namespace RubbishCam.Api.Exceptions;


[Serializable]
public class ConflictException : Exception
{
	public ConflictException() { }
	public ConflictException( string message ) : base( message ) { }
	public ConflictException( string message, Exception inner ) : base( message, inner ) { }
	protected ConflictException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context ) : base( info, context ) { }
}
