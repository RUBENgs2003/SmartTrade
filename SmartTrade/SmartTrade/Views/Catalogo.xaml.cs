﻿using System;
using System.Collections.Generic;
using System.Linq;
using SmartTrade.Entities;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using SmartTrade.Logica.Services;
using Acr.UserDialogs;
using System.Threading.Tasks;

namespace SmartTrade.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Catalogo : ContentPage
    {
        private STService service;
        private List<Producto> catalogoProductos = new List<Producto>();

        public Catalogo()
        {
            InitializeComponent();
            this.service = STService.Instance;

            ConfigurarNavegacion();
            ConfigurarPickerFiltrado();

            SearchBar searchBar = (SearchBar)FindByName("searchBar");
            searchBar.TextChanged += OnBusqueda;

            //TEST - REDUCE EL STOCK A 0 PARA PROBAR LAS ALERTAS
            //test();

        }

        private async Task test()
        {
            List<Producto> productos = await service.getProductosListaDeseos();
            foreach(Producto p in productos)
            {
                p.ReducirStock(p.Producto_Vendedor.First(), p.Producto_Vendedor.First().Stock);
            }
        }

        private void ConfigurarNavegacion()
        {
            StackLayout stackLayout = (StackLayout)FindByName("userLoggedNavigation");
            StackLayout stackLayoutNoLogged = (StackLayout)FindByName("userNotLoggedNavigation");

            stackLayout.IsVisible = service.GetUsuarioLogueado() != null;
            stackLayoutNoLogged.IsVisible = service.GetUsuarioLogueado() == null;

        }

        private async void CargarProductos()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Cargando productos...");
                if (catalogoProductos.Count == 0) this.catalogoProductos = await service.GetAllProductos();
                UserDialogs.Instance.HideLoading();
                if (catalogoProductos.Count == 0)
                {
                    Debug.WriteLine("No se han encontrado productos");
                }
                else
                {
                    MostrarProductos(catalogoProductos);

                }
            }
            catch (ServiceException e)
            {
                UserDialogs.Instance.HideLoading();
                UserDialogs.Instance.Alert(e.Message, "Error", "Aceptar");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error al cargar los productos: {e.Message}");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CargarProductos();
        }

        private void ConfigurarPickerFiltrado()
        {
            try
            {
                Picker picker = (Picker)FindByName("picker_categorias");
                picker.SelectedIndexChanged += (s, e) =>
                {

                    //mostrar todos los productos
                    if (picker.SelectedIndex == 0) { MostrarProductos(catalogoProductos); return; }

                    string categoria = picker.Items[picker.SelectedIndex];
                    List<Producto> productosFiltrados = catalogoProductos.Where(p => p.Categoria == categoria).ToList();
                    MostrarProductos(productosFiltrados);
                };
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

        }

        private void OnBusqueda(object sender, EventArgs e)
        {

            SearchBar searchBar = (SearchBar)FindByName("searchBar");
            string busqueda = searchBar.Text.ToLower();


            Grid grid_productos = (Grid)FindByName("grid_productos");
            grid_productos.Children.Clear();

            List<Producto> productosFiltrados = catalogoProductos.Where(p => p.Nombre.ToLower().Contains(busqueda)).ToList();

            if (busqueda == "")
            {
                productosFiltrados = catalogoProductos;
            }

            MostrarProductos(productosFiltrados);
        }

        private void MostrarProductos(List<Producto> productos)
        {

            try
            {
                int columnasPorFila = 2;
                int filaActual = 0;
                int columnaActual = 0;

                Grid grid_productosDestacados = (Grid)FindByName("grid_productosDestacados");

                grid_productos.Children.Clear();
                grid_productos.RowDefinitions.Clear();


                if (productos.Count == 0)
                {
                    grid_productosDestacados.IsVisible = false;
                    grid_productos.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    Label label = new Label { Text = "No se han encontrado productos", FontSize = 20, TextColor = Color.Black };
                    grid_productos.Children.Add(label);
                    return;
                }

                for (int i = 0; i < Math.Ceiling((double)productos.Count / columnasPorFila); i++)
                {
                    grid_productos.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                }


                foreach (Producto producto in productos)
                {
                    Producto_vendedor producto_vendedor = producto.Producto_Vendedor.OrderBy(pv => pv.Precio).First();

                    Frame frame = new Frame
                    {
                        HeightRequest = 180,
                        Margin = new Thickness(0, 0, 5, 0),
                        CornerRadius = 10,
                        BackgroundColor = Color.White,
                        Padding = 10,
                        HasShadow = true
                    };

                    grid_productos.Children.Add(frame, columnaActual, filaActual);
                    columnaActual++;
                    if (columnaActual >= columnasPorFila)
                    {
                        columnaActual = 0;
                        filaActual++;
                        // Añade una nueva definición de fila si es necesario
                        grid_productos.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    }

                    TapGestureRecognizer tap = new TapGestureRecognizer();
                    tap.Tapped += (s, ev) =>
                    {
                        ProductPage productPage = new ProductPage(producto);
                        Navigation.PushAsync(productPage);
                    };

                    frame.GestureRecognizers.Add(tap);

                    StackLayout stackLayout = new StackLayout
                    {
                        Children =
                        {
                            new Image
                            {
                                Source = new UriImageSource { Uri = new Uri(producto.Imagen) },
                                Aspect = Aspect.AspectFill,
                                HeightRequest = 120
                            },
                            new Label
                            {
                                Text = producto.Nombre,
                                FontAttributes = FontAttributes.Bold,
                                TextColor = Color.Black
                            },
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Children =
                                {
                                    new Image
                                    {
                                        Source = new UriImageSource { Uri = new Uri("https://i.ibb.co/NZ99Tp4/Huella-Eco.png") },
                                        Aspect = Aspect.AspectFill,
                                        HeightRequest = 15
                                    },
                                    new Label
                                    {
                                        Text = producto.Huella_eco,
                                        FontAttributes = FontAttributes.Bold,
                                        TextColor = Color.Green
                                    },
                                    new Label
                                    {
                                        Text = "Desde " + producto_vendedor.Precio.ToString() + "€",
                                        FontAttributes = FontAttributes.Bold,
                                        TextColor = Color.Black,
                                        HorizontalOptions = LayoutOptions.EndAndExpand
                                    }
                                }
                            }
                        }
                    };

                    frame.Content = stackLayout;

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void BtnCarrito_click(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Carrito());
        }

        public void BtnPerfil_click(object sender, EventArgs e)
        {
            Perfil perfil = new Perfil();
            Navigation.PushAsync(perfil);
        }

        private void BtnAlerta_click(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Alertas());
        }

        private void BtnDeseos_click(object sender, EventArgs e)
        {
            //PARA PROBAR GUARDARMASTARDE
           //  Navigation.PushAsync(new ListaDeseos());
            Navigation.PushAsync(new ListaDeseos());
        }

        private void BtnLogin_click(object sender, EventArgs e)
        {
            Navigation.PushAsync(new LoginPage());
        }

        //bloquear boton de atras
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

    }
}