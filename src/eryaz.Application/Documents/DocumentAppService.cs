using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.UI;
using eryaz.Customers;
using eryaz.Documents.Dto;
using eryaz.Movements;
using eryaz.Products;
using eryaz.Products.Dto;
using eryaz.Warehouses;

namespace eryaz.Documents
{
    public class DocumentAppService : ApplicationService
    {
        private readonly IRepository<Document> _documentRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Warehouse> _warehouseRepository;
        private readonly IRepository<Movement> _movementRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<DocumentMovementStatus> _documentMovementStatusRepository;

        public DocumentAppService(
            IRepository<Document> documentRepository,
            IRepository<Customer> customerRepository,
            IRepository<Warehouse> warehouseRepository,
            IRepository<Movement> movementRepository,
            IRepository<Product> productRepository,
            IRepository<DocumentMovementStatus> documentMovementStatusRepository)
        {
            _documentRepository = documentRepository;
            _customerRepository = customerRepository;
            _warehouseRepository = warehouseRepository;
            _movementRepository = movementRepository;
            _productRepository = productRepository;
            _documentMovementStatusRepository = documentMovementStatusRepository;
           
        }

        public async Task CreateDocument(CreateDocumentDto input)
        {
            var document = await _documentRepository.FirstOrDefaultAsync(p => p.DocumentNumber == input.DocumentNumber);
            if (document != null)
            {
                throw new UserFriendlyException("Girilen evrak numarasına sahip bir evrak mevcut.");
            }

            var customer = _customerRepository.Get(input.CustomerId);

            if (customer == null)
            {
                throw new UserFriendlyException("Girilen müşteri bulunamadı.");
            }

            List<Movement> movements = new List<Movement>();
            int stockCounter = 0;
            foreach(var productId in input.ProductIds)
            {
                var product = await _productRepository.GetAsync(productId);
                var warehouse = _warehouseRepository.FirstOrDefault(w => w.WarehouseType == WarehouseType.Entrance);
                var movement = new Movement() { MovementDate = DateTime.Now, Product = product, MovementType = MovementType.Entry, Warehouse = warehouse, IsDeleted = false };
                movement.Stock = input.Stocks[stockCounter++];
                var movementEntity = await _movementRepository.InsertAsync(movement);

                movements.Add(movementEntity);
                _documentMovementStatusRepository.Insert(new DocumentMovementStatus
                {
                    Movement = movementEntity,
                    IsCompleted = false
                });
            }

            var documentToInsert = ObjectMapper.Map<Document>(input);
            documentToInsert.Customer = customer;
            documentToInsert.Movements = movements;
            var addedDocument = await _documentRepository.InsertAsync(documentToInsert);


        }

        public async Task<DocumentDto> GetDocument(int id)
        {
            var document = _documentRepository.GetAllIncluding(d => d.Customer, m => m.Movements).FirstOrDefault(d => d.Id == id);

            var movements = _movementRepository.GetAllIncluding(m => m.Product, m => m.Warehouse).Where(m => m.DocumentId == id).ToList();
            document.Movements = movements;

            var movementStatuses = _documentMovementStatusRepository.GetAllList(ms => movements.Contains(ms.Movement));
            
            var mappedDocument = ObjectMapper.Map<DocumentDto>(document);
            mappedDocument.Movements = document.Movements;
            mappedDocument.MovementStatuses = movementStatuses;
            return mappedDocument;
        }

        public List<DocumentDto> GetAllDocuments()
        {
            var documents = _documentRepository.GetAllIncluding(d => d.Customer, m => m.Movements).ToList();

            return ObjectMapper.Map<List<DocumentDto>>(documents);

        }

        public async Task DeleteDocument(int id)
        {
            await _documentRepository.DeleteAsync(id);
        }

        public PagedResultDto<DocumentDto> GetAllDocumentsPaged(PagedDocumentResultRequestDto input)
        {
            var repo = _documentRepository.GetAllIncluding(d => d.Customer)
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.DocumentNumber.Contains(input.Keyword));
            var count = repo.Count();

