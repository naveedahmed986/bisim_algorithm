using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Bisimulation_Desktop
{
    public static class ConvertModel
    {
        //Serialize XML to Nta and returns Nta class object
        public static Nta XMLtoNta(string path)
        {
            Nta nta;
            XmlSerializer serializer = new XmlSerializer(typeof(Nta));
            StreamReader file = new StreamReader(path);
            nta = (Nta)serializer.Deserialize(file);
            file.Close();
            return nta;
        }

        //Serialize Nta to XML and write it on the disk
        public static bool NtatoXML(Nta model)
        {
            if (model != null)
            {
                XmlSerializer xml = new XmlSerializer(typeof(Nta));
                var xmlnsEmpty = new XmlSerializerNamespaces();
                xmlnsEmpty.Add("", "");
                var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//bi_model.xml";
                FileStream file = File.Create(path);
                using (XmlWriter writer = XmlWriter.Create(file))
                {
                    //writer.WriteDocType("nta", "-//Uppaal Team//DTD Flat System 1.1//EN\' \'http://www.it.uu.se/research/group/darts/uppaal/flat-1_1.dtd\'", null, null);
                    xml.Serialize(writer, model, xmlnsEmpty);
                }
                file.Close();
                return true;
            }
            return false;
        }
    }
}
