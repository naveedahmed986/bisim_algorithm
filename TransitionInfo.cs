using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bisimulation_Desktop
{
    public static class TransitionInfo
    {
        public static Transition GetTransitionBySourceAndTargetId(Template template, string sourceId, string targetId)
        {
            Transition transition = null;
            if (template != null)
            {
                foreach (Transition t in template.Transition)
                {
                    if (t.Source.Ref == sourceId && t.Target.Ref == targetId)
                    {
                        transition = t;
                        break;
                    }
                }
            }
            return transition;
        }

        public static string GetChannelNameFromTransition(Transition transition)
        {
            if(transition.Label != null && transition.Label.Count > 0)
            {
                Label label = (from l in transition.Label where l.Kind.Equals(Constant.TransitionLabelKind.Synchronization) select l).First();
                return label.Text;
            }
            return "";
        }

        public static Label TransitionHasChannel(Transition transition)
        {
            foreach (Label label in transition.Label)
                if (label.Kind.Equals(Constant.TransitionLabelKind.Synchronization))
                    return label;
            return null;
        }
    }
}
