using System;
namespace hds
{
	public class WorldIssueManager
	{
		
		public void handleErrorEvent(string operation, string message,string debug,int level){
			if(level==1)
				Output.WriteLine("[*Critical] Handled error: "+message+"\nDoing: "+operation+"\n"+debug);
			if(level==2)
				Output.WriteLine("[*Major] Handled error: "+message+"\nDoing: "+operation+"\n");
			if(level==3)
				Output.WriteLine("[*Hints] Handled error: "+message);
		}
	}
}

