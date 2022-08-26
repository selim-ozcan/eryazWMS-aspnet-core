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
using eryaz.Movements;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Uow;
using System.Linq.Expressions;

namespace eryaz.Customers
{
    public class CustomerAppService : ApplicationService
    {
        private readonly CustomerManager _customerManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public CustomerAppService(
            CustomerManager customerManager,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _customerManager = customerManager;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task CreateCustomerAsync(CreateCustomerDto input)
        {
            var newCustomer = ObjectMapper.Map<Customer>(input);
            await _customerManager.CreateCustomerAsync(newCustomer);
        }

        public async Task<CustomerDto> GetCustomerAsync(int Id)
        {
            var customer = await _customerManager.GetCustomerWhereAsync(customer => customer.Id == Id);
            var newCustomer = ObjectMapper.Map<CustomerDto>(customer);
            return newCustomer;
        }

        public async Task<List<CustomerDto>> GetAllCustomersAsync()
        {
            var customers = _customerManager.GetAllCustomers();
            return ObjectMapper.Map<List<CustomerDto>>(await customers.ToListAsync());

        }

        public async Task<PagedResultDto<CustomerDto>> GetAllCustomersPagedAsync(PagedCustomerResultRequestDto input)
        {
            int count;
            async Task<List<Customer>> GetAllCustomers(params Expression<Func<Customer, bool>>[] predicates)
            {
                var customers = _customerManager.GetAllCustomers(predicates).WhereIf(!string.IsNullOrEmpty(input.Keyword),
                  customer =>
                  customer.Title.Contains(input.Keyword) ||
                  customer.CustomerCode.Contains(input.Keyword) ||
                  customer.Email.Contains(input.Keyword));

                count = customers.Count();
                customers = customers.Skip(input.SkipCount).Take(input.MaxResultCount);

                return await customers.ToListAsync();
            }

            List<Customer> customersToReturn;
            if(input.IncludeDeleted == null)
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    customersToReturn = await GetAllCustomers();
                }
            }
            else if(input.IncludeDeleted == true)
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    customersToReturn = await GetAllCustomers(customer => customer.IsDeleted == true);
                }
            }
            else
            {
                customersToReturn = await GetAllCustomers();
            }
            
            return new PagedResultDto<CustomerDto>
            {
                Items = ObjectMapper.Map<List<CustomerDto>>(customersToReturn),
                TotalCount = count,
            };
        }

        public async Task UpdateCustomer(CustomerDto input)
        {
            await _customerManager.UpdateCustomerAsync(ObjectMapper.Map<Customer>(input));         
        }

        public async Task DeleteCustomer(int id)
        {
            await _customerManager.DeleteCustomerAsync(id);
        }
    }
}