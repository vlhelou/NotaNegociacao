using System;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.ReadingOrderDetector;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;



var arquivos = Directory.GetFiles(@"C:\Users\vlhel\OneDrive\Bolsa\2023\", "*.pdf");

foreach (var arquivo in arquivos)
{
    Console.WriteLine(arquivo);

    var conteudos = ImportaConteudos(arquivo);
    NotaNegociacao.Importacao imp = new NotaNegociacao.Importacao(conteudos);
}

//var conteudos = ImportaConteudos(@"C:\Users\vlhel\OneDrive\Bolsa\2023\2023 abril.pdf");

//NotaNegociacao.Importacao imp = new NotaNegociacao.Importacao(conteudos);




static List<List<string>> ImportaConteudos(string arquivo)
{
    List<List<string>> retorno = new();

    using var document = PdfDocument.Open(arquivo);
    foreach (var page in document.GetPages())
    {
        IEnumerable<Word> words = page.GetWords();
        List<string> palavaras = new();
        foreach (var word in words)
        {
            palavaras.Add(word.ToString());
        }
        retorno.Add(palavaras);
    };


    return retorno;

}