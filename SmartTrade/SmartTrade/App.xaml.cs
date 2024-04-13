﻿using SmartTrade.Persistencia.Services;
using SmartTrade.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartTrade
{
    public partial class App : Application
    {

        public App()
        {

            InitializeComponent();
            STService service = STService.Instance;
            // DependencyService.Register<MockDataStore>();
           MainPage = new NavigationPage(new Catalogo(service));
            
           // MainPage = new ProductPage(service);
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
