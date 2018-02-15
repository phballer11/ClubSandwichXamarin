﻿using System;
using System.Collections.Generic;
using ClubSandwich.Model;
using ClubSandwich.Service;
using Realms;
using Xamarin.Forms;

namespace ClubSandwich
{
    public partial class Login : ContentPage
    {
        Effect fontEffect;
        Realm _realm;
        UserService _userService;

        public Login()
        {
            _realm = RealmConnection.GetInstance();

            InitializeComponent();
            fontEffect = new LabelFontEffect();

            Title.Effects.Add(fontEffect);
        }

        private async void Login_Clicked(object sender, EventArgs e) {
            // TODO: use Token.Text to pass in bearer token to api before navigatin
            var transaction = _realm.BeginWrite();

            _realm.RemoveAll();
            _realm.Add<LoginCredential>(new LoginCredential() { Token = Token.Text });

            _userService = new UserService();
            var response = await _userService.GetMyUserInfo();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _realm.Add<User>(response.Data.Me);

                transaction.Commit();
                transaction.Dispose();

                App.Current.MainPage = new MainPage();
            }
            else
            {
                _realm.RemoveAll();

                transaction.Commit();
                transaction.Dispose();

                await DisplayAlert("Failed", "Authentication Failed", "OK");
            }
        }
    }
}
