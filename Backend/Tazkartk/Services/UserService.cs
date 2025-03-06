using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Tazkartk.Data;
using Tazkartk.DTO.AccontDTOs;
using Tazkartk.DTO.CompanyDTOs;
using Tazkartk.DTO.UserDTOs;
using Tazkartk.Interfaces;
using Tazkartk.Models;
using Tazkartk.Models.Enums;

namespace Tazkartk.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPhotoService _photoService;
        private readonly UserManager<Account> _AccountManager;

        public UserService(ApplicationDbContext context, IPhotoService photoService, UserManager<Account> accountManager)
        {
            _context = context;
            _photoService = photoService;
            _AccountManager = accountManager;
        }
        public async Task<List<UserDetails>> GetUsers()
        {
            return await _context.Users.AsNoTracking()
                .Select(u => new UserDetails
                {
                    Email = u.Email,
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhotoUrl = u.photo,
                    phoneNumber = u.PhoneNumber,
                }).ToListAsync();
        }

        public async Task<User?> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;
            return user;
        }

        public async Task<UserDetails?> GetUserDetailsById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;
            return new UserDetails
            {
                Id = id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                phoneNumber = user.PhoneNumber,
                PhotoUrl = user.photo
            };
        }
        public async Task<UserDetails?>AddUser(RegisterDTO DTO,Roles role)
        {
             if (await _AccountManager.FindByEmailAsync(DTO.Email) != null)
             {
                return null;
             }
            var user = new User
            {
                FirstName = DTO.FirstName,
                LastName = DTO.LastName,
                Email = DTO.Email,
                PhoneNumber = DTO.PhoneNumber,
                UserName = DTO.Email,
                EmailConfirmed=true
            };
            var result = await _AccountManager.CreateAsync(user, DTO.Password);
            if (!result.Succeeded)
            {
                return null;
            }
            await _AccountManager.AddToRoleAsync(user, role.ToString());

            return new UserDetails
            {
                Id=user.Id,
                FirstName= user.FirstName,
                LastName= user.LastName,
                Email= user.Email,
                phoneNumber= user.PhoneNumber,
            };
        }
        public async Task<UserDetails> EditUser(User user,EditUserDTO DTO)
        {
           
            if (!string.IsNullOrEmpty(DTO.firstName))
            {
                user.FirstName = DTO.firstName;
            }

            if (!string.IsNullOrEmpty(DTO.lastName))
            {
                user.LastName = DTO.lastName;
            }

            if (!string.IsNullOrEmpty(DTO.Phone))
            {
                user.PhoneNumber = DTO.Phone;
            }


            if (DTO.photo!=null)
            {
                if(!string.IsNullOrEmpty(user.photo))
                {
                    await _photoService.DeletePhotoAsync(user.photo);
                }

                var photoResult = await _photoService.AddPhotoAsync(DTO.photo);
                user.photo = photoResult.Url.ToString();
            }

           await _context.SaveChangesAsync();
            return new UserDetails
            {
                Id=user.Id,
                Email=user.Email,
                FirstName=user.FirstName,
                LastName=user.LastName,
                phoneNumber=user.PhoneNumber,
                PhotoUrl=user.photo
            };
        }
        public async Task DeleteUser(User user)
        {
            if (!string.IsNullOrEmpty(user.photo))
            {
                await _photoService.DeletePhotoAsync(user.photo);
            }
            // var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
              await _context.SaveChangesAsync();

        }
       
    }
}
