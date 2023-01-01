using System.Data;
using System.Data.Common;

namespace Library;

public class Post
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
}
