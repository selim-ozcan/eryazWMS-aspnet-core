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
using eryaz.Authorization.Users;
using eryaz.Customers.Dto;
using eryaz.Movements;
using eryaz.Products;
using eryaz.Products.Dto;

namespace eryaz.Products
{
    public class ProductAppService : ApplicationService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Movement> _movementRepository;

        public ProductAppService(IRepository<Product> productRepository, IRepository<Movement> movementRepository)
        {
            _productRepository = productRepository;
            _movementRepository = movementRepository;
        }

        public async Task CreateProduct(CreateProductDto input)
        {
            var product = await _productRepository.FirstOrDefaultAsync(p => p.ProductCode == input.ProductCode);
            if (product != null)
            {
                throw new UserFriendlyException("Girilen stok koduna sahip bir ürün mevcut.");   
            }

            var newProduct = ObjectMapper.Map<Product>(input);
            await _productRepository.InsertAsync(newProduct);
        }

        public async Task<ProductDto> GetProduct(int id)
        {
            var product = await _productRepository.FirstOrDefaultAsync(p => p.Id == id);
            return ObjectMapper.Map<ProductDto>(product);
        }

        public async Task<List<ProductDto>> GetAllProducts()
        {
            var products = await _productRepository.GetAllListAsync();
            return ObjectMapper.Map<List<ProductDto>>(products);

        }

        public PagedResultDto<ProductDto> GetAllProductsPaged(PagedProductResultRequestDto input)
        {
            var products = _productRepository.GetAll()
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Brand.Contains(input.Keyword));
            var count = products.Count();

            return new PagedResultDto<ProductDto>
            {
                Items = ObjectMapper.Map<List<ProductDto>>(products.ToList()),
                TotalCount = count,
            };
        }

        public async Task UpdateProduct(ProductDto input)
        {
            var product = await _productRepository.FirstOrDefaultAsync(p => p.ProductCode == input.ProductCode);
            var productOld = await _productRepository.FirstOrDefaultAsync(p => p.Id == input.Id);
            if (product != null && productOld.Id != product.Id)
            {
                throw new UserFriendlyException("Girilen stok koduna sahip bir ürün mevcut.");
            }

            ObjectMapper.Map(input, productOld);

            await _productRepository.UpdateAsync(productOld);
        }

        public async Task DeleteProduct(int id)
        {
            var movements = _movementRepository.GetAllIncluding(m => m.Product, m => m.Warehouse).Where(d => d.Product.Id == id).ToList();
            var documentFlag = false;
            var movementFlag = false;
            if (movements.Count > 0)
            {
                foreach(Movement movement in movements)
                {
                    if (movement.DocumentId != null)
                    {
                        documentFlag = true;
                    }
                }
                if(documentFlag)
                {
                    
                    foreach(Movement movement in movements)
                    {
                        if (movement != null && movement.Warehouse.WarehouseType != Warehouses.WarehouseType.Entrance) movementFlag = true;
                    }
                    if (movementFlag) throw new UserFriendlyException("Stok kartı bir hareket görmüştür silinemez!");
                    else throw new UserFriendlyException("Stok kartı bir evrak ile ilişkilidir silinemez!");
                }
                
            }

            await _productRepository.DeleteAsync(id);
        }
    }
}