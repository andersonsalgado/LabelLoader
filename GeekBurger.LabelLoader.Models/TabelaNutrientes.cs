using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GeekBurger.LabelLoader.Models
{
    [Table("Cad_Ingredientes")]
    public class TabelaNutrientes
    {
        [Key]
        [Column("Id",TypeName = "bigint")]
        public long Id { get; set; }

        [Column("Descricao", TypeName = "varchar(60)")]
        public string Descricao { get; set; }

        public TabelaNutrientes(long _Id, string _Descricao)
        {
            Id = _Id;
            Descricao = _Descricao;
        }
    }
}
