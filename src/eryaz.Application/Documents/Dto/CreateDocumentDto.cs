using System;
using System.Collections.Generic;
using Abp.AutoMapper;
using eryaz.Documents;
using eryaz.Documents.Dto;


namespace eryaz.Documents.Dto
{
    public class CreateDocumentDto
    {
        public DocumentHeaderDto documentHeader { get; set; }
        public List<DocumentDetailDto> documentDetails { get; set; }
    }
}

