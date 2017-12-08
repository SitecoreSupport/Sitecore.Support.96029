using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using Sitecore.Analytics.Pipelines.ParseReferrer;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.StringExtensions;
using Sitecore.Xml;

namespace Sitecore.Support.Analytics.Pipelines.ParseReferrer
{
  public class AddNotProvidedKeywords : ParseReferrerBaseProcessor
  {
    private List<string> hostNamesList = new List<string>();

    private void AddHostParameterName(XmlNode configNode)
    {
      Assert.ArgumentNotNull(configNode, "configNode");

      string attribute = XmlUtil.GetAttribute("hostname", configNode);
      if (attribute != null)
      {
        this.hostNamesList.Add(attribute);
      }
    }

    private void GetHostNamesList()
    {
      XmlNode configNode = Factory.GetConfigNode("pipelines/parseReferrer/processor[1]/engines");
      if (configNode == null)
      {
        Log.Warn("Sitecore.Support.96029: The search engines section was not found", this);
        return;
      }

      foreach (XmlNode configNode2 in configNode.ChildNodes)
      {
        this.AddHostParameterName(configNode2);
      }
    }

    public override void Process(ParseReferrerArgs args)
    {
      this.GetHostNamesList();

      if (args.Interaction.Keywords.IsNullOrEmpty() && this.hostNamesList.Any((string x) => args.UrlReferrer.DnsSafeHost.Contains(x)))
      {
        args.Interaction.Keywords = "(not provided)";
      }
    }
  }
}