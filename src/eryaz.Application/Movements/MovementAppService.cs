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
using eryaz.Movements;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Uow;
using System.Linq.Expressions;
using eryaz.Customers.Dto;
using eryaz.Documents.Dto;
using eryaz.Products.Dto;
using eryaz.Warehouses.Dto;

namespace eryaz.Movements
{
    public class MovementAppService : ApplicationService
    {
        private readonly MovementManager _movementManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public MovementAppService(
            MovementManager movementManager,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _movementManager = movementManager;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task CreateMovementAsync(CreateMovementDto input)
        {
            var newMovement = ObjectMapper.Map<Movement>(input);
            await _movementManager.CreateMovementAsync(newMovement);
        }

        public async Task<MovementDto> GetMovementAsync(int Id)
        {
            var movement = await _movementManager.GetMovementWhereAsync(movement => movement.Id == Id);
            var newMovement = ObjectMapper.Map<MovementDto>(movement);
            return newMovement;
        }

        public async Task<List<MovementDto>> GetAllMovementsAsync()
        {
            var movements = _movementManager.GetAllMovements();
            return ObjectMapper.Map<List<MovementDto>>(await movements.ToListAsync());

        }

        public async Task<PagedResultDto<MovementDto>> GetAllMovementsPagedAsync(PagedMovementResultRequestDto input)
        {
            int count;
            async Task<List<Movement>> GetAllMovements(params Expression<Func<Movement, bool>>[] predicates)
            {
                var movements = _movementManager.GetAllMovements(predicates)
                    .Include(movement => movement.Product)
                    .Include(movement=> movement.Warehouse)
                    .WhereIf(!string.IsNullOrEmpty(input.Keyword),
                  movement =>
                  movement.Product.ProductName.Contains(input.Keyword) ||
                  movement.Product.ProductCode.Contains(input.Keyword) ||
                  movement.Warehouse.WarehouseName.Contains(input.Keyword));

                count = movements.Count();
                movements = movements.Skip(input.SkipCount).Take(input.MaxResultCount);

                return await movements.ToListAsync();
            }

            List<Movement> movementsToReturn;
            if (input.IncludeDeleted == null)
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    movementsToReturn = await GetAllMovements();
                }
            }
            else if (input.IncludeDeleted == true)
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    movementsToReturn = await GetAllMovements(movement => movement.IsDeleted == true);
                }
            }
            else
            {
                movementsToReturn = await GetAllMovements();
            }

            var mappedMovementList = ObjectMapper.Map<List<MovementDto>>(movementsToReturn);
            for (int i = 0; i < mappedMovementList.Count; i++)
            {
                mappedMovementList[i].ProductDto = ObjectMapper.Map<ProductDto>(movementsToReturn[i].Product);
                mappedMovementList[i].WarehouseDto = ObjectMapper.Map<WarehouseDto>(movementsToReturn[i].Warehouse);
            }

            return new PagedResultDto<MovementDto>
            {
                Items = mappedMovementList,
                TotalCount = count,
            };
        }
    }
}