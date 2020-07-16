using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bisimulation_Desktop
{
    public class TemplateInfo
    {
        public static Dictionary<string, string> templateType = new Dictionary<string, string>();

        public static Template GetTemplateByName(Nta model, string templateName)
        {
            foreach (Template template in model.Template)
                if (!string.IsNullOrEmpty(templateName) && template.Name.Text.Equals(templateName))
                    return template;
            return null;
        }
    }
}
