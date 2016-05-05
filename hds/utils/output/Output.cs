using System;
using System.IO;

namespace hds
{
	public class Output{
		
		static int verbose = 0;
		
		public Output (){}
		
		static public void OptWriteLine(Object obj){
			if (verbose==1)
				Console.WriteLine(obj);
		}
		
		static public void WriteLine(Object obj){
				Console.WriteLine(obj);
                //writeToLogForConsole(obj);
		}


        static public void WriteRpcLog(Object obj)
        {

            try
            {
                StreamWriter w = File.AppendText("UnknownRPC.txt");
                w.WriteLine(obj);
                w.Flush();
                w.Close();
            }
            catch
            {
                // just pass
            }
        }

        static public void WriteDebugLog(Object obj)
        {

            try
            {
                StreamWriter w = File.AppendText("DebugLog.txt");
                w.WriteLine(obj);
                w.Flush();
                w.Close();
            }
            catch
            {
                // just pass
            }
        }

        static public void WritePacketLog(Object obj, string type, string pss, string cseq, string sseq){
            
            writeToLogForConsole(obj);

            string header = "";

            switch (type)
            {
                case "CLIENT":
                    header = "Client->Server";
                    break;

                case "SERVER":
                    header = "Server -> Client";
                    break;

                case "MARGINSERVER":
                    header = "MarginServer -> MarginClient";
                    break;

                case "MARGINCLIENT":
                    header = "MarginClient -> MarginServer";
                    break;
            }
            
            
            try{
                StreamWriter w = File.AppendText("PacketLog.txt");
                if (type == "CLIENT" || type == "SERVER")
                {
                    w.WriteLine(header + "[{0} {1} PSS :{2} CSEQ: {3} SSEQ: {4}]", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString(), pss, cseq, sseq);
                }

                if (type == "MARGINSERVER" || type == "MARGINCLIENT")
                {
                    w.WriteLine(header + "[{0} {1} ]", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                }
                
                w.WriteLine(obj);
                w.WriteLine();
                w.Flush();
                w.Close();
            }
            catch
            {
                // just pass
            }
        }
		
		static public void Write(Object obj){
				Console.Write(obj);
		}

        static public void writeToLogForConsole(Object obj)
        {
            try
            {
                StreamWriter w = File.AppendText("ServerLog.txt");
                w.WriteLine("[{0} {1}] ", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                w.WriteLine(obj);
                w.WriteLine("");
                w.Flush();
                w.Close();
            }
            catch
            {
                // just pass
            }
            

        }

        static public void writeToLogFile(string text){
            /*
            StreamWriter w = File.AppendText("ServerLog.txt");
            w.WriteLine("[{0} {1}] ", DateTime.Now.ToLongTimeString(),DateTime.Now.ToLongDateString());
            w.WriteLine(text);
            w.WriteLine("");
            w.Flush();
            w.Close();
             */

        }

        static public void AppendToFile(string text)
        {
            

            string dateTime = "[" + TimeUtils.getCurrentDateTime() + "] ";
            try
            {
                System.IO.File.AppendAllText("ServerLog.txt", dateTime + text + "\n\n");
            }
            catch (Exception)
            {
                // pass
            }
        }

		static public void WriteTxtFile(string text){
			try{
			System.IO.File.WriteAllText ("log03.txt", text);
			}
			catch(Exception){
				//PASS :D
			}
			
		}
	}
}

