package team.hobbyrobot.subos.net.api.exceptions;

public class RequestParamsException extends Exception
{
	/**
	 * 
	 */
	private static final long serialVersionUID = -3451153443239230853L;
	
	public String message;
	public String[] badParams;
	
	public RequestParamsException(String message, String... badParams)
	{
		this.message = message;
		this.badParams = badParams;
	}
}
