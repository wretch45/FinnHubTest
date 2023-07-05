using FinnHubTest.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinnHubTest.Data;

namespace FinnHubTest.Services
{
    public class FavoritesService
    {
        private readonly ApplicationDbContext _context;

        public FavoritesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetFavoriteSymbolsAsync(string userId)
        {
            return await _context.Favorites
                .Where(f => f.UserId == userId)
                .Select(f => f.Symbol)
                .ToListAsync();
        }

        public async Task AddFavoriteAsync(string symbol, string userId)
        {
            if (!await _context.Favorites.AnyAsync(f => f.Symbol == symbol && f.UserId == userId))
            {
                _context.Favorites.Add(new Favorite { Symbol = symbol, UserId = userId });
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveFavoriteAsync(string symbol, string userId)
        {
            var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.Symbol == symbol && f.UserId == userId);
            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
            }
        }

    }
}