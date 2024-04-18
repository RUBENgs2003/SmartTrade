﻿using SmartTrade.Entities;
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
        private ProductPage productPage;
        //private Registro registro;
        //private LoginPage loginPage;

        public LoginPage(STService service)
        {
            this.service = service;
            InitializeComponent();
            this.BindingContext = new LoginViewModel();
        }

        private async void Ingresar_Clicked (object sender, EventArgs e)
        {
            try
            {
                string username = correo.Text;
                string password = contraseña.Text;
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    await DisplayAlert("Error", "Por favor, ingrese su correo y contraseña", "OK");
                }
                else
                {
                    if (await service.Login(username, password))
                    {
                        correo.Text = string.Empty;
                        contraseña.Text = string.Empty;
                        await DisplayAlert("Éxito", "Inicio de sesión exitoso", "OK");
                        //await Navigation.PopAsync();
                        Catalogo paginaPrincipal = new Catalogo(service);
                        await Navigation.PushAsync(paginaPrincipal);
                    }
                    else
                    {
                        await DisplayAlert("Error", "Correo o contraseña incorrectos", "OK");
                    }
                }
            }  
            catch (Exception)
            {
                correo.Text = string.Empty;
                contraseña.Text = string.Empty;
            }
        }

        private void VerContraseña_Changed(object sender, TextChangedEventArgs e)
        {
            if (VerContraseña.IsChecked == true) contraseña.IsPassword = false;
            else contraseña.IsPassword = true; ;
        }

        private void Correo_Focused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(correo.Text))
            {
                correo.TextColor = Color.Gray;
            }
        }

        private void Correo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(correo.Text))
            { 
                correo.TextColor = Color.Black;
            }
        }

        private void Contraseña_Focused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(contraseña.Text))
            {
                contraseña.TextColor = Color.Gray;
            }
        }

        private void Contraseña_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(contraseña.Text)) 
            { 
                contraseña.TextColor = Color.Black; 
            }
        }
        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            //await Navigation.
            SeleccionRegistro registro = new SeleccionRegistro(service);
            await Navigation.PushAsync(registro);
        }
    }
}