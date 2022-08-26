using System;
using System.Threading.Tasks;
using System.Linq;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using System.Collections.Generic;
using eryaz.Documents;
using AutoMapper;
using System.Linq.Expressions;
using Abp.Collections.Extensions;

namespace eryaz.Customers
{
    public class CustomerManager : IDomainService, ITransientDependency
    {

        private readonly IRepository<Customer, int> _customerRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly DocumentHeaderManager _documentManager;

        public CustomerManager(
            IRepository<Customer, int> customerRepository,
            IUnitOfWorkManager unitOfWorkManager,
            DocumentHeaderManager documentManager)

        {
            _customerRepository = customerRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _documentManager = documentManager;
        }

        public async Task CreateCustomerAsync(Customer Customer)
        {
            var customerInDb = await GetCustomerWhereAsync(customer => customer.CustomerCode == Customer.CustomerCode);
            if (customerInDb != null)
            {
                throw new UserFriendlyException("User with the given code already exists!");
            }
            await _customerRepository.InsertAsync(Customer);
        }

        public async Task<Customer> GetCustomerByIdAsync(int Id)
        {
            return await _customerRepository.FirstOrDefaultAsync(Id);
        }
        public async Task<Customer> GetCustomerWhereAsync(params Expression<Func<Customer, bool>>[] predicates)
        {
            var customers = _customerRepository.GetAll();
            foreach(var predicate in predicates)
            {
                customers = customers.Where(predicate);
            }
          
            return await customers.SingleOrDefaultAsync();
        }

        public IQueryable<Customer> GetAllCustomers(params Expression<Func<Customer, bool>>[] predicates)
        {
            var customers = _customerRepository.GetAll();
            
            foreach (var predicate in predicates)
            {
                customers = customers.Where(predicate);
            }

            return customers;
        }

        public async Task UpdateCustomerAsync(Customer customerWithUpdatedInfo)
        {
            var customerWithTheSameCode = await _customerRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(p => p.CustomerCode == customerWithUpdatedInfo.CustomerCode);

            if (customerWithTheSameCode != null && customerWithTheSameCode.Id != customerWithUpdatedInfo.Id )
            {
                throw new UserFriendlyException("A customer with the given code already exists!");
            }

            await _customerRepository.UpdateAsync(customerWithUpdatedInfo);
        }

        public async Task DeleteCustomerAsync(int Id)
        {
            //var customerToDelete = await GetCustomerWhereAsync(customer => customer.Id == Id);
            //if(customerToDelete != null && customerToDelete.IsDeleted == true)
            //{
            //    throw new UserFriendlyException("Customer is already deleted!");
            //}
            //var documentsLinkedToCustomer = await _documentManager.GetAllDocuments(document => document.CustomerId == Id).AsNoTracking().ToListAsync();
            //if(documentsLinkedToCustomer.Count() > 0)
            //{
            //    throw new UserFriendlyException("A customer linked to a document cannot be deleted!");
            //}

            //await _customerRepository.DeleteAsync(Id);
        }
    }
}