            var documents = repo.ToList();
            List<DocumentDto> documentsDto = new(count);
            var list = ObjectMapper.Map<List<DocumentDto>>(repo);

            return new PagedResultDto<DocumentDto>
            {
                Items = list,
                TotalCount = count,
            };
        }

        public async Task<DocumentDto> UpdateDocument(DocumentDto input)
        {

            var document = await _documentRepository.FirstOrDefaultAsync(d => d.DocumentNumber == input.DocumentNumber);
            if (document != null)
            {
                throw new UserFriendlyException("Girilen evrak koduna sahip bir evrak mevcut.");
            }

            var documentToReturn = ObjectMapper.Map<Document>(input);

            var updatedDocument = ObjectMapper.Map<DocumentDto>(await _documentRepository.UpdateAsync(documentToReturn));

            return updatedDocument;
        }

        public List<DocumentDto> GetDocumentListByCustomer(int customerId)
        {
            return ObjectMapper.Map<List<DocumentDto>>(_documentRepository.GetAll().Where(p => p.Customer.Id == customerId).ToList());
        }

        public async Task DeleteMovementFromDocument(int documentId, int movementId)
        {
            var document = await _documentRepository.FirstOrDefaultAsync(d => d.Id == documentId);
            if(document == null)
            {
                throw new UserFriendlyException("Verilen doküman mevcut değil");
            }
            var movement = _movementRepository.GetAllIncluding(m => m.Product).FirstOrDefault(m => m.Id == movementId);
            if (movement == null)
            {
                throw new UserFriendlyException("Girilen transfer koduna sahip bir transfer mevcut değil.");
            }
            var movementStatus = await _documentMovementStatusRepository.FirstOrDefaultAsync(ms => ms.Movement.Id == movementId);
            if(movementStatus.IsCompleted)
            {
                throw new UserFriendlyException("Transfer çoktan gerçekleşti :(");
            }

            await _documentMovementStatusRepository.DeleteAsync(movementStatus);

            var entranceWarehouse = await _warehouseRepository.FirstOrDefaultAsync(w => w.WarehouseType == WarehouseType.Entrance);
            var product = await _productRepository.FirstOrDefaultAsync(movement.Product.Id);
            var exitMovement = new Movement()
            {
                MovementDate = DateTime.Now,
                Product = product,
                MovementType = MovementType.Exit,
                Warehouse = entranceWarehouse,
                Stock = movement.Stock
            };

            movement.DocumentId = null;
            await _movementRepository.UpdateAsync(movement);
            await _movementRepository.InsertAsync(exitMovement);
        }

        public async Task UpdateMovementOfDocument(int documentId, int movementId, int stock)
        {
            var document = await _documentRepository.FirstOrDefaultAsync(d => d.Id == documentId);
            if (document == null)
            {
                throw new UserFriendlyException("Verilen doküman mevcut değil");
            }
            var movement = _movementRepository.GetAllIncluding(m => m.Product, m => m.Warehouse).FirstOrDefault(m => m.Id == movementId);
            if (movement == null)
            {
                throw new UserFriendlyException("Girilen transfer koduna sahip bir transfer mevcut değil.");
            }
            if (stock <= 0)
            {
                throw new UserFriendlyException("Stok değeri 0'dan küçük olamaz");
            }

            var movementToAdd = new Movement()
            {
                MovementDate = DateTime.Now,
                Product = movement.Product,
                MovementType = MovementType.Entry,
                Warehouse = movement.Warehouse,
                Stock = stock,
                DocumentId = movement.DocumentId
            };

            await _movementRepository.InsertAsync(movementToAdd);

        }

