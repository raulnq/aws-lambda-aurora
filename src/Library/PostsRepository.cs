using Amazon.RDS.Util;
using Dapper;
using Npgsql;

namespace Library;

public class PostsRepository
{
    private readonly NpgsqlConnection _connection;

    public PostsRepository(string connectionString)
    {
        _connection = new NpgsqlConnection(connectionString);

        _connection.ProvidePasswordCallback = RequestAwsIamAuthToken;
    }

    private string RequestAwsIamAuthToken(string host, int port, string database, string username)
    {
        return RDSAuthTokenGenerator.GenerateAuthToken(host, port, username);
    }

    public Task Create(Post post)
    {
        return _connection.ExecuteAsync("INSERT INTO public.posts(id, title, description) VALUES (@Id, @Title, @Description)", post);
    }

    public Task<IEnumerable<Post>> List()
    {
        return _connection.QueryAsync<Post>("SELECT id, title, description from public.posts");
    }
}