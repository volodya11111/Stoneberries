using Microsoft.EntityFrameworkCore;
using Stoneberries.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoneberries
{
    public class CartService
    {
        private readonly ApplicationContext _context;
        private readonly CurrentUserService _currentUserService;

        public CartService(ApplicationContext context, CurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task AddProductToCartAsync(Product product)
        {
            var currentUser = _currentUserService.LoggedInUser;

            var cart = await _context.Carts
                .Include(c => c.Items) 
                .FirstOrDefaultAsync(c => c.UserId == currentUser.Id);

            if (cart == null)
            {
                cart = new Cart { UserId = currentUser.Id };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var cartItem = cart.Items.FirstOrDefault(item => item.ProductId == product.Id);

            if (cartItem != null)
            {
                cartItem.Quantity++;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = product.Id,
                    Quantity = 1
                });
            }

            await _context.SaveChangesAsync(); 
        }
        public List<CartItem> GetCartItems(int userId)
        {
            var cart = _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefault(c => c.UserId == userId);
            return cart?.Items.ToList() ?? new List<CartItem>();

        }
        public void RemoveFromCart(int cartItemId)
        {
            var cartItem = _context.CartsItem.FirstOrDefault(ci => ci.Id == cartItemId);
            if (cartItem != null)
            {
                _context.CartsItem.Remove(cartItem);
                _context.SaveChanges();
            }
        }
        public void UpdateCartItemQuantity(int cartItemId, int newQuantity)
        {
            var cartItem = _context.CartsItem.FirstOrDefault(ci => ci.Id == cartItemId);
            if (cartItem != null)
            {
                cartItem.Quantity = newQuantity;
                _context.SaveChanges();
            }
        }
        public void ClearCart(int userId)
        {
            var cartItems = _context.CartsItem.Where(item => item.Cart.UserId == userId);

            if (cartItems.Any())
            {
                _context.CartsItem.RemoveRange(cartItems);
                _context.SaveChanges();
            }
        }
    }
}
