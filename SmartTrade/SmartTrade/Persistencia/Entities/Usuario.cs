﻿using System;
using System.Collections.Generic;
using System.Text;
//using Postgrest.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTrade.Entities
{
    [Table("Usuario")]
    public partial class Usuario
    {

        [Key]
        [Column("nickname")]
        public string Nickname { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("contraseña")]
        public string Contraseña { get; set; }

        [Column("direccion")]
        public string Direccion { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("fecha_nac")]
        public DateTime Fecha_nac { get; set; }

    }
}
