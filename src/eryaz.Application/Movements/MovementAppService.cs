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
using eryaz.Movements.Dto;
using eryaz.Movements;
using eryaz.Documents;

namespace eryaz.Movements
{
    public class MovementAppService : ApplicationService
    {
        private readonly IRepository<Movement> _movementRepository;
        private readonly IRepository<Document> _documentRepository;

        public MovementAppService(IRepository<Movement> movementRepository, IRepository<Document> documentRepository)
        {
            _movementRepository = movementRepository;
            _documentRepository = documentRepository;
        }

        public async Task CreateMovement(CreateMovementDto input)
        {
            var newMovement = ObjectMapper.Map<Movement>(input);
            await _movementRepository.InsertAsync(newMovement);

            // Stok var mı yok mu kontrol etme mantığı
        }

        public async Task<MovementDto> GetMovement(int Id)
        {
            var movement = await _movementRepository.FirstOrDefaultAsync(c => c.Id == Id);
            if(movement == null)
            {
                throw new UserFriendlyException("Girilen transfer bulunamadı.");
            }
            var newMovement = ObjectMapper.Map<MovementDto>(movement);
            return newMovement;
        }

        public async Task<List<MovementDto>> GetAllMovements()
        {
            var Movements = await _movementRepository.GetAllListAsync();
            return ObjectMapper.Map<List<MovementDto>>(Movements);

        }

        public PagedResultDto<MovementDto> GetAllMovementsPaged(PagedMovementResultRequestDto input)
        {
            var movements = _movementRepository.GetAllIncluding(m => m.Product, movements => movements.Warehouse)
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Warehouse.WarehouseName.Contains(input.Keyword) || x.Product.ProductCode.Contains(input.Keyword));
            var count = movements.Count();

            return new PagedResultDto<MovementDto>
            {
                Items = ObjectMapper.Map<List<MovementDto>>(movements.ToList()),
                TotalCount = count,
            };
        }
    }
}