using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace finalHour
{
    // ── Model ─────────────────────────────────────────────────────────────────

    public class Product
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public string PriceDisplay => Price.ToString("C");

        public string Summary =>
            "Product ID:   " + ProductId + "\n" +
            "Product Name: " + ProductName + "\n" +
            "Description:  " + Description + "\n" +
            "Price:        " + PriceDisplay;
    }

    // ── MainWindow ────────────────────────────────────────────────────────────

    public partial class MainWindow : Window
    {
        private readonly List<Product> _products = new List<Product>();
        private readonly List<Product> _cart = new List<Product>();
        private string _searchTerm = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            RefreshGrids();
        }

        // ── Clear ─────────────────────────────────────────────────────────────

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearInputs();
        }

        // ── Add Product ───────────────────────────────────────────────────────

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Product product;
            if (!TryReadInputs(out product)) return;

            var result = MessageBox.Show(
                "Are you sure you want to add this product?\n\n" + product.Summary,
                "Confirm Add Product",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _products.Add(product);
                RefreshGrids();
                ClearInputs();
            }
        }

        // ── Remove Product ────────────────────────────────────────────────────

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            Product selected = ProductsGrid.SelectedItem as Product;
            if (selected == null)
            {
                MessageBox.Show("Please select a product to remove.",
                    "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                "Are you sure you want to remove this product?\n\n" + selected.Summary,
                "Confirm Remove Product",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _products.Remove(selected);
                RefreshGrids();
            }
        }

        // ── Add to Cart ───────────────────────────────────────────────────────

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            Product selected = ProductsGrid.SelectedItem as Product;
            if (selected == null)
            {
                MessageBox.Show("Please select a product to add to cart.",
                    "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                "Are you sure you want to add this product to the cart?\n\n" + selected.Summary,
                "Confirm Add to Cart",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Copy so the original stays in Available Products
                _cart.Add(new Product
                {
                    ProductId = selected.ProductId,
                    ProductName = selected.ProductName,
                    Description = selected.Description,
                    Price = selected.Price
                });
                RefreshGrids();
            }
        }

        // ── Remove From Cart ──────────────────────────────────────────────────

        private void RemoveFromCart_Click(object sender, RoutedEventArgs e)
        {
            Product selected = CartGrid.SelectedItem as Product;
            if (selected == null)
            {
                MessageBox.Show("Please select a product to remove from cart.",
                    "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                "Are you sure you want to remove this product from the cart?\n\n" + selected.Summary,
                "Confirm Remove From Cart",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _cart.Remove(selected);
                RefreshGrids();
            }
        }

        // ── Search handlers ───────────────────────────────────────────────────

        private void TxtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            _searchTerm = TxtSearch.Text?.Trim() ?? string.Empty;
            RefreshGrids();
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            TxtSearch.Clear();
            _searchTerm = string.Empty;
            RefreshGrids();
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private bool TryReadInputs(out Product product)
        {
            product = null;

            if (string.IsNullOrWhiteSpace(TxtProductId.Text) ||
                string.IsNullOrWhiteSpace(TxtProductName.Text) ||
                string.IsNullOrWhiteSpace(TxtDescription.Text) ||
                string.IsNullOrWhiteSpace(TxtPrice.Text))
            {
                MessageBox.Show("Please fill in all fields.",
                    "Missing Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            decimal price;
            if (!decimal.TryParse(TxtPrice.Text, out price))
            {
                MessageBox.Show("Price must be a valid number.",
                    "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            product = new Product
            {
                ProductId = TxtProductId.Text.Trim(),
                ProductName = TxtProductName.Text.Trim(),
                Description = TxtDescription.Text.Trim(),
                Price = price
            };
            return true;
        }

        private void ClearInputs()
        {
            TxtProductId.Clear();
            TxtProductName.Clear();
            TxtDescription.Clear();
            TxtPrice.Clear();
        }

        private void RefreshGrids()
        {
            IEnumerable<Product> list = _products;

            // Filter
            if (!string.IsNullOrWhiteSpace(_searchTerm))
            {
                var term = _searchTerm.ToLowerInvariant();
                list = list.Where(p =>
                    (!string.IsNullOrEmpty(p.ProductId) && p.ProductId.ToLowerInvariant().Contains(term)) ||
                    (!string.IsNullOrEmpty(p.ProductName) && p.ProductName.ToLowerInvariant().Contains(term)) ||
                    (!string.IsNullOrEmpty(p.Description) && p.Description.ToLowerInvariant().Contains(term))
                );
            }

            // Sort
            switch (_sortMode)
            {
                case "price_asc": list = list.OrderBy(p => p.Price); break;
                case "price_desc": list = list.OrderByDescending(p => p.Price); break;
                case "name_asc": list = list.OrderBy(p => p.ProductName); break;
                case "name_desc": list = list.OrderByDescending(p => p.ProductName); break;
            }

            ProductsGrid.ItemsSource = null;
            ProductsGrid.ItemsSource = list.ToList();

            CartGrid.ItemsSource = null;
            CartGrid.ItemsSource = _cart;
        }
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string term = TxtSearch.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(term))
            {
                MessageBox.Show("Please enter a search term.", "Search",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var found = _products.FirstOrDefault(p =>
                (!string.IsNullOrEmpty(p.ProductId) && p.ProductId.ToLowerInvariant().Contains(term.ToLowerInvariant())) ||
                (!string.IsNullOrEmpty(p.ProductName) && p.ProductName.ToLowerInvariant().Contains(term.ToLowerInvariant())) ||
                (!string.IsNullOrEmpty(p.Description) && p.Description.ToLowerInvariant().Contains(term.ToLowerInvariant()))
            );

            if (found != null)
            {
                MessageBox.Show(found.Summary, "Product Found",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("No product found matching \"" + term + "\".", "Not Found",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private string _sortMode = string.Empty;

        private void SortPriceAsc_Click(object sender, RoutedEventArgs e)
        {
            _sortMode = "price_asc";
            RefreshGrids();
        }

        private void SortPriceDesc_Click(object sender, RoutedEventArgs e)
        {
            _sortMode = "price_desc";
            RefreshGrids();
        }

        private void SortNameAsc_Click(object sender, RoutedEventArgs e)
        {
            _sortMode = _sortMode == "name_asc" ? "name_desc" : "name_asc";
            SortNameBtn.Content = _sortMode == "name_asc" ? "Name A→Z" : "Name Z→A";
            RefreshGrids();
        }
        private void ClearSort_Click(object sender, RoutedEventArgs e)
        {
            _sortMode = string.Empty;
            SortNameBtn.Content = "Name A→Z";
            RefreshGrids();
        }


    }
}