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
using System.Reflection.Metadata;

namespace eryaz.Documents
{
    public class DocumentDetailManager : IDomainService, ITransientDependency
    {

        private readonly IRepository<DocumentDetail, int> _documentDetailRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public DocumentDetailManager(
            IRepository<DocumentDetail, int> documentDetailRepository,
            IUnitOfWorkManager unitOfWorkManager)

        {
            _documentDetailRepository = documentDetailRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task CreateDocumentDetailAsync(DocumentDetail DocumentDetail)
        {
            await _documentDetailRepository.InsertAsync(DocumentDetail);
        }

        public async Task<DocumentDetail> GetDocumentDetailByIdAsync(int Id)
        {
            return await _documentDetailRepository.FirstOrDefaultAsync(Id);
        }

        public async Task<DocumentDetail> GetDocumentDetailWhereAsync(params Expression<Func<DocumentDetail, bool>>[] predicates)
        {
            var documents = _documentDetailRepository.GetAll();
            foreach (var predicate in predicates)
            {
                documents = documents.Where(predicate);
            }

            return await documents.SingleOrDefaultAsync();
        }

        public IQueryable<DocumentDetail> GetAllDocumentDetails(params Expression<Func<DocumentDetail, bool>>[] predicates)
        {
            var documents = _documentDetailRepository.GetAll();

            foreach (var predicate in predicates)
            {
                documents = documents.Where(predicate);
            }

            return documents;
        }

        public async Task DeleteDocumentDetailAsync(int Id)
        {
            await _documentDetailRepository.DeleteAsync(Id);
        }
    }
}

