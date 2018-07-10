
using TargetLoginRequest = Assets.CommunicationSystem.Simulator.LoginRequestSimualtor;

namespace Framework.System.Communication.SubSystems.Login
{

	public class NormalLoginSubSystem : ALoginSubSystem
	{
		public NormalLoginSubSystem() : base(new TargetLoginRequest())
		{
		}
	}
}


