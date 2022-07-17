using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NotaNegociacao.Dados;

internal class DB : DbContext
{
    public DbSet<Operacao>? Operacao { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer($"Server=.;Database=NotasOperacao;Trusted_Connection=True");
}
