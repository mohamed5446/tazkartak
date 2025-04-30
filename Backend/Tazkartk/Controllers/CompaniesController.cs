using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tazkartk.DTO;
using Tazkartk.Models.Enums;
using Tazkartk.DTO.CompanyDTOs;
using Tazkartk.Interfaces;
using Tazkartk.DTO.Response;
using Swashbuckle.AspNetCore.Annotations;

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

       
        //[HttpDelete("del")]
        //public async Task<IActionResult>ChangeLogos()
        //{
        //    await _companyService.changeLogos();
        //    return Ok();
        //}
        
        [HttpGet]
        [SwaggerOperation(Summary = "List All Companies")]
        public async Task<IActionResult>GetAll()
        {
            var companies= await _companyService.GetAllCompaniesAsync();
              return Ok(companies);
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Get Company By Id")]

        public async Task<IActionResult> GetCompanyById(int id)
        {
            var Company =await _companyService.GetCompanyByIdAsync(id);
            return Company == null ? NotFound("Company not found") : Ok(Company);  
        }

       // [Authorize(Roles = "Admin , Company")]
        [HttpPost]
        [SwaggerOperation(Summary = "Add Company")]

        public async Task<IActionResult>CreateCompany(CompanyRegisterDTO DTO)
        {          
            var result = await _companyService.CreateCompanyAsync(DTO);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("{Id:int}")]
       // [Authorize(Roles = "Admin , Company")]
        [SwaggerOperation(Summary = "Edit Company")]

        public async Task<IActionResult>EditCompany(int Id,[FromForm]CompanyEditDTO DTO)
        {
            var result = await _companyService.EditCompanyAsync(Id, DTO);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("{id:int}")]
       // [Authorize(Roles = "Admin , Company")]
        [SwaggerOperation(Summary = "Delete Company")]

        public async Task<IActionResult>DeleteCompany(int id)
        {
            var result=await _companyService.DeleteCompanyAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
