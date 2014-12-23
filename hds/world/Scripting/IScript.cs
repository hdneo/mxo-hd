using System;
using System.Collections.Generic;

namespace hds.world.scripting{

	public interface IScript{

		Dictionary<int,string> Register();
		bool Test();
	}
}

