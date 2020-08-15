using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace Bisimulation_Desktop
{
	[XmlRoot(ElementName = "name")]
	public class Name
	{
		[XmlAttribute(AttributeName = "x")]
		public string X { get; set; }
		[XmlAttribute(AttributeName = "y")]
		public string Y { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "location")]
	public class Location
	{
		[XmlElement(ElementName = "name")]
		public Name Name { get; set; }
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName = "x")]
		public string X { get; set; }
		[XmlAttribute(AttributeName = "y")]
		public string Y { get; set; }
		[XmlElement(ElementName = "label")]
		public Label Label { get; set; }
		[XmlElement(ElementName = "committed")]
		public string Committed { get; set; }
		[XmlElement(ElementName = "urgent")]
		public string Urgent { get; set; }
	}

	[XmlRoot(ElementName = "label")]
	public class Label
	{
		[XmlAttribute(AttributeName = "kind")]
		public string Kind { get; set; }
		[XmlAttribute(AttributeName = "x")]
		public string X { get; set; }
		[XmlAttribute(AttributeName = "y")]
		public string Y { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "init")]
	public class Init
	{
		[XmlAttribute(AttributeName = "ref")]
		public string Ref { get; set; }
	}

	[XmlRoot(ElementName = "source")]
	public class Source
	{
		[XmlAttribute(AttributeName = "ref")]
		public string Ref { get; set; }
	}

	[XmlRoot(ElementName = "target")]
	public class Target
	{
		[XmlAttribute(AttributeName = "ref")]
		public string Ref { get; set; }
	}

	[XmlRoot(ElementName = "transition")]
	public class Transition
	{
		[XmlElement(ElementName = "source")]
		public Source Source { get; set; }
		[XmlElement(ElementName = "target")]
		public Target Target { get; set; }
		[XmlElement(ElementName = "label")]
		public List<Label> Label { get; set; }
		[XmlElement(ElementName = "nail")]
		public List<Nail> Nail { get; set; }
	}

	[XmlRoot(ElementName = "template")]
	public class Template
	{
		[XmlElement(ElementName = "name")]
		public Name Name { get; set; }
		[XmlElement(ElementName = "parameter")]
		public string Parameter { get; set; }
		[XmlElement(ElementName = "declaration")]
		public string Declaration { get; set; }
		[XmlElement(ElementName = "location")]
		public List<Location> Location { get; set; }
		[XmlElement(ElementName = "init")]
		public Init Init { get; set; }
		[XmlElement(ElementName = "transition")]
		public List<Transition> Transition { get; set; }
	}

	[XmlRoot(ElementName = "nail")]
	public class Nail
	{
		[XmlAttribute(AttributeName = "x")]
		public string X { get; set; }
		[XmlAttribute(AttributeName = "y")]
		public string Y { get; set; }
	}

	[XmlRoot(ElementName = "nta")]
	public class Nta
	{
		[XmlElement(ElementName = "declaration")]
		public string Declaration { get; set; }
		[XmlElement(ElementName = "template")]
		public List<Template> Template { get; set; }
		[XmlElement(ElementName = "system")]
		public string System { get; set; }
	}

}