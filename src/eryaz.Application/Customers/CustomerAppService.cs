using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using eryaz.Customers.Dto;
using eryaz.Customers;
using eryaz.Documents;

namespace eryaz.Customers
{
    public class CustomerAppService : ApplicationService
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Document> _documentRepository;

        public CustomerAppService(IRepository<Customer> customerRepository, IRepository<Document> documentRepository)
        {
            _customerRepository = customerRepository;
            _documentRepository = documentRepository;
        }

        public async Task CreateCustomer(CreateCustomerDto input)
        {
            var customer = await _customerRepository.FirstOrDefaultAsync(p => p.CustomerCode == input.CustomerCode);
            if (customer != null)
            {
                throw new UserFriendlyException("Girilen müşteri koduna sahip bir müşteri mevcut.");   
            }

            var newCustomer = ObjectMapper.Map<Customer>(input);
            await _customerRepository.InsertAsync(newCustomer);
        }

        public async Task<CustomerDto> GetCustomer(int Id)
        {
            var customer = await _customerRepository.FirstOrDefaultAsync(c => c.Id == Id);
            var newCustomer = ObjectMapper.Map<CustomerDto>(customer);
            return newCustomer;
        }

        public async Task<CustomerDto> GetCustomerWithCode(string CustomerCode)
        {
            var customer = await _customerRepository.FirstOrDefaultAsync(c => c.CustomerCode == CustomerCode);
            var newCustomer = ObjectMapper.Map<CustomerDto>(customer);
            return newCustomer;
        }

        public async Task<List<CustomerDto>> GetAllCustomers()
        {
            var customers = await _customerRepository.GetAllListAsync();
            return ObjectMapper.Map<List<CustomerDto>>(customers);

        }

        public PagedResultDto<CustomerDto> GetAllCustomersPaged(PagedCustomerResultRequestDto input)
        {
            var customers = _customerRepository.GetAll()
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Title.Contains(input.Keyword));
            var count = customers.Count();

            return new PagedResultDto<CustomerDto>
            {
                Items = ObjectMapper.Map<List<CustomerDto>>(customers.ToList()),
                TotalCount = count,
            };
        }

        public async Task UpdateCustomer(CustomerDto input)
        {
            var customer = await _customerRepository.FirstOrDefaultAsync(p => p.CustomerCode == input.CustomerCode);
            var customerOld = await _customerRepository.FirstOrDefaultAsync(p => p.Id == input.Id);
            if (customer != null && customerOld.Id != customer.Id)
            {
                throw new UserFriendlyException("Girilen müşteri koduna sahip bir müşteri mevcut.");
            }

            ObjectMapper.Map(input, customerOld);

            await _customerRepository.UpdateAsync(customerOld);
        }

        public async Task DeleteCustomer(int id)
        {
            var documents = _documentRepository.GetAllIncluding(d => d.Customer).Where(d => d.Customer.Id == id).ToList();
            if(documents.Count > 0)
            {
                throw new UserFriendlyException("Evrak ile ilişkili müşteri silinemez!");
            }

            await _customerRepository.DeleteAsync(id);


        }
    }
}