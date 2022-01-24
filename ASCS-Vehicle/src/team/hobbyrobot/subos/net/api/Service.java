package team.hobbyrobot.subos.net.api;

import team.hobbyrobot.subos.net.api.exceptions.RequestGeneralException;
import team.hobbyrobot.subos.net.api.exceptions.RequestParamsException;
import team.hobbyrobot.subos.net.api.exceptions.UnknownRequestException;
import team.hobbyrobot.tdn.core.TDNRoot;

public interface Service
{
	TDNRoot processRequest(String request, TDNRoot params) throws UnknownRequestException, RequestParamsException, RequestGeneralException;
}
