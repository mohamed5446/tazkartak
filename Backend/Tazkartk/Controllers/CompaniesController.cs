using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tazkartk.DTO.CompanyDTOs;
using Tazkartk.Interfaces;

namespace Tazkartk.Controllers
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

        [HttpGet]
        public async Task<IActionResult>GetAll()
        {
            var companies= await _companyService.GetAllCompanies();
              return companies==null ? NotFound() : Ok(companies);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCompanyById(int id)
        {
            var company=await _companyService.GetCompanyDetailsById(id);
            return company==null ? NotFound("company not found") : Ok(company);
        }
        [HttpPost]
        public async Task<IActionResult>CreateCompany(CompanyRegisterDTO DTO)
        {
            var result = await _companyService.CreateCompany(DTO);
            return result ==null ?BadRequest(): Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult>EditCompany(int id,[FromForm]CompanyEditDTO DTO)
        {
            var company=await _companyService.GetCompanyById(id);
            if (company == null) return NotFound();
            var updated = await _companyService.EditCompany(company, DTO);
            return Ok(new { message = "edit succeed", User = updated });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult>DeleteCompany(int id)
        {
          var company=await _companyService.GetCompanyById(id);
            if (company == null) return NotFound();
            await _companyService.DeleteCompany(company);
            return NoContent();

        }
    }
}
