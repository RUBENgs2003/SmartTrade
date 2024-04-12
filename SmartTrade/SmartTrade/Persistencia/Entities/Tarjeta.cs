﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartTrade.Entities
{
    [Table("Tarjeta")]
    public partial class Tarjeta
    {
        [Key]
        [Column("nº")]
        public int Numero { get; set; }

        [Column("fecha_cad")]
        public DateTime Fecha_cad { get; set; }

        [Column("nº_seguridad")]
        public int Num_seguridad { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [ForeignKey("id_comprador")]
        public string Nick_comprador { get; set; }


    }
}
