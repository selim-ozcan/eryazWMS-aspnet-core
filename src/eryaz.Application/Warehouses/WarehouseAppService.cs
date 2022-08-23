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
using eryaz.Movements;

namespace eryaz.Warehouses
{
    public class WarehouseAppService : ApplicationService
    {
        private readonly IRepository<Warehouse> _warehouseRepository;
        private readonly IRepository<Movement> _movementRepository;

        public WarehouseAppService(IRepository<Warehouse> warehouseRepository, IRepository<Movement> movementRepository)
        {
            _warehouseRepository = warehouseRepository;
            _movementRepository = movementRepository;
        }

        public async Task CreateWarehouse(CreateWarehouseDto input)
        {
            var warehouse = await _warehouseRepository.FirstOrDefaultAsync(p => p.WarehouseCode == input.WarehouseCode);
            if (warehouse != null)
            {
                throw new UserFriendlyException("Girilen depo koduna sahip bir depo mevcut.");
            }

            var newWarehouse = ObjectMapper.Map<Warehouse>(input);
            await _warehouseRepository.InsertAsync(newWarehouse);
        }

        public async Task<WarehouseDto> GetWarehouse(int id)
        {
            var warehouse = await _warehouseRepository.FirstOrDefaultAsync(w => w.Id == id);
            return ObjectMapper.Map<WarehouseDto>(warehouse);
        }

        public async Task<List<WarehouseDto>> GetAllWarehouses()
        {
            var warehouses = await _warehouseRepository.GetAllListAsync();
            return ObjectMapper.Map<List<WarehouseDto>>(warehouses);

        }

        public PagedResultDto<WarehouseDto> GetAllWarehousesPaged(PagedWarehouseResultRequestDto input)
        {
            var warehouses = _warehouseRepository.GetAll()
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.WarehouseName.Contains(input.Keyword));
            var count = warehouses.Count();

            return new PagedResultDto<WarehouseDto>
            {
                Items = ObjectMapper.Map<List<WarehouseDto>>(warehouses.ToList()),
                TotalCount = count,
            };
        }

        public async Task UpdateWarehouse(WarehouseDto input)
        {
            var warehouse = await _warehouseRepository.FirstOrDefaultAsync(p => p.WarehouseCode == input.WarehouseCode);
            var warehouseOld = await _warehouseRepository.FirstOrDefaultAsync(p => p.Id == input.Id);
            if (warehouse != null && warehouseOld.Id != warehouse.Id)
            {
                throw new UserFriendlyException("Girilen stok koduna sahip bir ürün mevcut.");
            }

            ObjectMapper.Map(input, warehouseOld);

            await _warehouseRepository.UpdateAsync(warehouseOld);
        }

        public async Task DeleteWarehouse(int id)
        {
            var movements = _movementRepository.GetAllIncluding(m => m.Warehouse).Where(m => m.Warehouse.Id == id).ToList();
            if (movements.Count > 0)
            {
                throw new UserFriendlyException("Ambar üzerinde stok hareketi bulunmaktadır!");
            }

            await _warehouseRepository.DeleteAsync(id);
        }
    }
}