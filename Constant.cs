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
            public static string VariablePostfix = "_";
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
            public static char Sender = '!';
            public static char Receiver = '?';
        }

        public static class TemplateType
        {
            public static string ENV = "ENV";
            public static string Other = "OTHER";
        }

        public static class LocationLabelKind
        {
            public static string Initial = "initial";
            public static string Committed = "committed";
            public static string Urgent = "urgent";
        }

        public static class Pattern
        {
            public static string lineCommentPattern = @"(^\s*[/?|\*])";
            public static string declarationPattern = @"\b(?!int|bool|const|typedef|clock|urgent|broadcast|chan|while|do|for|if|else|switch|case|break|default|return|[^[]*]|[0-9])\w+\b";
            public static string functionPattern = @"^(\s*\w+\s+\w+\s*)\(.*\)";
            public static string functionArgsPattern = @"\b(?!void|int|bool|const|typedef|clock|urgent|broadcast|chan)\w+\b";
            public static string arrayRangePattern = @"\b[a-zA-Z](\w+)*\b";
        }
    }
}
