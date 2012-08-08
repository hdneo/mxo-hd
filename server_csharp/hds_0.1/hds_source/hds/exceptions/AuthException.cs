
using System;

namespace hds
{
	
	
	public class AuthException: Exception
	{

		// Autopass errorString to Exception class
		public AuthException(string errorString):base(errorString){

		}
	
		// We want a customized ToString method, so override
		public override String ToString(){
			return "Auth error: "+this.Message;
		}

	}
}
