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
using AutoMapper;
using System.Linq.Expressions;
using Abp.Collections.Extensions;
using eryaz.Movements;

namespace eryaz.Warehouses
{
    public class WarehouseManager : IDomainService, ITransientDependency
    {

        private readonly IRepository<Warehouse, int> _warehouseRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly MovementManager _movementManager;

        public WarehouseManager(
            IRepository<Warehouse, int> warehouseRepository,
            IUnitOfWorkManager unitOfWorkManager,
            MovementManager movementManager)

        {
            _warehouseRepository = warehouseRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _movementManager = movementManager;
        }

        public async Task CreateWarehouseAsync(Warehouse Warehouse)
        {
            var warehouseInDb = await GetWarehouseWhereAsync(warehouse => warehouse.WarehouseCode == Warehouse.WarehouseCode);
            if (warehouseInDb != null)
            {
                throw new UserFriendlyException("User with the given code already exists!");
            }
            await _warehouseRepository.InsertAsync(Warehouse);
        }

        public async Task<Warehouse> GetWarehouseByIdAsync(int Id)
        {
            return await _warehouseRepository.FirstOrDefaultAsync(Id);
        }
        public async Task<Warehouse> GetWarehouseWhereAsync(params Expression<Func<Warehouse, bool>>[] predicates)
        {
            var warehouses = _warehouseRepository.GetAll();
            foreach (var predicate in predicates)
            {
                warehouses = warehouses.Where(predicate);
            }
            
            return await warehouses.SingleOrDefaultAsync();
        }

        public IQueryable<Warehouse> GetAllWarehouses(params Expression<Func<Warehouse, bool>>[] predicates)
        {
            var warehouses = _warehouseRepository.GetAll();

            foreach (var predicate in predicates)
            {
                warehouses = warehouses.Where(predicate);
            }

            return warehouses;
        }

        public async Task UpdateWarehouseAsync(Warehouse warehouseWithUpdatedInfo)
        {
            var warehouseWithTheSameCode = await _warehouseRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(p => p.WarehouseCode == warehouseWithUpdatedInfo.WarehouseCode);

            if (warehouseWithTheSameCode != null && warehouseWithTheSameCode.Id != warehouseWithUpdatedInfo.Id)
            {
                throw new UserFriendlyException("A warehouse with the given code already exists!");
            }

            await _warehouseRepository.UpdateAsync(warehouseWithUpdatedInfo);
        }

        public async Task DeleteWarehouseAsync(int Id)
        {
            var warehouseToDelete = await GetWarehouseWhereAsync(warehouse => warehouse.Id == Id);
            if (warehouseToDelete != null && warehouseToDelete.IsDeleted == true)
            {
                throw new UserFriendlyException("Warehouse is already deleted!");
            }
            var movementsLinkedToWarehouse = await _movementManager.GetAllMovements(movement => movement.WarehouseId == Id).AsNoTracking().ToListAsync();
            if (movementsLinkedToWarehouse.Count() > 0)
            {
                throw new UserFriendlyException("A warehouse linked to a movement cannot be deleted!");
            }

            await _warehouseRepository.DeleteAsync(Id);
        }
    }
}

