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
using eryaz.Movements;

namespace eryaz.Products
{
    public class ProductManager : IDomainService, ITransientDependency
    {

        private readonly IRepository<Product, int> _productRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly DocumentHeaderManager _documentHeaderManager;
        private readonly MovementManager _movementManager;

        public ProductManager(
            IRepository<Product, int> productRepository,
            IUnitOfWorkManager unitOfWorkManager,
            DocumentHeaderManager documentHeaderManager,
            MovementManager movementManager)

        {
            _productRepository = productRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _documentHeaderManager = documentHeaderManager;
            _movementManager = movementManager;
        }

        public async Task CreateProductAsync(Product Product)
        {
            var productInDb = await GetProductWhereAsync(product => product.ProductCode == Product.ProductCode);
            if (productInDb != null)
            {
                throw new UserFriendlyException("User with the given code already exists!");
            }
            await _productRepository.InsertAsync(Product);
        }

        public async Task<Product> GetProductByIdAsync(int Id)
        {
            return await _productRepository.FirstOrDefaultAsync(Id);
        }
        public async Task<Product> GetProductWhereAsync(params Expression<Func<Product, bool>>[] predicates)
        {
            var products = _productRepository.GetAll();
            foreach (var predicate in predicates)
            {
                products = products.Where(predicate);
            }
    
            return await products.SingleOrDefaultAsync();
        }

        public IQueryable<Product> GetAllProducts(params Expression<Func<Product, bool>>[] predicates)
        {
            var products = _productRepository.GetAll();

            foreach (var predicate in predicates)
            {
                products = products.Where(predicate);
            }

            return products;
        }

        public async Task UpdateProductAsync(Product productWithUpdatedInfo)
        {
            var productWithTheSameCode = await _productRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(p => p.ProductCode == productWithUpdatedInfo.ProductCode);

            if (productWithTheSameCode != null && productWithTheSameCode.Id != productWithUpdatedInfo.Id)
            {
                throw new UserFriendlyException("A product with the given code already exists!");
            }

            await _productRepository.UpdateAsync(productWithUpdatedInfo);
        }

        public async Task DeleteProductAsync(int Id)
        {
            var productToDelete = await GetProductWhereAsync(product => product.Id == Id);
            if (productToDelete != null && productToDelete.IsDeleted == true)
            {
                throw new UserFriendlyException("Product is already deleted!");
            }
            var movementsLinkedToProduct = await _movementManager.GetAllMovements(movement => movement.ProductId == Id).AsNoTracking().ToListAsync();
            if (movementsLinkedToProduct.Count() > 0)
            {
                throw new UserFriendlyException("A product linked to a movement cannot be deleted!");
            }

            // product linked to a document header cannot be deleted ekle...

            await _productRepository.DeleteAsync(Id);
        }
    }
}

