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
using eryaz.Products.Dto;
using eryaz.Products;
using eryaz.Documents;
using eryaz.Movements;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Uow;
using System.Linq.Expressions;

namespace eryaz.Products
{
    public class ProductAppService : ApplicationService
    {
        private readonly ProductManager _productManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ProductAppService(
            ProductManager productManager,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _productManager = productManager;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task CreateProductAsync(CreateProductDto input)
        {
            var newProduct = ObjectMapper.Map<Product>(input);
            await _productManager.CreateProductAsync(newProduct);
        }

        public async Task<ProductDto> GetProductAsync(int Id)
        {
            var product = await _productManager.GetProductWhereAsync(product => product.Id == Id);
            var newProduct = ObjectMapper.Map<ProductDto>(product);
            return newProduct;
        }

        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            var products = _productManager.GetAllProducts();
            return ObjectMapper.Map<List<ProductDto>>(await products.ToListAsync());

        }

        public async Task<PagedResultDto<ProductDto>> GetAllProductsPagedAsync(PagedProductResultRequestDto input)
        {
            int count;
            async Task<List<Product>> GetAllProducts(params Expression<Func<Product, bool>>[] predicates)
            {
                var products = _productManager.GetAllProducts(predicates).WhereIf(!string.IsNullOrEmpty(input.Keyword),
                  product =>
                  product.ProductName.Contains(input.Keyword) ||
                  product.ProductCode.Contains(input.Keyword) ||
                  product.Brand.Contains(input.Keyword));

                count = products.Count();
                products = products.Skip(input.SkipCount).Take(input.MaxResultCount);

                return await products.ToListAsync();
            }

            List<Product> productsToReturn;
            if (input.IncludeDeleted == null)
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    productsToReturn = await GetAllProducts();
                }
            }
            else if (input.IncludeDeleted == true)
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    productsToReturn = await GetAllProducts(product => product.IsDeleted == true);
                }
            }
            else
            {
                productsToReturn = await GetAllProducts();
            }

            return new PagedResultDto<ProductDto>
            {
                Items = ObjectMapper.Map<List<ProductDto>>(productsToReturn),
                TotalCount = count,
            };
        }

        public async Task UpdateProduct(ProductDto input)
        {
            await _productManager.UpdateProductAsync(ObjectMapper.Map<Product>(input));
        }

        public async Task DeleteProduct(int id)
        {
            await _productManager.DeleteProductAsync(id);
        }
    }
}
