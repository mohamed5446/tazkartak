using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Tazkartk.Application.DTO;
using Tazkartk.Application.DTO.CompanyDTOs;
using Tazkartk.Application.DTO.Email;
using Tazkartk.Application.DTO.Payments;
using Tazkartk.Application.DTO.Response;
using Tazkartk.Application.Extensions;
using Tazkartk.Application.Interfaces;
using Tazkartk.Application.Interfaces.External;
using Tazkartk.Application.Repository;
using Tazkartk.Domain.Models;
using Tazkartk.Domain.Models.Enums;


namespace Tazkartk.Application.Services
{
    public class CompanyService : ICompanyService
    {
       private  readonly IPhotoService _photoService;
        private readonly UserManager<Account> _AccountManager;
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _conf;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailBodyService _emailBodyService;
        private readonly IEmailService _emailService;
        public CompanyService(IPhotoService photoService, UserManager<Account> accountManager, IConfiguration conf, IMapper mapper, IUnitOfWork unitOfWork, IPaymentService paymentService, IEmailBodyService emailBodyService, IEmailService emailService)
        {
            _photoService = photoService;
            _AccountManager = accountManager;
            _conf = conf;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
            _emailBodyService = emailBodyService;
            _emailService = emailService;
        }

        public async Task<List<payoutDTO>>GetCompanyPayouts(int CompanyId)
        {
            return await _unitOfWork.Payouts.ProjectToList<payoutDTO>(p=>p.CompanyId==CompanyId);
        }
        public async Task<ApiResponse<string>> WithdrawlBalance(int CompanyId,WithdrawDTO DTO)
        {
            var company = await _unitOfWork.Companies.GetById(CompanyId);
            if (company == null)
            {
               return ApiResponse<string>.Error("الشركة غير موجودة");
            }       
            var balance = company.Balance;
            if (balance <= 0) return ApiResponse<string>.Error("الرصيد غير كافي للسحب");
            var res = await _paymentService.DispurseAsync(DTO.issuer,DTO.WalletPhoneNumber,balance);
            if (!res.Success)
                return ApiResponse<string>.Error(res.message);

            company.Balance = 0;
            var payout = new Payout
            {
                Amount = balance,
                CompanyId = CompanyId,
                Date = DateTime.UtcNow,
                PaymentMethod = PaymentMethods.Wallet,
                Wallet_Issuer = res.Issuer,
                WalletNumber = res.mssidn,
                Status = PaymentStatus.Succeeded.GetDisplayName(),
                PayoutId = res.TransactionId
            };
            await _unitOfWork.Payouts.Add(payout);
            await _unitOfWork.CompleteAsync();
            var Email = new EmailRequest
            {
                Email = company.Email,
                Subject = "..",
                Body = _emailBodyService.PayoutConfirmationEmailBody(company.Name, balance, DateTime.Now.ToUniversalTime().ToEgyptDateString(),DTO.WalletPhoneNumber)
            };
           await _emailService.SendEmail(Email);
            return ApiResponse<string>.success("تم تحويل الرصيد بنجاح");
           
        }
        public async Task<ApiResponse<CompanyDTO>> CreateCompanyAsync(CompanyRegisterDTO DTO)
        {

            if (await _AccountManager.FindByEmailAsync(DTO.Email) != null)
            {
                return ApiResponse<CompanyDTO>.Error("البريد الإلكتروني مستخدم من قبل");
            }
            var Company = _mapper.Map<Company>(DTO);
            Company.Logo = _conf["Logo"];
            Company.UserName = Company.Email;
            Company.EmailConfirmed = true;
            var result = await _AccountManager.CreateAsync(Company, DTO.Password);
            if (!result.Succeeded)
            {
                return ApiResponse<CompanyDTO>.Error(result.Errors.FirstOrDefault()?.Description ?? "حدث خطا ");
            }
            await _AccountManager.AddToRoleAsync(Company, Roles.Company.ToString());
            var Data = _mapper.Map<CompanyDTO>(Company);

            return ApiResponse<CompanyDTO>.success("تمت اضافة الشركة بنجاح ", Data, StatusCode.Created);
        }
        public async Task<IReadOnlyList<CompanyDTO>> GetAllCompaniesAsync()
        {
            return await _unitOfWork.Companies.ProjectToList<CompanyDTO>();
        }

        public async Task<CompanyDTO?> GetCompanyByIdAsync(int id)
        {
            return await _unitOfWork.Companies.GetById<CompanyDTO>(c=>c.Id==id);
        }
        public async Task<ApiResponse<CompanyDTO>> EditCompanyAsync(int Id, CompanyEditDTO DTO)
        {
            var Company = await _unitOfWork.Companies.GetById(Id);
            if (Company == null)
            {
                return ApiResponse<CompanyDTO>.Error("الشركة غير موجودة ");
            }
            Company.PhoneNumber = DTO.PhoneNumber?.Trim() ?? Company.PhoneNumber;
            Company.Name = DTO.Name?.Trim() ?? Company.Name;
            Company.City = DTO.City?.Trim() ?? Company.City;
            Company.Street = DTO.Street?.Trim() ?? Company.Street;
            if (DTO.Logo != null)
            {
                if (!string.IsNullOrEmpty(Company.Logo))
                {
                  await _photoService.DeletePhotoAsync(Company.Logo);
                }
                var photoResult = await _photoService.AddPhotoAsync(DTO.Logo);
             
                Company.Logo = photoResult;
            }
            _unitOfWork.Companies.Update(Company);
            await _unitOfWork.CompleteAsync();
            var Data = _mapper.Map<CompanyDTO>(Company);
            return ApiResponse<CompanyDTO>.success("تم التعديل بنجاح", Data);
        }

        public async Task<ApiResponse<CompanyDTO>> DeleteCompanyAsync(int CompanyId)
        {
            var company=await _unitOfWork.Companies.GetById(c=>c.Id==CompanyId,c=>c.Trips);
         
            if (company == null)
            {
                return ApiResponse<CompanyDTO>.Error("الشركة غير موجودة ");
            }
            if (!string.IsNullOrEmpty(company.Logo))
            {
            await _photoService.DeletePhotoAsync(company.Logo); 
            }
            var has_trips = company.Trips.Any();
            if (has_trips)
            {
                return ApiResponse<CompanyDTO>.Error(" يجب أولاً حذف الرحلات الخاصة بهذه الشركة قبل أن تتمكن من حذفها ");
            }
            _unitOfWork.Companies.Remove(company);
            await _unitOfWork.CompleteAsync();
            return ApiResponse<CompanyDTO>.success("تم حذف الشركة بنجاح ");
        }

        #region Extra
        public async Task<ApiResponse<string>> IncreaseCompanyBalance(int CompanyId)
        {
            var company = await _unitOfWork.Companies.GetById(CompanyId);
            if (company == null)
            {
                return ApiResponse<string>.Error("error");
            }
            company.Balance += 1;
            await _unitOfWork.CompleteAsync();
            return ApiResponse<string>.success("balance increased by 1");
        }
        #endregion


    }
}
