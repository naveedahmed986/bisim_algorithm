using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bisimulation_Desktop
{
    class Channel
    {
        //templateName, isBroadcaster (is true "!" else "?"), transitionSourceId, transitionTargetId
        List<Tuple<string, bool, string, string>> list = 
            new List<Tuple<string, bool, string, string>>();
        
        public List<Tuple<string, bool, string, string>> GetChannels()
        {
            return list;
        }

        public void AddChannelInfo(string templateName, bool isBroadcaster, string transitionSourceId,string transitionTargetId)
        {
                list.Add(new Tuple<string, bool, string, string>(
                   templateName, isBroadcaster, transitionSourceId, transitionTargetId));
        }
    }
}
