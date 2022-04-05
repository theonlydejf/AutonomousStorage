package team.hobbyrobot.subos.net.api;

import java.net.Socket;

import team.hobbyrobot.subos.net.api.exceptions.RequestGeneralException;
import team.hobbyrobot.subos.net.api.exceptions.RequestParamsException;
import team.hobbyrobot.subos.net.api.exceptions.UnknownRequestException;
import team.hobbyrobot.tdn.core.TDNRoot;

public interface Service
{
	TDNRoot processRequest(String request, TDNRoot params, Socket client) throws UnknownRequestException, RequestParamsException, RequestGeneralException;
	void init();
}
