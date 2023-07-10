using FinnHubTest.Models;
using FinnHubTest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FinnHubTest.Data;

[Authorize]
public class FavoritesController : Controller
{
    private readonly IFinnhubService _finnhubService;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;

    public FavoritesController(IFinnhubService finnhubService, UserManager<IdentityUser> userManager, ApplicationDbContext context)
    {
        _finnhubService = finnhubService;
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        List<Stock> favoriteStocks = new List<Stock>();
        var user = await _userManager.GetUserAsync(User);
        
        if(user == null)
            return BadRequest("User not found");

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

        if(user == null)
            return BadRequest("User not found");

        // Check if favorite already exists for this user
        var existingFavorite = await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == user.Id && f.Symbol == symbol);

        // If it exists, return an error message
        if (existingFavorite != null)
        {
            return Json(new { error = "This stock is already in your favorites." });
        }

        // If it doesn't exist, add it
        var favorite = new Favorite
        {
            UserId = user.Id,
            Symbol = symbol
        };

        _context.Favorites.Add(favorite);
        await _context.SaveChangesAsync();
        return Json(new { success = "Stock added to favorites." });
    }

    [HttpPost]
    public async Task<IActionResult> Remove(string symbol)
    {
        var user = await _userManager.GetUserAsync(User);
        
        if(user == null)
            return BadRequest("User not found");

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

    [HttpGet]
    public async Task<IActionResult> GetFavoriteStocks()
    {
        List<Stock> favoriteStocks = new List<Stock>();
        var user = await _userManager.GetUserAsync(User);
        
        if(user == null)
            return BadRequest("User not found");

        var favoriteSymbols = await _context.Favorites
            .Where(f => f.UserId == user.Id)
            .Select(f => f.Symbol)
            .ToListAsync();

        foreach (var symbol in favoriteSymbols)
        {
            var stock = await _finnhubService.GetStockInformation(symbol);
            favoriteStocks.Add(stock);
        }

        return Json(favoriteStocks);
    }
}
