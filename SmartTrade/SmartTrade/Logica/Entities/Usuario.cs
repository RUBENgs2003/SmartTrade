﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTrade.Entities
{
    public partial class Usuario
    {
        public Usuario() { }
        public Usuario(string nickname, string nombre, string contraseña, string direccion, string email, DateTime fecha_nac)
        {
            this.Nickname = nickname;
            this.Nombre = nombre;
            this.Contraseña = contraseña;
            this.Direccion = direccion;
            this.Email = email;
            this.Fecha_nac = fecha_nac;
        }
    }
}
