using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Playground.Contract;
using Playground.Contract.Camera;
using Playground.Contract.Geometries;
using Playground.Contract.Models;
using Playground.Essentials;
using Playground.Stl;
using Playground.Xv3;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Threading;

namespace Playground.ViewModels;

public sealed partial class ViewportViewModel
    : ObservableObject
{
    private readonly Dispatcher         mDispatcher;
    private readonly IGeometryBuilder   mGeometryBuilder;
    private readonly ModelNameGenerator mModelNameGenerator;

    public ViewportViewModel(Dispatcher dispatcher, IGeometryBuilder geometryBuilder)
    {
        ArgumentNullException.ThrowIfNull(dispatcher);
        ArgumentNullException.ThrowIfNull(geometryBuilder);

        mDispatcher      = dispatcher;
        mGeometryBuilder = geometryBuilder;
        mModelNameGenerator = new ModelNameGenerator();

        var targetPosition = System.Numerics.Vector3.Zero;
        var cameraPosition = new System.Numerics.Vector3(5f, 5f, 5f);

        Camera = new PerspectiveCamera
        {
            Position      = cameraPosition,
            LookDirection = targetPosition - cameraPosition,
        };

        Models = [];
    }

    public PerspectiveCamera Camera { get; }

    public ObservableCollection<IGeometryModel> Models { get; }

    [RelayCommand]
    private void ZoomOut()
    {
        Camera.ZoomOut(1);
    }

    [RelayCommand]
    private async Task ImportFileAsync()
    {
        using var openFileDialog = new OpenFileDialog
        {
            Multiselect      = false,
            Filter           = "All Supported Files (*.xv3;*.stl)|*.*",
            InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exchange")
        };

        if (openFileDialog.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        string filePath = openFileDialog.FileName;
        if (filePath is null)
        {
            return;
        }

        string fileExtension = Path.GetExtension(filePath);

        if (fileExtension.Equals(".stl", StringComparison.OrdinalIgnoreCase))
        {
            await mDispatcher.InvokeAsync(() => ImportStl(filePath));
            return;
        }

        if (fileExtension.Equals(".xv3", StringComparison.OrdinalIgnoreCase))
        {
            await mDispatcher.InvokeAsync(() => ImportXv3(filePath));
            return;
        }
    }

    private void ImportStl(string filePath)
    {
        StlSolid solid = StlReader.LoadSolidFromFile(filePath);

        var model = new MeshGeometryModel(MakeMeshGeometry(solid, new Color3<float>(0f, 0f, 1f)));

        Models.Add(model);
    }

    private void ImportXv3(string filePath)
    {
        Xv3Document document = Xv3Document.LoadFromXmlFile(filePath);

        List<Xv3Polyline> polylines = [];

        document.Flat(null, polylines, null);

        var model = new LineGeometryModel(MakeLineGeometry(polylines, new Color3<float>(0f, 0f, 1f)));

        Models.Add(model);
    }

    private IGeometry MakeLineGeometry(List<Xv3Polyline> polylines, Color3<float> color)
    {
        mGeometryBuilder.BeginLineGeometry();

        foreach (var polyline in polylines)
        {
            int name = mModelNameGenerator.GenerateName();

            var points = polyline.Points;

            mGeometryBuilder.AddPolyline(name, [..points], [.. Enumerable.Repeat(color, points.Count)], false);
        }

        return mGeometryBuilder.BuildGeometry();
    }

    private IGeometry MakeMeshGeometry(StlSolid solid, Color3<float> color)
    {
        int name = mModelNameGenerator.GenerateName();

        mGeometryBuilder.BeginMeshGeometry();

        var facets = solid.Facets;
        int vertexCount = facets.Count * 3;

        var positions = new Vector3<float>[vertexCount];
        var colors    = new Color3<float>[vertexCount];
        var normals   = new Vector3<float>[vertexCount];
        var indices   = new int[vertexCount];
        
        for (int i = 0; i < facets.Count; ++i)
        {
            var facet = solid.Facets[i];

            indices[i * 3    ] = i * 3;
            indices[i * 3 + 1] = i * 3 + 1;
            indices[i * 3 + 2] = i * 3 + 2;

            positions[i * 3    ] = facet.Vertex1;
            positions[i * 3 + 1] = facet.Vertex2;
            positions[i * 3 + 2] = facet.Vertex3;

            normals[i * 3]     = facet.Normal;
            normals[i * 3 + 1] = facet.Normal;
            normals[i * 3 + 2] = facet.Normal;
        }

        Array.Fill(colors, color);

        mGeometryBuilder.AddMesh(name, positions, colors, normals, indices);

        return mGeometryBuilder.BuildGeometry();
    }
}
