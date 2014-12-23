using System;
namespace hds
{
	
	class TailLink{
		
		private object content;
		private TailLink previous;
		private TailLink next;
		private int objectType; //This allows checkings for mixed types
		
		public TailLink(){
			content = null;
			previous = null;
			next = null;
			objectType = 0;
		}
		
		public TailLink(object _content){
			content = null;
			previous = null;
			next = null;
			content = _content;
			objectType = 0;
		}
		
		public TailLink(object _content,int _objectType){
			content = null;
			previous = null;
			next = null;
			content = _content;
			objectType = _objectType;
		}
		
		public int ObjectType{
			get{return objectType;}
			set{objectType = value;}
		}
		
		public object Content {
        	get { return content; }
        	set { content = value; }
    	}
		
		public TailLink Previous {
        	get { return previous; }
        	set { previous = value; }
    	}
		
		public TailLink Next {
        	get { return next; }
        	set { next = value; }
    	}
		
				
	}
	
	public class DynamicTail
	{
		
		private TailLink head;
		private TailLink tail;
		private int size;
		
		public DynamicTail ()
		{
			size = 0;
			head = null;
			tail = null;
		}
		
		public void Clear(){
			//Delete All (sample POP)
			for (int i = 0;i<size;i++){
				DeleteAt(0);
			}
		}
		
		public int Size(){
			return size;
		}
		
		public int TypeAt(int i){
			// Give them null if not available (empty, out of bounds + - )
			if (size == 0 || i>(size-1) || i < 0)
				return -1;

			TailLink temp = head;
			while (i!=0){
				temp = temp.Next; // <<< Fun here
				i--;
			}
			return temp.ObjectType;
		}
		
		public object At(int i){
			
			// Give them null if not available (empty, out of bounds + - )
			if (size == 0 || i>(size-1) || i < 0)
				return null;

			TailLink temp = head;
			while (i!=0){
				temp = temp.Next; // <<< Fun here
				i--;
			}
			return temp.Content;
		}
		
		public void Append(object content){
			if (head == null){
				head = new TailLink(content);
				tail = head;
				size++;
			}
			else{
				tail.Next = new TailLink(content);
				tail.Next.Previous = tail;
				tail = tail.Next; // <<< fun here too
				size++;
			}
		}
		
		public void Append(object content,int objectType){
			if (head == null){
				head = new TailLink(content, objectType);
				tail = head;
				size++;
			}
			else{
				tail.Next = new TailLink(content, objectType);
				tail.Next.Previous = tail;
				tail = tail.Next; // <<< fun here too
				size++;
			}
		}
		
		
		public int DeleteAt(int i){
			// Return 1 if deleted, 0  (or less) if not
			if (size == 0 || i>(size-1) || i < 0)
				return -1;
			
			TailLink temp; // Temporary used object
			
			if (i==0){ //deleting the head
				temp = head.Next;
				head = null; //Garbage collector, hi!
				head = temp;
				size--;
				return 1;
			}
			
			if (i==(size-1)){ //deleting the tail
				temp = tail.Previous;
				tail = null; // Garbage collector, hi again!
				tail = temp;
				size--;
				return 1;
			}
			
			
			
			// deleting any other object
			temp = head;
			while (i!=0){
				temp = temp.Next; // Look for it
				i--;
			}
			
			/// A => B => C
			// (deleting B here)
			TailLink aux = temp;
			
			aux.Next.Previous = aux.Previous;
			aux.Previous.Next = aux.Next;
			aux = null;
			size--;
			return 1;
			
		}
		
		
		
	}
}

