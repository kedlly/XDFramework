
using TargetLoginRequest = Assets.CommunicationSystem.Simulator.LoginRequestSimualtor;

namespace Assets.CommunicationSystem.SubSystems.Login
{

	public class NormalLoginSubSystem : ALoginSubSystem
	{
		public NormalLoginSubSystem() : base(new TargetLoginRequest())
		{
		}
	}
}


