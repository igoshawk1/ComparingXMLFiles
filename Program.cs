using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

public class Program
{
    public static void Main()
    {
        string testxml = "C:\\Users\\Administrator\\Desktop\\GetLiveSportsTest.xml";
        string livexml = "C:\\Users\\Administrator\\Desktop\\GetLiveSportsLive.xml";

        XmlComparator.CompareXml(testxml, livexml);
    }
}

public class XmlComparator
{
    private static List<string> differences = new List<string>();

    public static void CompareXml(string testxml, string livexml)
    {
        try
        {
            XDocument doc1 = XDocument.Load(testxml);
            XDocument doc2 = XDocument.Load(livexml);

            Console.WriteLine("Comparing XML files...");

            CompareElements(doc1.Root, doc2.Root);

            Console.WriteLine($"Total differences found: {differences.Count}");
            PrintDifferences();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static void AddDifference(string message)
    {
        differences.Add(message);
    }

    private static void PrintDifferences()
    {
        foreach (var difference in differences)
        {
            Console.WriteLine(difference);
        }
    }

    private static void CompareAttributes(XElement e1, XElement e2)
    {
        var attr1 = e1.Attributes().ToList();
        var attr2 = e2.Attributes().ToList();
        var c = e1.Attribute("name");

        foreach (var a in attr1)
        {
            var a2 = attr2.FirstOrDefault(a2 => a2.Name.LocalName == a.Name.LocalName);
            if (a2 == null)
            {
                AddDifference($"Attribute '{a.Name.LocalName}' missing in the second XML");
            }
            else if (a.Value != a2.Value)
            {
                AddDifference($"{c} Value mismatch for attribute '{a.Name.LocalName}': '{a.Value}' and '{a2.Value}'");
            }
        }
    }

    private static void CompareElements(XElement e1, XElement e2)
    {
        if (e1.Name.LocalName != e2.Name.LocalName)
        {
            AddDifference($"Element name mismatch: '{e1.Name.LocalName}' and '{e2.Name.LocalName}'");
            return;
        }

        CompareAttributes(e1, e2);

        var child1 = e1.Elements().ToList();
        var child2 = e2.Elements().ToList();

        if (child1.Count != child2.Count)
        {
            AddDifference($" Number of child elements mismatch for element '{e1.Name.LocalName}': {child1.Count} and {child2.Count}");
        }
        else
        {
            for (int i = 0; i < child1.Count; i++)
            {
                CompareElements(child1[i], child2[i]);
            }
        }
    }
}
