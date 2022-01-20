package team.hobbyrobot.tdn.core;

import java.io.IOException;

public interface TDNTypeParser
{
	String typeKey();
	TDNValue readFromStream(TDNBufferedReader reader) throws IOException;
	void writeToStream(TDNBufferedWriter writer, Object value) throws IOException;
}
