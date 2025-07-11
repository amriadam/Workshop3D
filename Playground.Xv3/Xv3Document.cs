using System.Xml;

namespace Playground.Xv3;

public sealed class Xv3Document
{
    public static Xv3Document LoadFromXmlFile(string filePath)
    {
        var xmlDocument = new XmlDocument
        {
            XmlResolver = null,
        };

        xmlDocument.Load(filePath);

        return Xv3Parser.ParseDocument(xmlDocument);
    }

    public List<Xv3Entity> Entities { get; } 
        = [];

    public void Flat(
        List<Xv3Group>?    groups,
        List<Xv3Polyline>? polylines,
        List<Xv3Polygon>?  polygons)
    {
        if (groups    is null 
         && polylines is null 
         && polygons  is null)
        {
            return;
        }

        foreach (var entity in Entities)
        {
            Flat(entity, groups, polylines, polygons);
        }
    }

    private static void Flat(
        Xv3Entity          entity,
        List<Xv3Group>?    groups,
        List<Xv3Polyline>? polylines,
        List<Xv3Polygon>?  polygons)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        if (entity is Xv3Polyline polyline)
        {
            polylines?.Add(polyline);
            return;
        }

        if (entity is Xv3Polygon polygon)
        {
            polygons?.Add(polygon);
            return;
        }

        if (entity is Xv3Group group)
        {
            groups?.Add(group);

            foreach (var child in group.Children)
            {
                Flat(child, groups, polylines, polygons);
            }

            return;
        }
    }
}
