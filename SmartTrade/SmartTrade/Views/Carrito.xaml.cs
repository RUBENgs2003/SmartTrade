﻿using Acr.UserDialogs;
using SmartTrade.Entities;
using SmartTrade.Logica.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartTrade.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Carrito : ContentPage
    {

        public STService service;
        CarritoBuilder carrito;

        public Carrito()
        {
            InitializeComponent();
            this.service = STService.Instance;

        }
        override protected void OnAppearing()
        {
            base.OnAppearing();
            UserDialogs.Instance.ShowLoading("Cargando productos...");
            CargarProductosCarrito();
            UserDialogs.Instance.HideLoading();
        }

        private async void CargarProductosCarrito()
        {
            try
            {
                carrito = new CarritoBuilder();
                Director director = new Director();
                await director.ConstruirCarrito(carrito);
                StackLayout stackLayout = this.FindByName<StackLayout>("listaItems");
                StackLayout stack_resumen = this.FindByName<StackLayout>("stack_Resumen");

                stackLayout.Children.Clear();

                if (carrito.GetProductos().Count == 0)
                {

                    stack_resumen.IsVisible = false;

                    StackLayout stackLayoutProductoVacio = new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        Children =
                        {
                            new Label
                            {
                                Text = "No hay productos en el carrito",
                                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                                TextColor = Color.Black,
                                FontAttributes = FontAttributes.Bold,
                                HorizontalOptions = LayoutOptions.Center,
                                VerticalOptions = LayoutOptions.Center
                            }
                        }
                    };

                    stackLayout.Children.Add(stackLayoutProductoVacio);

                    return;
                }
                else
                {
                    stack_resumen.IsVisible = true;
                    ActualizarResumen();    
                    MostrarProductosCarrito(carrito.GetProductos());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error al cargar los productos del carrito: {e.Message}");
            }
        }

        private void MostrarProductosCarrito(List<ItemCarrito> productosCarrito)
        {

            StackLayout stackLayout = this.FindByName<StackLayout>("listaItems");

            foreach (ItemCarrito item in productosCarrito)
            {
                crearTarjeta(item, stackLayout);
            }

        }

        private async void crearTarjeta(ItemCarrito item, StackLayout stackLayout)
        {
            try
            {
                Producto producto = await service.GetProductoByIdProductoVendedor(item.idProductoVendedor);
                Producto_vendedor productoVendedor = producto.Producto_Vendedor.FirstOrDefault(pv => pv.Id == item.idProductoVendedor);
                span_costeTotal.Text = carrito.GetCosteTotal().ToString() + "€";
                span_puntosObtenidos.Text = carrito.GetPuntosObtenidos().ToString();

                string caracteristicas = "";
                if (item.Caracteristica != null)
                {
                    caracteristicas = "Características: " + item.Caracteristica;
                }

                var roundedButtonPlus = new Button
                {
                    Text = "+",
                    TextColor = Color.Black,
                    WidthRequest = 30,
                    HeightRequest = 30,
                    BackgroundColor = Color.FromHex("#f0f0f0"),
                    CornerRadius = 5,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };

                Label cantidadLabel = new Label { Text = item.Cantidad.ToString(), FontAttributes = FontAttributes.Bold, TextColor = Color.Black, VerticalOptions = LayoutOptions.Center };

                roundedButtonPlus.Clicked += async (sender, args) =>
                {
                    
                    await carrito.ActualizarCantidadItem(item, 1);
                    cantidadLabel.Text = item.Cantidad.ToString();
                    ActualizarResumen();

                    UserDialogs.Instance.Toast("Cantidad del producto actualizada", TimeSpan.FromSeconds(3));
                };

                var roundedButtonMinus = new Button
                {
                    Text = "-",
                    TextColor = Color.Black,
                    WidthRequest = 30,
                    HeightRequest = 30,
                    BackgroundColor = Color.FromHex("#f0f0f0"),
                    CornerRadius = 5,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };

                roundedButtonMinus.Clicked += async (sender, args) =>
                {
                    // Restar la cantidad al hacer clic en el botón de restar
                    if (item.Cantidad > 1)
                    {
                        
                        await carrito.ActualizarCantidadItem(item, -1);
                        cantidadLabel.Text = item.Cantidad.ToString();
                        ActualizarResumen();

                        UserDialogs.Instance.Toast("Cantidad del producto actualizada", TimeSpan.FromSeconds(3));
                    }
                    else if (item.Cantidad == 1)
                    {
                        UserDialogs.Instance.Confirm(new ConfirmConfig
                        {
                            Message = "¿Desea eliminar el producto del carrito?",
                            OkText = "Sí",
                            CancelText = "No",
                            OnAction = async (result) =>
                            {
                                if (result)
                                {
                                    await service.EliminarItemCarrito(item);
                                    _ = carrito.RemoveItem(item);
                                    CargarProductosCarrito();
                                }
                            }
                        });
                    }
                };

                var quantityButtonsLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 10,
                    Children =
                    {
                        new Label { Text = "Cantidad:", TextColor = Color.Black, VerticalOptions = LayoutOptions.Center },
                        roundedButtonMinus,
                        cantidadLabel,
                        roundedButtonPlus
                    }
                };

                var productCard = new Frame
                {
                    BackgroundColor = Color.White,
                    CornerRadius = 10,
                    Margin = new Thickness(10),
                    Padding = new Thickness(10),
                    HasShadow = true,
                    Content = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Spacing = 10,
                        Children =
                        {
                            new Image
                            {
                                Source = producto.Imagen,
                                HeightRequest = 100,
                                WidthRequest = 100,
                                Aspect = Aspect.AspectFit,
                                GestureRecognizers =
                                {
                                    new TapGestureRecognizer
                                    {
                                        Command = new Command(async () =>
                                        {
                                            await Navigation.PushAsync(new ProductPage( producto));
                                        })
                                    }
                                }
                            },
                            new StackLayout
                            {
                                VerticalOptions = LayoutOptions.Center,
                                Spacing = 5,
                                Children =
                                {
                                    new Label { Text = producto.Nombre, FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), TextColor = Color.Black, FontAttributes = FontAttributes.Bold },
                                    new Label { Text = producto.Descripcion.Length > 50 ? producto.Descripcion.Substring(0, 50) + "..." : producto.Descripcion, TextColor = Color.Gray },
                                    new Label { Text = caracteristicas, TextColor = Color.Gray } ,
                                    quantityButtonsLayout,
                                    new Label { Text = productoVendedor.Precio.ToString() + "€", FontAttributes = FontAttributes.Bold, FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), TextColor = Color.Black },
                                }
                            }
                        }
                    }
                };

                stackLayout.Children.Add(productCard);

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error al obtener el producto: {e.Message}");
            }
        }

        private void ActualizarResumen()
        {
            span_costeTotal.Text = carrito.GetCosteTotal().ToString() + "€";
            span_puntosObtenidos.Text = carrito.GetPuntosObtenidos().ToString();
        }

        private void BtnAtras_click(object sender, EventArgs e)
        {
            Console.WriteLine("Atras");
            Navigation.PopAsync();
        }

        private void BtnPerfil_click(object sender, EventArgs e)
        {
            //TODO 
            Console.WriteLine("Perfil");
        }

        private void BtnFinalizarCompra_click(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PedidoPage(carrito.GetProductos()));
            //TODO
            Console.WriteLine("Finalizar Compra");
        }

    }
}