using FinnHubTest.Models;
using FinnHubTest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FinnHubTest.Data;
using Microsoft.Extensions.Logging;
using System;

[Authorize]
public class FavoritesController : Controller
{
    private readonly IFinnhubService _finnhubService;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FavoritesController> _logger;

    public FavoritesController(IFinnhubService finnhubService, UserManager<IdentityUser> userManager, ApplicationDbContext context, ILogger<FavoritesController> logger)
    {
        _finnhubService = finnhubService;
        _userManager = userManager;
        _context = context;
        _logger = logger;
    }

    // Action to display the favorite stocks of the logged-in user
    public async Task<IActionResult> Index()
    {
        try
        {
            List<Stock> favoriteStocks = new List<Stock>();
            var user = await _userManager.GetUserAsync(User);

            // If user is null, return BadRequest
            if (user == null)
            {
                _logger.LogError("User not found");
                return BadRequest("User not found");
            }

            // Get the favorite symbols of the user from the database
            var favoriteSymbols = await _context.Favorites
                .Where(f => f.UserId == user.Id)
                .Select(f => f.Symbol)
                .ToListAsync();

            // For each symbol, get the stock information and add it to the list
            foreach (var symbol in favoriteSymbols)
            {
                var stock = await _finnhubService.GetStockInformation(symbol);
                favoriteStocks.Add(stock);
            }

            // Return the list of favorite stocks to the view
            return View(favoriteStocks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting index");
            return BadRequest("An error occurred while processing your request.");
        }
    }

    [HttpPost] // Action to add a stock to favorites
    public async Task<IActionResult> Add(string symbol)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            // If user is null, return BadRequest
            if (user == null)
            {
                _logger.LogError("User not found");
                return BadRequest("User not found");
            }

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding favorite");
            return BadRequest("An error occurred while processing your request.");
        }
    }

    [HttpPost] // Action to remove a stock from favorites
    public async Task<IActionResult> Remove(string symbol)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            // If user is null, return BadRequest
            if (user == null)
            {
                _logger.LogError("User not found");
                return BadRequest("User not found");
            }

            // Find the favorite in the database and remove it
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while removing favorite");
            return BadRequest("An error occurred while processing your request.");
        }
    }

    [HttpGet] // Action to get the favorite stocks of the logged-in user
    public async Task<IActionResult> GetFavoriteStocks()
    {
        try
        {
            List<Stock> favoriteStocks = new List<Stock>();
            var user = await _userManager.GetUserAsync(User);

            // If user is null, return BadRequest
            if (user == null)
            {
                _logger.LogError("User not found");
                return BadRequest("User not found");
            }

            // Get the favorite symbols of the user from the database
            var favoriteSymbols = await _context.Favorites
                .Where(f => f.UserId == user.Id)
                .Select(f => f.Symbol)
                .ToListAsync();

            // For each symbol, get the stock information and add it to the list
            foreach (var symbol in favoriteSymbols)
            {
                var stock = await _finnhubService.GetStockInformation(symbol);
                favoriteStocks.Add(stock);
            }

            // Return the list of favorite stocks as JSON
            return Json(favoriteStocks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting favorite stocks");
            return BadRequest("An error occurred while processing your request.");
        }
    }
}
