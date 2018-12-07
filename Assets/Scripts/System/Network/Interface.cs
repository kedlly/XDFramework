
namespace Framework._System.Communication
{
	interface ITCPRequest
	{
		string IpAddr { get; }
		uint Port { get; }
	}

	interface ITCPRespond
	{

	}

	delegate void OnConnectionRespond(ITCPRespond respond);

	interface ITCPConnector
	{
		void Connect(bool isAsync);
		event OnConnectionRespond OnRespond;
	}
}
