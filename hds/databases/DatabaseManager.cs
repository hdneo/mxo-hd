using System;
using System.Collections.Generic;
using System.Text;

using hds.databases.interfaces;

namespace hds.databases{
    
    public class DatabaseManager{

        public IAuthDBHandler AuthDbHandler { get; set; }
        public IMarginDBHandler MarginDbHandler { get; set; }
        public IWorldDBHandler WorldDbHandler { get; set; }

    }
}
