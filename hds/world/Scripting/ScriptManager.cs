using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.CSharp;

namespace hds.world.scripting{

	public class ScriptManager {
	
		private Dictionary<int,Node> scriptStore;

		public ScriptManager () {
			scriptStore = new Dictionary<int, Node>();
		}

		public void AddEntryPoint(int _id, string _ClassName, string _MethodName, Object _CreatedInstance){
			scriptStore[_id] = new Node(){
				MethodName = _MethodName,
				ClassName = _ClassName,
				CreatedInstance = _CreatedInstance
			};
		}

		public bool LaunchScript(int _id, byte[] _rawInfo){
			Node node = null;
			if(scriptStore.TryGetValue(_id,out node)){
				try{
				node.CreatedInstance.GetType().
            				GetMethod(node.MethodName).
            					Invoke(node.CreatedInstance, new object[]{_rawInfo});
				}
				catch(Exception ex){
					Console.WriteLine("Error during launch of the method" + node.MethodName);
					Console.WriteLine(ex.Message);
				}
				return true;
			}
			Console.WriteLine("Script method with id "+_id+" was not registered");
			return false;
		}
	}


	class Node{
		public string MethodName {get;set;}
		public string ClassName { get; set; }
		public Object CreatedInstance {get;set;}
	}
}

