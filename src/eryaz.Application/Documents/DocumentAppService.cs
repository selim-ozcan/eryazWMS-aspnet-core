using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.UI;
using eryaz.Customers;
using eryaz.Documents.Dto;
using eryaz.Movements;
using eryaz.Warehouses;
using Microsoft.EntityFrameworkCore;
using eryaz.Products;
using eryaz.Customers.Dto;
using System.Reflection.PortableExecutable;
using eryaz.Products.Dto;

namespace eryaz.Documents
{
    public class DocumentAppService : ApplicationService
    {
        private readonly DocumentHeaderManager _documentHeaderManager;
        private readonly DocumentDetailManager _documentDetailManager;
        private readonly ProductManager _productManager;
        private readonly CustomerManager _customerManager;
        private readonly MovementManager _movementManager;
        private readonly WarehouseManager _warehouseManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public DocumentAppService(
            DocumentHeaderManager documentHeaderManager,
            DocumentDetailManager documentDetailManager,
            ProductManager productManager,
            CustomerManager customerManager,
            MovementManager movementManager,
            WarehouseManager warehouseManager,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _documentHeaderManager = documentHeaderManager;
            _documentDetailManager = documentDetailManager;
            _productManager = productManager;
            _customerManager = customerManager;
            _movementManager = movementManager;
            _warehouseManager = warehouseManager;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task CreateDocumentAsync(CreateDocumentDto input)
        {
            // Check if document already exists.
            var inputDocumentNumber = input.documentHeader.DocumentNumber;
            var documentHeader = _documentHeaderManager.GetAllDocumentHeaders(d => d.DocumentNumber == inputDocumentNumber).AsNoTracking().FirstOrDefault();
            if(documentHeader != null) throw new UserFriendlyException("A document with the given document number already exists!");

            // Check if customer exists.
            var customerId = input.documentHeader.CustomerId;
            var customer = await _customerManager.GetCustomerByIdAsync(customerId);
            if (customer == null) throw new UserFriendlyException("Customer does not exists!");

            // Check if product exist.
            var documentDetails = input.documentDetails;
            foreach (var detail in documentDetails)
            {
                var product = await _productManager.GetProductByIdAsync(detail.ProductId);
                if(product == null) throw new UserFriendlyException("Product with ID" + detail.ProductId + "does not exists!");
            }

            // Map Dto's to Entities.
            var documentHeaderMapped = ObjectMapper.Map<DocumentHeader>(input.documentHeader);
            var documentDetailsMapped = ObjectMapper.Map<List<DocumentDetail>>(input.documentDetails);

            // Create document header.
            await _documentHeaderManager.CreateDocumentHeaderAsync(documentHeaderMapped);
            await _unitOfWorkManager.Current.SaveChangesAsync();

            // Create document details.
            foreach(var documentDetail in documentDetailsMapped)
            {
                documentDetail.DocumentHeaderId = documentHeaderMapped.Id; 
                await _documentDetailManager.CreateDocumentDetailAsync(documentDetail);
            }

            // Create movements of the document.
            await CreateDocumentMovementsAsync(documentDetailsMapped);
        }

        public async Task<DocumentHeaderDto> GetDocumentHeaderAsync(int id)
        {
            var documentHeader = await _documentHeaderManager.GetAllDocumentHeaders(header => header.Id == id).Include(header => header.Customer).Include(header => header.DocumentDetails).FirstOrDefaultAsync();

            var customerMapped = ObjectMapper.Map<CustomerDto>(documentHeader.Customer);
            var documentHeaderMapped = ObjectMapper.Map<DocumentHeaderDto>(documentHeader);
            documentHeaderMapped.CustomerDto = customerMapped;


            return documentHeaderMapped;
        }

        public async Task<List<DocumentHeaderDto>> GetAllDocumentHeadersAsync()
        {
            var documentHeaders = _documentHeaderManager.GetAllDocumentHeaders();
            return ObjectMapper.Map<List<DocumentHeaderDto>>(await documentHeaders.ToListAsync());

        }

        public async Task<PagedResultDto<DocumentHeaderDto>> GetAllDocumentHeadersPagedAsync(PagedDocumentResultRequestDto input)
        {
            int count;
            async Task<List<DocumentHeader>> GetAllDocumentHeaders(params Expression<Func<DocumentHeader, bool>>[] predicates)
            {
                var documentHeaders = _documentHeaderManager.GetAllDocumentHeaders(predicates).Include(documentHeader => documentHeader.Customer).WhereIf(!string.IsNullOrEmpty(input.Keyword),
                  documentHeader =>
                  documentHeader.Customer.CustomerCode.Contains(input.Keyword) ||
                  documentHeader.DocumentNumber.Contains(input.Keyword) ||
                  documentHeader.Customer.Title.Contains(input.Keyword));

                count = documentHeaders.Count();
                documentHeaders = documentHeaders.Skip(input.SkipCount).Take(input.MaxResultCount);

                return documentHeaders.ToList();
            }

            List<DocumentHeader> documentHeadersToReturn;
            if (input.IncludeDeleted == null)
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    documentHeadersToReturn = await GetAllDocumentHeaders();
                }
            }
            else if (input.IncludeDeleted == true)
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    documentHeadersToReturn = await GetAllDocumentHeaders(documentHeader => documentHeader.IsDeleted == true);
                }
            }
            else
            {
                documentHeadersToReturn = await GetAllDocumentHeaders();
            }

            var mappedHeaderList = ObjectMapper.Map<List<DocumentHeaderDto>>(documentHeadersToReturn);
            for (int i = 0; i < mappedHeaderList.Count; i++)
            {
                mappedHeaderList[i].CustomerDto = ObjectMapper.Map<CustomerDto>(documentHeadersToReturn[i].Customer);
            }

            return new PagedResultDto<DocumentHeaderDto>
            {
                Items = mappedHeaderList,
                TotalCount = count,
            };
        }

        private async Task CreateDocumentMovementsAsync(List<DocumentDetail> documentDetails)
        {
            var entryWarehouse = await _warehouseManager.GetWarehouseWhereAsync(warehouse => warehouse.WarehouseType == WarehouseType.Entrance);

            foreach (var detail in documentDetails)
            {
                var movementToAdd = new Movement()
                {
                    MovementDate = DateTime.Now,
                    MovementType = MovementType.Entry,
                    Product = detail.Product,
                    ProductId = detail.ProductId,
                    Stock = detail.Stock,
                    Warehouse = entryWarehouse,
                    WarehouseId = entryWarehouse.Id
                };

                await _movementManager.CreateMovementAsync(movementToAdd);
            }
        }

        public async Task<List<DocumentDetailDto>> GetDetailsOfDocumentAsync(int documentHeaderId)
        {
            var documentDetails = await _documentDetailManager.GetAllDocumentDetails(detail => detail.DocumentHeaderId == documentHeaderId).Include(detail => detail.Product).ToListAsync();

            var documentDetailsMapped = ObjectMapper.Map<List<DocumentDetailDto>>(documentDetails);
            for (int i = 0; i < documentDetails.Count; i++)
            {
                documentDetailsMapped[i].ProductDto = ObjectMapper.Map<ProductDto>(documentDetails[i].Product);
            }
            
            return documentDetailsMapped;
        }

        public async Task CompleteDetailOfDocument(int documentHeaderId, int detailId)
        {
            var documentHeader = await _documentHeaderManager.GetAllDocumentHeaders(header => header.Id == documentHeaderId).AsNoTracking().FirstOrDefaultAsync();
            if (documentHeader == null) throw new UserFriendlyException("A document with the given id does not exist!");

            var documentDetail = await _documentDetailManager.GetAllDocumentDetails(detail => detail.Id == detailId).Include(detail => detail.Product).FirstOrDefaultAsync();
            if (documentDetail == null) throw new UserFriendlyException("A document detail with given id does not exist!");

            var entryWarehouse = await _warehouseManager.GetAllWarehouses(warehouse => warehouse.WarehouseType == WarehouseType.Entrance).FirstOrDefaultAsync();
            var mainWarehouse = await _warehouseManager.GetAllWarehouses(warehouse => warehouse.WarehouseType == WarehouseType.Main).FirstOrDefaultAsync();

            var exitMovement = new Movement()
            {
                MovementDate = DateTime.Now,
                MovementType = MovementType.Exit,
                Product = documentDetail.Product,
                ProductId = documentDetail.Product.Id,
                Stock = documentDetail.Stock,
                Warehouse = entryWarehouse,
                WarehouseId = entryWarehouse.Id
            };

            var entryMovement = new Movement()
            {
                MovementDate = DateTime.Now,
                MovementType = MovementType.Entry,
                Product = documentDetail.Product,
                ProductId = documentDetail.Product.Id,
                Stock = documentDetail.Stock,
                Warehouse = mainWarehouse,
                WarehouseId = mainWarehouse.Id
            };

            documentDetail.IsCompleted = true;
            await _movementManager.CreateMovementAsync(exitMovement);
            await _movementManager.CreateMovementAsync(entryMovement);
        }

        public async Task DeleteDetailFromDocument(int documentHeaderId, int detailId)
        {
            var documentHeader = await _documentHeaderManager.GetAllDocumentHeaders(header => header.Id == documentHeaderId).AsNoTracking().FirstOrDefaultAsync();
            if (documentHeader == null) throw new UserFriendlyException("A document with the given id does not exist!");

            var documentDetail = await _documentDetailManager.GetAllDocumentDetails(detail => detail.Id == detailId).Include(detail => detail.Product).FirstOrDefaultAsync();
            if (documentDetail == null) throw new UserFriendlyException("A document detail with given id does not exist!");
            if (documentDetail.IsCompleted == true) throw new UserFriendlyException("A completed detail cannot be deleted!");

            var entryWarehouse = await _warehouseManager.GetAllWarehouses(warehouse => warehouse.WarehouseType == WarehouseType.Entrance).FirstOrDefaultAsync();
            var exitMovement = new Movement()
            {
                MovementDate = DateTime.Now,
                MovementType = MovementType.Exit,
                Product = documentDetail.Product,
                ProductId = documentDetail.Product.Id,
                Stock = documentDetail.Stock,
                Warehouse = entryWarehouse,
                WarehouseId = entryWarehouse.Id
            };

            await _movementManager.CreateMovementAsync(exitMovement);
            await _documentDetailManager.DeleteDocumentDetailAsync(documentDetail.Id);
        }

        public async Task UpdateDetailOfDocument(int documentHeaderId, int detailId, int stock)
        {
            var documentHeader = await _documentHeaderManager.GetAllDocumentHeaders(header => header.Id == documentHeaderId).AsNoTracking().FirstOrDefaultAsync();
            if (documentHeader == null) throw new UserFriendlyException("A document with the given id does not exist!");

            var documentDetail = await _documentDetailManager.GetAllDocumentDetails(detail => detail.Id == detailId).Include(detail => detail.Product).FirstOrDefaultAsync();
            if (documentDetail == null) throw new UserFriendlyException("A document detail with given id does not exist!");
            if (stock <= 0) throw new UserFriendlyException("Stok değeri 0'dan küçük olamaz");

            var entryWarehouse = await _warehouseManager.GetAllWarehouses(warehouse => warehouse.WarehouseType == WarehouseType.Entrance).FirstOrDefaultAsync();

            var movementToAdd = new Movement()
            {
                MovementDate = DateTime.Now,
                MovementType = MovementType.Entry,
                Product = documentDetail.Product,
                ProductId = documentDetail.Product.Id,
                Stock = stock,
                Warehouse = entryWarehouse,
                WarehouseId = entryWarehouse.Id
            };

            documentDetail.Stock += stock;
            await _movementManager.CreateMovementAsync(movementToAdd);
        }

        public async Task FinishDocument(int documentHeaderId)
        {
            var documentHeader = await _documentHeaderManager.GetDocumentHeaderByIdAsync(documentHeaderId);
            var documentDetails = await _documentDetailManager.GetAllDocumentDetails(detail => detail.DocumentHeaderId == documentHeaderId).ToListAsync();
            foreach (var detail in documentDetails)
            {
                if (detail.IsCompleted == false) throw new UserFriendlyException("A document that has uncompleted details cannot be finished!");
            }

            documentHeader.IsCompleted = true;
        }
    }
}