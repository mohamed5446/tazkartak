using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazkartk.Application.DTO.CompanyDTOs;
using Tazkartk.Application.Repository;
using Tazkartk.Domain.Models;
using Tazkartk.Infrastructure.Data;

namespace Tazkartk.Infrastructure.Repositories
{
    internal class CompanyRepository : GenericRepository<Company>,ICompanyRepository
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context, IMapper mapper):base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

     
     
    }
}
