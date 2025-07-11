using Playground.Essentials;
using System.Globalization;

namespace Playground.Stl;

public static class StlReader
{
    public static StlSolid LoadSolidFromFile(string filePath)
    {
        return IsBinaryFormat(filePath) 
             ? ReadBinaryFile(filePath) 
             : ReadTextFile(filePath);
    }

    private static StlSolid ReadBinaryFile(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        using var reader = new BinaryReader(stream);

        reader.ReadBytes(80);

        uint triangleCount = reader.ReadUInt32();

        var solid = new StlSolid();

        while (triangleCount > 0)
        {
            solid.Facets.Add(ReadStlTriangle(reader));

            --triangleCount;
        }

        return solid;
    }

    private static StlSolid ReadTextFile(string filePath)
    {
        return ReadSolid(File.ReadAllLines(filePath));
    }

    private static bool IsBinaryFormat(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        using var reader = new BinaryReader(stream);
        
        if (stream.Length < 84)
        {
            return false;
        }

        byte[] header = reader.ReadBytes(80);

        uint triangleCount = reader.ReadUInt32();

        long expectedSize = 84 + triangleCount * 50; // 50 is the size of the triangle in bytes

        return stream.Length == expectedSize;
    }

    private static StlSolid ReadSolid(IReadOnlyList<string> lines)
    {
        if (lines.Count < 2)
        {
            throw new FormatException("Incorrect file format.");
        }

        var line = lines[0];
        var parts = SplitLine(line, 2);

        if (parts[0] != "solid")
        {
            throw new FormatException("Incorrect file format");
        }

        line = lines[lines.Count - 1];
        parts = SplitLine(line, 2);

        if (parts[0] != "endsolid")
        {
            throw new FormatException("Incorrect file format");
        }

        var solid = new StlSolid();

        for (int i = 1; i < lines.Count - 1; i += 7) // ignore solid and endsolid
        {
            solid.Facets.Add(ParseStlTriangle(lines, i));
        }

        return solid;
    }

    private static StlTriangle ParseStlTriangle(IReadOnlyList<string> lines, int offset)
    {
        if (lines.Count - offset < 7)
        {
            throw new InvalidOperationException("Incorrect file format");
        }

        // line : "facet normal .."
        var parts = SplitLine(lines[offset++], 5);

        if (parts[0] != "facet" || parts[1] != "normal")
        {
            throw new FormatException("Incorrect file format");
        }

        var facet = new StlTriangle
        {
            Normal = new Vector3<float>(
                ParseFloat32Invariant(parts[2]),
                ParseFloat32Invariant(parts[3]),
                ParseFloat32Invariant(parts[4]))
        };

        //next line : "outer loop"
        parts = SplitLine(lines[offset++], 2);

        if (parts[0] != "outer" || parts[1] != "loop")
        {
            throw new FormatException("Incorrect file format");
        }

        // next 3 lines
        facet.Vertex1 = ParseVertex(lines[offset++]);
        facet.Vertex2 = ParseVertex(lines[offset++]);
        facet.Vertex3 = ParseVertex(lines[offset++]);

        // next line : "endloop"
        parts = SplitLine(lines[offset++], 1);

        if (parts[0] != "endloop")
        {
            throw new FormatException("Incorrect file format");
        }

        // next line : "endfacet"
        parts = SplitLine(lines[offset++], 1);

        if (parts[0] != "endfacet")
        {
            throw new FormatException("Incorrect file format");
        }

        return facet;
    }

    private static Vector3<float> ParseVertex(string line)
    {
        var parts = SplitLine(line, 4);

        if (parts[0] != "vertex")
        {
            throw new FormatException("Incorrect file format");
        }

        return new Vector3<float>(
            ParseFloat32Invariant(parts[1]),
            ParseFloat32Invariant(parts[2]),
            ParseFloat32Invariant(parts[3]));
    }

    private static string TrimLine(string line)
    {
        if (line is null)
        {
            throw new FormatException("Incorrect file format");
        }

        return line.Trim();
    }

    private static string[] SplitLine(string line, int requiredCount)
    {
        line = TrimLine(line);

        var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != requiredCount)
        {
            throw new FormatException("Incorrect file format");
        }

        return parts;
    }

    private static StlTriangle ReadStlTriangle(BinaryReader reader)
    {
        var facets = new StlTriangle
        {
            Normal  = ReadVector3(reader),
            Vertex1 = ReadVector3(reader),
            Vertex2 = ReadVector3(reader),
            Vertex3 = ReadVector3(reader),
        };

        // custom attributes.
        reader.ReadBytes(reader.ReadUInt16());

        return facets;
    }

    private static Vector3<float> ReadVector3(BinaryReader reader)
    {
        return new Vector3<float>(
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle());
    }

    private static float ParseFloat32Invariant(string text)
    {
        return float.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);
    }
}
