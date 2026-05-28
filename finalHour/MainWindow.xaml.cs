using System.Collections.Generic;
using System.Windows;

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
            ProductsGrid.ItemsSource = null;
            ProductsGrid.ItemsSource = _products;

            CartGrid.ItemsSource = null;
            CartGrid.ItemsSource = _cart;
        }
    }
}