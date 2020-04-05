using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bisimulation_Desktop
{
    public static class Constant
    {
        public static class Common
        {
            public static string AuxilaryChannelPostfix = "_aux";
            public static string InputChannelPrefix = "i_";
            public static string OutputChannelPrefix = "o_";
        }
        public static class TransitionLabelKind
        {
            public static string Synchronization = "synchronisation";
            public static string Comment = "comment";
            public static string Select = "select";
            public static string Guard = "guard";
            public static string Update = "update";
        }

        public static class ActionType
        {
            public static char Output = '!';
            public static char Input = '?';
        }

        public static class LocationLabelKind
        {
            public static string Initial = "initial";
            public static string Committed = "committed";
            public static string Urgent = "urgent";
        }
    }
}
