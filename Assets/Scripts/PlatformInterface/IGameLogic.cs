using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.PlatformInterface
{
	public interface IGameLogic
	{
		void Initialize();
		void Update();
		void CleanUp();
	}
}
