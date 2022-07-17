using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaNegociacao;

internal class Importacao
{

    public Importacao(List<List<string>> conteudos)
    {
        System.Globalization.CultureInfo myInfo = System.Globalization.CultureInfo.CurrentUICulture;

        NotaNegociacao.Dados.DB db = new();
        foreach (var conteudo in conteudos)
        {
            if (conteudo.Count < 13)
                break;
            DateTime dataPregao = DataPregao(conteudo);
            Console.WriteLine($"{dataPregao}");
            int ponto = 0;
            if (conteudo[9] == "TRADE")
                ponto = -1;
            else 
                ponto = conteudo.IndexOf("Operacional");
            if (ponto + 13 >= conteudo.Count())
                break;

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
                novo.Preco = decimal.Parse(linha[6], System.Globalization.CultureInfo.CurrentCulture);
                novo.Valor = decimal.Parse(linha[10], System.Globalization.CultureInfo.CurrentCulture);
                novo.DebitoCredito = linha[11];
                novo.TaxaOperacional = decimal.Parse(linha[12], System.Globalization.CultureInfo.CurrentCulture);
                db.Add(novo);
                //Console.WriteLine(linha[4]);
                ponto += 13;
            }
        }
        db.SaveChanges();
    }

    DateTime DataPregao(List<string> conteudo)
    {
        var ponto = conteudo.IndexOf("pregão");
        if (DateTime.TryParseExact(conteudo[ponto + 1], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dtOperacao))
            return dtOperacao;

        return new DateTime();
    }
}
