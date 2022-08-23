using System;
using Abp.Domain.Repositories;
using eryaz.Products;

namespace eryaz.products
{
    public class ProductManager
    {
        private readonly IRepository<Product> _productRepository;

        public ProductManager(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public void CreateProduct()
        {

        }
    }
}

