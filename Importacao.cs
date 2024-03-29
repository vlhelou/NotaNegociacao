using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NotaNegociacao;

internal class Importacao
{
    NotaNegociacao.Dados.DB db = new();
    DateTime dataPregao;
    public Importacao(List<List<string>> conteudos)
    {
        System.Globalization.CultureInfo myInfo = System.Globalization.CultureInfo.CurrentUICulture;


        foreach (var pagina in conteudos)
        {
            //for (int i = 0; i < pagina.Count; i++)
            //{
            //    Console.WriteLine($"{i.ToString().PadLeft(4)} {pagina[i]}");
            //}

            dataPregao = DataPregao(pagina);
            Console.WriteLine($"\t{dataPregao.ToString("dd/MM/yyyy")}");
            if (!db.Diario.Where(p => p.DataPregao == dataPregao).Any())
            {
                LeCabecalho(pagina);
                LeConteudo(pagina);
            }
        }
        db.SaveChanges();
    }

    DateTime DataPregao(List<string> conteudo)
    {
        var ponto = conteudo.IndexOf("pregão");
        if (DateTime.TryParseExact(conteudo[ponto + 3], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dtOperacao))
            return dtOperacao;

        return new DateTime();
    }

    decimal Str2Decimal(string valor)
    {
        if (string.IsNullOrEmpty(valor.Trim()))
            return 0;

        var localizado = Regex.Match(valor, @"[0-9,]+");
        if (localizado.Success)
            return decimal.Parse(localizado.Value, System.Globalization.CultureInfo.CurrentCulture);

        return 0;
    }

    void LeCabecalho(List<string> conteudo)
    {
        if (conteudo.IndexOf("CONTINUA...") > -1)
            return;
        Dados.Diario diario = new Dados.Diario();
        diario.DataPregao = dataPregao;
        int ponto = conteudo.IndexOf("negócios");
        diario.Bruto = Str2Decimal(conteudo[ponto + 5]);
        if (conteudo[ponto + 7].Trim() == "D")
            diario.Bruto *= -1;


        string IRRF = conteudo[ponto + 22];
        string TaxaOperacional = conteudo[ponto + 23];
        string RegistroB3 = conteudo[ponto + 24];
        string taxaB3 = conteudo[ponto + 25];
        string Impostos = conteudo[ponto + 42];
        string Liquido = conteudo[ponto + 76];
        string debitoCredito = conteudo[ponto + 78];

        diario.IRRF = Str2Decimal(IRRF);
        diario.TaxaOperacional = Str2Decimal(TaxaOperacional);
        diario.RegistroB3 = Str2Decimal(RegistroB3);
        diario.TaxaB3 = Str2Decimal(taxaB3);
        diario.Impostos = Str2Decimal(Impostos);
        diario.Liquido = Str2Decimal(Liquido);
        if (debitoCredito.Trim() == "D")
            diario.Liquido *= -1;
        db.Add(diario);
    }

    void LeConteudo(List<string> conteudo)
    {
        if (conteudo.Count < 13)
            return;

        Console.WriteLine($"{dataPregao}");
        int ponto = 0;
        if (conteudo[9] == "TRADE")
            ponto = -1;
        else
            ponto = conteudo.IndexOf("Operacional");
        if (ponto + 13 >= conteudo.Count())
            return;

        while (true)
        {
            if (ponto + 13 >= conteudo.Count())
                break;
            var linha = conteudo.GetRange(ponto + 1, 13);
            if (linha[9] != "TRADE")
                break;
            var novo = new NotaNegociacao.Dados.Operacao();
            novo.CompraVenda = linha[0];
            novo.Produto = linha[1];
            if (linha[4].StartsWith("@"))
                linha[4] = linha[4].Substring(1);
            //if (DateTime.TryParseExact(linha[4], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dtOperacao))
            novo.Data = dataPregao;

            novo.Quantidade = int.Parse(linha[5]);
            novo.Preco = Str2Decimal(linha[6]);
            novo.Valor = Str2Decimal(linha[10]);
            novo.DebitoCredito = linha[11];
            novo.TaxaOperacional = Str2Decimal(linha[12]);
            db.Add(novo);
            //Console.WriteLine(linha[4]);
            ponto += 13;
        }

    }
}
