namespace hds
{
    /// <summary>
    /// Ranged combat requests (RPC 0x41). Interlock / close combat lives in hds.Game.Combat.CloseCombatService.
    /// </summary>
    public class CombatHandler
    {
        private readonly WorldClient client;

        public CombatHandler(WorldClient client)
        {
            this.client = client;
        }

        public void ProcessRangeCombatRequest(ref byte[] packet)
        {
            // ToDo: free attacks with weapons (payload: u16 target view, u16 target spawn)
        }
    }
}
