﻿using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Tazkartk.Application.DTO.CompanyDTOs;
using Tazkartk.Application.DTO.Payments;
using Tazkartk.Application.Interfaces;

namespace Tazkartk.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompaniesController(ICompanyService companyService)
        {
            _companyService = companyService;
        }
        [SwaggerOperation(Summary ="test method :increase company Balance")]
        [HttpPost("{CompanyId}/increase")]
        public async Task<IActionResult>IncreaseBalance(int CompanyId)
        {
            var res = await _companyService.IncreaseCompanyBalance(CompanyId);
            return StatusCode((int)res.StatusCode, res);

        }
        [SwaggerOperation(Summary="List Company Payouts History ")]
        [HttpGet("{CompanyId}/Payouts")]
        public async Task<IActionResult> GetCompanyPayouts(int CompanyId)
        {
            var res = await _companyService.GetCompanyPayouts(CompanyId);
            return Ok(res);
        }

        /// <summary>
        /// Withdraw Comapny Balance to mobile wallet.
        /// </summary>
        /// <remarks>
        /// Test credentials:
        /// 
        /// vodafone: 01023456789  
        /// etisalat: 01123456789  
        /// orange: 01223456789  
        /// bank_wallet: 01123416789  
        /// </remarks>
        [HttpPost("{companyId}/withraw")]
        public async Task<IActionResult> withdrawlbalance(int companyId,WithdrawDTO DTO)
        {
            var res = await _companyService.WithdrawlBalance(companyId,DTO);
            return StatusCode((int)res.StatusCode, res);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "List All Companies")]
        public async Task<IActionResult> GetAll()
        {
            var companies = await _companyService.GetAllCompaniesAsync();
            return Ok(companies);
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Get Company By Id")]
        public async Task<IActionResult> GetCompanyById(int id)
        {
            var Company = await _companyService.GetCompanyByIdAsync(id);
            return Company == null ? NotFound("Company not found") : Ok(Company);
        }

        // [Authorize(Roles = "Admin , Company")]
        [HttpPost]
        [SwaggerOperation(Summary = "Add Company")]
        public async Task<IActionResult> CreateCompany(CompanyRegisterDTO DTO)
        {
            var result = await _companyService.CreateCompanyAsync(DTO);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("{Id:int}")]
        // [Authorize(Roles = "Admin , Company")]
        [SwaggerOperation(Summary = "Edit Company")]
        public async Task<IActionResult> EditCompany(int Id, [FromForm] CompanyEditDTO DTO)
        {
            var result = await _companyService.EditCompanyAsync(Id, DTO);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("{id:int}")]
        // [Authorize(Roles = "Admin , Company")]
        [SwaggerOperation(Summary = "Delete Company")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var result = await _companyService.DeleteCompanyAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }

        //[HttpDelete("del")]
        //public async Task<IActionResult>ChangeLogos()
        //{
        //    await _companyService.changeLogos();
        //    return Ok();
        //}
    }
}
