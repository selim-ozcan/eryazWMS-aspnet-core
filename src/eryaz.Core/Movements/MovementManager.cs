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
using eryaz.Documents;
using AutoMapper;
using System.Linq.Expressions;
using Abp.Collections.Extensions;

namespace eryaz.Movements
{
    public class MovementManager : IDomainService, ITransientDependency
    {

        private readonly IRepository<Movement, int> _movementRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly DocumentHeaderManager _documentHeaderManager;

        public MovementManager(
            IRepository<Movement, int> movementRepository,
            IUnitOfWorkManager unitOfWorkManager,
            DocumentHeaderManager documentHeaderManager)

        {
            _movementRepository = movementRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _documentHeaderManager = documentHeaderManager;
        }

        public async Task CreateMovementAsync(Movement Movement)
        {
            await _movementRepository.InsertAsync(Movement);
        }

        public async Task<Movement> GetMovementByIdAsync(int Id)
        {
            return await _movementRepository.FirstOrDefaultAsync(Id);
        }

        public async Task<Movement> GetMovementWhereAsync(params Expression<Func<Movement, bool>>[] predicates)
        {
            var movements = _movementRepository.GetAll();
            foreach (var predicate in predicates)
            {
                movements = movements.Where(predicate);
            }

            return await movements.SingleOrDefaultAsync();
        }

        public IQueryable<Movement> GetAllMovements(params Expression<Func<Movement, bool>>[] predicates)
        {
            var movements = _movementRepository.GetAll();

            foreach (var predicate in predicates)
            {
                movements = movements.Where(predicate);
            }

            return movements;
        }
    }
}

