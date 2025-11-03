
namespace Core.Responses
{
    public class VaultResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null;
        public DateTime Created { get; set; }
    }
}
