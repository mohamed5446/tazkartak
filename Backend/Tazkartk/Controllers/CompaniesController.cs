using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tazkartk.DTO;
using Tazkartk.Models.Enums;
using Tazkartk.DTO.CompanyDTOs;
using Tazkartk.Interfaces;
using Tazkartk.DTO.Response;

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
              return Ok(companies);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCompanyById(int id)
        {
            var Company =await _companyService.GetCompanyById(id);
            return Company == null ? NotFound("Company not found") : Ok(Company);  
        }
        [HttpPost]
        public async Task<IActionResult>CreateCompany(CompanyRegisterDTO DTO)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorMessage = string.Join("; ", errorMessages);

                return StatusCode(400, new ApiResponse<string>
                {
                    Success = false,
                    StatusCode = Models.Enums.StatusCode.BadRequest,
                    message = errorMessage
                });
            }
            var result = await _companyService.CreateCompany(DTO);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("{Id:int}")]
        public async Task<IActionResult>EditCompany(int Id,[FromForm]CompanyEditDTO DTO)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorMessage = string.Join("; ", errorMessages); 

                return StatusCode(400, new ApiResponse<string>
                {
                    Success = false,
                    StatusCode = Models.Enums.StatusCode.BadRequest,
                    message = errorMessage 
                });
            }
            var result = await _companyService.EditCompany(Id, DTO);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult>DeleteCompany(int id)
        {
            var result=await _companyService.DeleteCompany(id);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
