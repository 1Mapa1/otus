namespace CatalogService.Application.Common
{
    public sealed class Result
    {
        public bool IsSuccess { get; init; }

        public Error? Error { get; init; }

        public static Result Success() => new() { IsSuccess = true };

        public static Result Failure(Error error) => new() { IsSuccess = false, Error = error };
    }
}
