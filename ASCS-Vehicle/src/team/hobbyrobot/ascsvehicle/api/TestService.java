package team.hobbyrobot.ascsvehicle.api;

import team.hobbyrobot.subos.net.api.Service;
import team.hobbyrobot.subos.net.api.exceptions.RequestGeneralException;
import team.hobbyrobot.subos.net.api.exceptions.RequestParamsException;
import team.hobbyrobot.subos.net.api.exceptions.UnknownRequestException;
import team.hobbyrobot.tdn.core.TDNRoot;
import team.hobbyrobot.tdn.core.TDNValue;
import team.hobbyrobot.tdn.base.TDNParsers;

public class TestService implements Service
{
	@Override
	public TDNRoot processRequest(String request, TDNRoot params)
		throws UnknownRequestException, RequestParamsException, RequestGeneralException
	{
		TDNRoot root = new TDNRoot();
		
		root.insertValue("request", new TDNValue(request, TDNParsers.STRING));
		root.insertValue("recieved-params", new TDNValue(params, TDNParsers.ROOT));
		
		return root;
	}

}
