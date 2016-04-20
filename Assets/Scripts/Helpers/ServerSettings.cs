using System;

public class ServerSettings {

	public string hostName = "";
	public string IP = "";
	public int portNumber = 55535;
	public int packetSize = 1024;
	public bool wasManuallyAdded = false;

	public ServerSettings(){}

	public ServerSettings(string[] address){
		hostName = address[0];
		IP = address[2];
		portNumber = Convert.ToInt32(address[1]);

	}
}
