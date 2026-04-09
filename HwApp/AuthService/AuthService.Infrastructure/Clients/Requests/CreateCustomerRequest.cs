namespace AuthService.Infrastructure.Clients.Requests
{
    internal class CreateCustomerRequest
    {
        public Guid Id { get; private set; } 
        
        public string Name { get; private set; }

        public CreateCustomerRequest(Guid userId, string name)
        {
            Id = userId;
            Name = name;
        }
    }
}
