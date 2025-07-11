using Playground.Essentials;
using System.Globalization;
using System.Xml;

namespace Playground.Xv3;

public static class Xv3Parser
{
    public static Xv3Document ParseDocument(XmlDocument input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        var rootElement = input.SelectSingleNode("XV3RootEntity")
            ?? throw new FormatException();

        var output = new Xv3Document();

        foreach (XmlNode node in rootElement.ChildNodes)
        {
            var entity = ParseEntity(node);
            if (entity is null)
            {
                continue;
            }

            output.Entities.Add(entity);
        }

        return output;
    }

    private static Xv3Entity? ParseEntity(XmlNode node)
    {
        if (node is null)
        {
            throw new ArgumentNullException(nameof(node));
        }

        XmlNode childContent = node.FirstChild;

        if (childContent is null)
        {
            return null;
        }

        string childType = GetAttribute(node, "type", string.Empty);

        if (childType == "XV3GroupEntity")
        {
            return ParseGroup(childContent);
        }
    
        if (childType == "XV3Polyline")
        {
            return ParsePolyline(childContent);
        }
    
        if (childType == "XV3Polygon")
        {
            return ParsePolygon(childContent);
        }

        return null;
    }

    private static Xv3Group ParseGroup(XmlNode node)
    {
        var group = new Xv3Group
        {
            Name  = GetAttribute(node, "Name", string.Empty),
            Layer = GetAttribute(node, "Layer", ParseInvariant, -1)
        };

        foreach (XmlNode childNode in node.ChildNodes)
        {
            var entity = ParseEntity(childNode);

            if (entity is null)
            {
                continue;
            }

            group.Children.Add(entity);
        }

        return group;
    }

    private static Xv3Polyline ParsePolyline(XmlNode node)
    {
        var obj = new Xv3Polyline
        {
            Name  = GetAttribute(node, "Name", string.Empty),
            Layer = GetAttribute(node, "Layer", ParseInvariant, -1),
        };

        obj.Points.AddRange(ParsePointList(node));

        return obj;
    }

    private static Xv3Polygon ParsePolygon(XmlNode node)
    {
        var obj = new Xv3Polygon
        {
            Name  = GetAttribute(node, "Name", string.Empty),
            Layer = GetAttribute(node, "Layer", ParseInvariant, -1),
        };

        obj.Points.AddRange(ParsePointList(node));

        return obj;
    }

    private static Vector3<float> ParsePoint(XmlNode node)
    {
        return new Vector3<float>(
            GetAttribute(node, "X", ParseInvariant, 0f),
            GetAttribute(node, "Y", ParseInvariant, 0f),
            GetAttribute(node, "Z", ParseInvariant, 0f));
    }

    private static List<Vector3<float>> ParsePointList(XmlNode node)
    {
        var pointList = new List<Vector3<float>>();

        var pointNodeList = node.SelectNodes("Pt3D");

        if (pointNodeList != null)
        {
            foreach (XmlNode item in pointNodeList)
            {
                if (item is null)
                {
                    continue;
                }

                pointList.Add(ParsePoint(item));
            }
        }

        return pointList;
    }

    private static string? GetAttribute(XmlNode node, string name)
    {
        var attributes = node.Attributes;

        if (attributes is null)
        {
            return null;
        }

        var attribute = attributes[name];

        if (attribute is null)
        {
            return null;
        }

        return attribute.Value;
    }

    private static string GetAttribute(XmlNode node, string name, string fallback)
    {
        return GetAttribute(node, name) ?? fallback;
    }

    private static T GetAttribute<T>(XmlNode node, string name, Func<string, T, T> parse, T fallback)
    {
        string? value = GetAttribute(node, name);

        return value != null 
             ? parse(value, fallback)
             : fallback;
    }

    private static int ParseInvariant(string s, int fallback)
    {
        return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value) 
             ? value 
             : fallback;
    }

    private static float ParseInvariant(string s, float fallback)
    {
        return float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out float value) 
             ? value 
             : fallback;
    }
}
