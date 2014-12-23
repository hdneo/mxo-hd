
using System;

namespace hds
{
	
	
	public class MarginException: Exception
	{
		
		// Autopass errorString to Exception class
		public MarginException(string errorString):base(errorString){

		}
	
		// We want a customized ToString method, so override
		public override String ToString(){
			return "Margin error: "+this.Message;
		}

	}
}
