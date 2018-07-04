using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
	public interface IState
	{
		void onEnter();
		void onExit();
	}
}
