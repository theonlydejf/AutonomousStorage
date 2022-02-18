package team.hobbyrobot.subos.navigation;

public interface LimitablePilot
{
	void setTravelLimit(double limit);
	double getTravelLimit();
	
	void setRotateLimit(double limit);
	double getRotateLimit(double limit);
}
