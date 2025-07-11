namespace Playground.Contract.Models
{
    public interface ILineGeometryModel
        : IGeometryModel
    {
        float LineWidth { get; set; }
    }
}
