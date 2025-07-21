using Microsoft.EntityFrameworkCore;
using PetCare.Auth.Domain.Entities;
using PetCare.Auth.Domain.Interfaces;
using PetCare.Auth.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;

namespace PetCare.Auth.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AuthDbContext _context;

        public RefreshTokenRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RefreshToken token)
        {
            await _context.RefreshTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetByUserAndTokenAsync(Guid userId, string token)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.UserId == userId && rt.Token == token);
        }

        public async Task UpdateAsync(RefreshToken token)
        {
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();
        }
    }
}
