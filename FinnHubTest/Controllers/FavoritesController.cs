using FinnHubTest.Models;
using FinnHubTest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnHubTest.Data;

[Authorize]
public class FavoritesController : Controller
{
    private readonly FinnhubService _finnhubService;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;

    public FavoritesController(FinnhubService finnhubService, UserManager<IdentityUser> userManager, ApplicationDbContext context)
    {
        _finnhubService = finnhubService;
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        List<Stock> favoriteStocks = new List<Stock>();
        var user = await _userManager.GetUserAsync(User);
        
        var favoriteSymbols = await _context.Favorites
            .Where(f => f.UserId == user.Id)
            .Select(f => f.Symbol)
            .ToListAsync();

        foreach (var symbol in favoriteSymbols)
        {
            var stock = await _finnhubService.GetStockInformation(symbol);
            favoriteStocks.Add(stock);
        }

        return View(favoriteStocks);
    }

    [HttpPost]
    public async Task<IActionResult> Add(string symbol)
    {
        var user = await _userManager.GetUserAsync(User);
        var favorite = new Favorite
        {
            UserId = user.Id,
            Symbol = symbol
        };

        _context.Favorites.Add(favorite);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Remove(string symbol)
    {
        var user = await _userManager.GetUserAsync(User);
        var favorite = await _context.Favorites
            .Where(f => f.UserId == user.Id && f.Symbol == symbol)
            .FirstOrDefaultAsync();

        if (favorite != null)
        {
            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
