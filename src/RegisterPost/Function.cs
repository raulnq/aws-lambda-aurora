using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Library;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace RegisterPost;

public class Function
{
    private PostsRepository _repository;

    public Function()
    {
        var host = Environment.GetEnvironmentVariable("DB_HOST");
        var database = Environment.GetEnvironmentVariable("DB_NAME");
        var user = Environment.GetEnvironmentVariable("DB_USER");
        _repository = new PostsRepository($"host={host};Port=5432;Database={database};Username={user};SSL Mode=Require;TrustServerCertificate=true");
    }

    public async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(APIGatewayHttpApiV2ProxyRequest input, ILambdaContext context)
    {
        var request = JsonSerializer.Deserialize<RegisterPostRequest>(input.Body)!;

        var post = new Post() { Id = Guid.NewGuid(), Description = request.Description, Title = request.Title };

        await _repository.Create(post);

        var response = JsonSerializer.Serialize(new RegisterPostResponse(post.Id));

        return new APIGatewayHttpApiV2ProxyResponse
        {
            Body = response,
            StatusCode = 200,
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };
    }

    public record RegisterPostRequest(string Description, string Title);

    public record RegisterPostResponse(Guid Id);
}
