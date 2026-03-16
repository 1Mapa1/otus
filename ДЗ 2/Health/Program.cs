
namespace Health
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var app = builder.Build();

            app.MapGet("/health/", () =>
            {
                return Results.Ok(new
                {
                    Status = "OK"
                });
            });

            app.Run();
        }
    }
}