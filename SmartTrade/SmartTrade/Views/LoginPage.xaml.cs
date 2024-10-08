﻿using Acr.UserDialogs;
using SmartTrade.Entities;
using SmartTrade.Logica.Services;
using SmartTrade.ViewModels;
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
    public partial class LoginPage : ContentPage
    {
        private STService service;
        //private string username;
        //private string password;
        //private Registro registro;
        //private LoginPage loginPage;

        public LoginPage()
        {
            this.service = STService.Instance;
            InitializeComponent();
            this.BindingContext = new LoginViewModel();
        }

        private async void Ingresar_Clicked (object sender, EventArgs e)
        {
            try
            {
                string username = mail.Text;
                string password = pass.Text;
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    await UserDialogs.Instance.AlertAsync("Debe completar todos los campos", "Error", "OK");
                }
                else
                {
                    UserDialogs.Instance.ShowLoading("Iniciando sesión...");
                    if (await service.Login(username, password))
                    {
                        mail.Text = string.Empty;
                        pass.Text = string.Empty;
                        //await DisplayAlert("Éxito", "Inicio de sesión exitoso", "OK");
                        UserDialogs.Instance.HideLoading();
                        UserDialogs.Instance.Toast("Inicio de sesión exitoso", TimeSpan.FromSeconds(2));
                        if (!service.IsVendedor()) {
                            Catalogo paginaPrincipal = new Catalogo();
                            await Navigation.PushAsync(paginaPrincipal);
                            UserDialogs.Instance.HideLoading();
                        }
                        else
                        {
                            List<Producto> productos = await service.GetProductosDeVendedor(service.GetLoggedNickname());
                            CatalogoVendedor paginaPrincipal = new CatalogoVendedor(service, productos);
                            await Navigation.PushAsync(paginaPrincipal);
                            UserDialogs.Instance.HideLoading();
                        }
                    }
                    else
                    {
                        UserDialogs.Instance.HideLoading();
                        UserDialogs.Instance.Alert("Correo o contraseña incorrectos", "Error", "OK");
                    }
                }
            }  
            catch (ServiceException err)
            {
                UserDialogs.Instance.HideLoading();
                UserDialogs.Instance.Alert(err.Message, "Error", "OK");
            }
            catch (Exception)
            {
                mail.Text = string.Empty;
                pass.Text = string.Empty;
            }
        }

        private void VerContraseña_Changed(object sender, TextChangedEventArgs e)
        {
            if (verPass.IsChecked == true) pass.IsPassword = false;
            else pass.IsPassword = true; ;
        }

        private void Correo_Focused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(mail.Text))
            {
                mail.TextColor = Color.Gray;
            }
        }

        private void Correo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(mail.Text))
            { 
                mail.TextColor = Color.Black;
            }
        }

        private void Contraseña_Focused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(pass.Text))
            {
                pass.TextColor = Color.Gray;
            }
        }

        private void Contraseña_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(pass.Text)) 
            { 
                pass.TextColor = Color.Black; 
            }
        }
        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            //await Navigation.
            SeleccionRegistro registro = new SeleccionRegistro();
            Navigation.PushAsync(registro);
        }
    }
}