using AutoMapper;
using CustomerService.Api.Contracts.Requests;
using CustomerService.Api.Contracts.Responses;
using CustomerService.Domain.Entities;

namespace CustomerService.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Customer, CustomerResponse>();
            CreateMap<CreateCustomerRequest, Customer>();
        }
    }
}
