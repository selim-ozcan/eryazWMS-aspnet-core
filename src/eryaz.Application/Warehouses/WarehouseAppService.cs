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
using eryaz.Warehouses.Dto;
using eryaz.Warehouses;
using eryaz.Documents;
using eryaz.Movements;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Uow;
using System.Linq.Expressions;

namespace eryaz.Warehouses
{
    public class WarehouseAppService : ApplicationService
    {
        private readonly WarehouseManager _warehouseManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public WarehouseAppService(
            WarehouseManager warehouseManager,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _warehouseManager = warehouseManager;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task CreateWarehouseAsync(CreateWarehouseDto input)
        {
            var newWarehouse = ObjectMapper.Map<Warehouse>(input);
            await _warehouseManager.CreateWarehouseAsync(newWarehouse);
        }

        public async Task<WarehouseDto> GetWarehouseAsync(int Id)
        {
            var warehouse = await _warehouseManager.GetWarehouseWhereAsync(warehouse => warehouse.Id == Id);
            var newWarehouse = ObjectMapper.Map<WarehouseDto>(warehouse);
            return newWarehouse;
        }

        public async Task<List<WarehouseDto>> GetAllWarehousesAsync()
        {
            var warehouses = _warehouseManager.GetAllWarehouses();
            return ObjectMapper.Map<List<WarehouseDto>>(await warehouses.ToListAsync());

        }

        public async Task<PagedResultDto<WarehouseDto>> GetAllWarehousesPagedAsync(PagedWarehouseResultRequestDto input)
        {
            int count;
            async Task<List<Warehouse>> GetAllWarehouses(params Expression<Func<Warehouse, bool>>[] predicates)
            {
                var warehouses = _warehouseManager.GetAllWarehouses(predicates).WhereIf(!string.IsNullOrEmpty(input.Keyword),
                  warehouse =>
                  warehouse.WarehouseName.Contains(input.Keyword) ||
                  warehouse.WarehouseCode.Contains(input.Keyword));

                count = warehouses.Count();
                warehouses = warehouses.Skip(input.SkipCount).Take(input.MaxResultCount);

                return await warehouses.ToListAsync();
            }

            List<Warehouse> warehousesToReturn;
            if (input.IncludeDeleted == null)
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    warehousesToReturn = await GetAllWarehouses();
                }
            }
            else if (input.IncludeDeleted == true)
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    warehousesToReturn = await GetAllWarehouses(warehouse => warehouse.IsDeleted == true);
                }
            }
            else
            {
                warehousesToReturn = await GetAllWarehouses();
            }

            return new PagedResultDto<WarehouseDto>
            {
                Items = ObjectMapper.Map<List<WarehouseDto>>(warehousesToReturn),
                TotalCount = count,
            };
        }

        public async Task UpdateWarehouse(WarehouseDto input)
        {
            await _warehouseManager.UpdateWarehouseAsync(ObjectMapper.Map<Warehouse>(input));
        }

        public async Task DeleteWarehouse(int id)
        {
            await _warehouseManager.DeleteWarehouseAsync(id);
        }
    }
}