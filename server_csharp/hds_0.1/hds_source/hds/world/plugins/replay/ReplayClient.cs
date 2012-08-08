using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;


namespace hds
{
	public class ReplayClient
	{
		
		private string fileName;
		
		public ReplayClient (string fileName){
			this.fileName = "replays/"+fileName;
		}
		
		private string getHexValues(string [] rawData){
			
			string result="";
			for(int i = 2;i<rawData.Length;i++){
				if(rawData[i][2]==' ' && rawData[i][1]!=' '){
					
					result=result+Regex.Replace( rawData[i], @"\s", "" );
				}
				else
					break;
			}
			return result;
		}
		
		public ArrayList getData(){
			ArrayList logData = new ArrayList();
			
			// Read the log file
			StreamReader SR = File.OpenText(fileName);
			string content = SR.ReadToEnd();
			SR.Close();
			
			// Split on double new-line
			string[] packets = Regex.Split(content, "\r\n\r\n");

			for (int i = 0;i<packets.Length;i++){
				string[] temp = Regex.Split(packets[i], "\r\n");
			
				
				if(temp.Length>1){ //Ensure not a blank line or whatever
					if(temp[1].StartsWith("PSS")){ // It's an encrypted packet
						string hexValues = getHexValues(temp);
						logData.Add(hexValues);
					}
				}
			}

			return logData;
		}
		
	}
}