        public async Task CompleteMovementOfDocument(int documentId, int movementId)
        {
            var document = await _documentRepository.FirstOrDefaultAsync(d => d.Id == documentId);
            if (document == null)
            {
                throw new UserFriendlyException("Verilen doküman mevcut değil");
            }
            var movement = _movementRepository.GetAllIncluding(m => m.Product).FirstOrDefault(m => m.Id == movementId);
            if (movement == null)
            {
                throw new UserFriendlyException("Girilen transfer koduna sahip bir transfer mevcut değil.");
            }
            var movementStatus = await _documentMovementStatusRepository.FirstOrDefaultAsync(ms => ms.Movement.Id == movementId);
            if (movementStatus.IsCompleted)
            {
                throw new UserFriendlyException("Transfer zaten gerçekleşti.");
            }

            var product = await _productRepository.FirstOrDefaultAsync(movement.Product.Id);

            var mainWarehouse = await _warehouseRepository.FirstOrDefaultAsync(w => w.WarehouseType == WarehouseType.Main);
            var entranceWarehouse = await _warehouseRepository.FirstOrDefaultAsync(w => w.WarehouseType == WarehouseType.Entrance);

            var exitMovement = new Movement()
            {
                MovementDate = DateTime.Now,
                Product = product,
                MovementType = MovementType.Exit,
                Warehouse = entranceWarehouse,
                Stock = movement.Stock
            };

            var entryMovement = new Movement()
            {
                MovementDate = DateTime.Now,
                Product = product,
                MovementType = MovementType.Entry,
                Warehouse = mainWarehouse,
                Stock = movement.Stock
            };

            await _movementRepository.InsertAsync(exitMovement);
            await _movementRepository.InsertAsync(entryMovement);

            movementStatus.IsCompleted = true;
            await _documentMovementStatusRepository.UpdateAsync(movementStatus);

            // check the movements that are result of update operations.
            var updateResultMovements = _movementRepository.GetAllIncluding(m => m.Product)
                .Where(m => m.DocumentId == documentId && m.Product.Id == product.Id && m.Id != movementId).ToList();
            foreach (Movement mov in updateResultMovements)
            {
                var otherExitMovement = new Movement()
                {
                    MovementDate = DateTime.Now,
                    Product = product,
                    MovementType = MovementType.Exit,
                    Warehouse = entranceWarehouse,
                    Stock = mov.Stock
                };

                var otherEntryMovement = new Movement()
                {
                    MovementDate = DateTime.Now,
                    Product = product,
                    MovementType = MovementType.Entry,
                    Warehouse = mainWarehouse,
                    Stock = mov.Stock
                };
            }

            var movementStatuses = _documentMovementStatusRepository.GetAllIncluding(ms => ms.Movement)
                .Where(ms => ms.Movement.DocumentId == documentId && ms.Movement.Product.Id == product.Id && ms.Id != movementStatus.Id).ToList();

            foreach(var movStatus in movementStatuses)
            {
                movStatus.IsCompleted = true;
                await _documentMovementStatusRepository.UpdateAsync(movStatus);
            }
        }

        public async Task FinishDocument(int documentId)
        {
            var document = _documentRepository.FirstOrDefault(d => d.Id == documentId);
            if (document == null)
            {
                throw new UserFriendlyException("Verilen doküman mevcut değil");
            }
            if (document.Status.Equals("TAMAMLANDI"))
            {
                throw new UserFriendlyException("Verilen doküman zaten tamamlanmış.");
            }

            var movementStatuses = _documentMovementStatusRepository.GetAllIncluding(ms => ms.Movement).Where(ms => ms.Movement.DocumentId == documentId).ToList();
            if (movementStatuses.Count <= 0) throw new UserFriendlyException("Verilen dokümanda hiç kalem yok!");
            for (int i = 0; i < movementStatuses.Count; i++)
            {
                if (movementStatuses[i].IsCompleted == false){
                    throw new UserFriendlyException("Verilen dokümanda tamamlanmayan kalemler var!");
                }
            }

            document.Status = "TAMAMLANDI";
            await _documentRepository.UpdateAsync(document);
        }
    }
}