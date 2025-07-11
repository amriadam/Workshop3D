namespace Playground.Contract.Geometries
{
    public struct GeometryRange
    {
        public int  Name        { get; set; }
        public int  VertexStart { get; set; }
        public int  VertexCount { get; set; }
        public int  IndexStart  { get; set; }
        public int  IndexCount  { get; set; }
    }
}
