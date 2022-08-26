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
    public class DocumentHeaderManager : IDomainService, ITransientDependency
    {

        private readonly IRepository<DocumentHeader, int> _documentHeaderRepository;
        private readonly DocumentDetailManager _documentDetailManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public DocumentHeaderManager(
            IRepository<DocumentHeader, int> documentHeaderRepository,
            DocumentDetailManager documentDetailManager,
            IUnitOfWorkManager unitOfWorkManager)

        {
            _documentHeaderRepository = documentHeaderRepository;
            _documentDetailManager = documentDetailManager;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task CreateDocumentHeaderAsync(DocumentHeader DocumentHeader)
        {
            var documentInDb = await GetDocumentHeaderWhereAsync(header => header.DocumentNumber == DocumentHeader.DocumentNumber);
            if (documentInDb != null)
            {
                throw new UserFriendlyException("Document with the given number already exists!");
            }
            await _documentHeaderRepository.InsertAsync(DocumentHeader);
        }

        public async Task<DocumentHeader> GetDocumentHeaderByIdAsync(int Id)
        {
            return await _documentHeaderRepository.FirstOrDefaultAsync(Id);
        }

        public async Task<DocumentHeader> GetDocumentHeaderWhereAsync(params Expression<Func<DocumentHeader, bool>>[] predicates)
        {
            var documents = _documentHeaderRepository.GetAll();
            foreach (var predicate in predicates)
            {
                documents = documents.Where(predicate);
            }

            return await documents.SingleOrDefaultAsync();
        }

        public IQueryable<DocumentHeader> GetAllDocumentHeaders(params Expression<Func<DocumentHeader, bool>>[] predicates)
        {
            var documents = _documentHeaderRepository.GetAll();

            foreach (var predicate in predicates)
            {
                documents = documents.Where(predicate);
            }

            return documents;
        }

        //public async Task<DocumentHeader> GetDocumentHeaderWithDetails()
        //{

        //}

        public async Task DeleteDocumentHeaderAsync(int Id)
        {
            //var documentToDelete = await GetDocumentWhereAsync(document => document.Id == Id);
            //if (documentToDelete != null && documentToDelete.IsDeleted == true)
            //{
            //    throw new UserFriendlyException("User is already deleted!");
            //}
            //var documentsLinkedToDocument = await _documentManager.GetAllDocumentsAsync(false, document => document.DocumentId == Id);
            //if (documentsLinkedToDocument.Count() > 0)
            //{
            //    throw new UserFriendlyException("A document linked to a document cannot be deleted!");
            //}

            //await _documentRepository.DeleteAsync(Id);
        }
    }
}

