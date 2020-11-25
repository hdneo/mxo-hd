namespace hds
{
    public class MissionDialog
    {
        public string text;
        public string type; 
        public enum DIALOG_TRIGGER { ENTER, LEAVE }

        public DIALOG_TRIGGER trigger = DIALOG_TRIGGER.ENTER;
        
    }
}