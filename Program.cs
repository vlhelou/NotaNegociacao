using System;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.ReadingOrderDetector;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;


var conteudos = ImportaConteudos(@"C:\Users\vlhel\OneDrive\Bolsa\2022\XPINC_NOTA_NEGOCIACAO_B3_12_2022.pdf");

NotaNegociacao.Importacao imp = new NotaNegociacao.Importacao(conteudos);




static List<List<string>> ImportaConteudos(string arquivo)
{
    List<List<string>> retorno = new();

    using var document = PdfDocument.Open(arquivo);
    foreach (var page in document.GetPages())
    {
        var letters = page.Letters; // no preprocessing
        var wordExtractor = NearestNeighbourWordExtractor.Instance;
        var wordExtractorOptions = new NearestNeighbourWordExtractor.NearestNeighbourWordExtractorOptions()
        {
            Filter = (pivot, candidate) =>
            {
                // check if white space (default implementation of 'Filter')
                if (string.IsNullOrWhiteSpace(candidate.Value))
                {
                    // pivot and candidate letters cannot belong to the same word 
                    // if candidate letter is null or white space.
                    // ('FilterPivot' already checks if the pivot is null or white space by default)
                    return false;
                }

                // check for height difference
                var maxHeight = Math.Max(pivot.PointSize, candidate.PointSize);
                var minHeight = Math.Min(pivot.PointSize, candidate.PointSize);
                if (minHeight != 0 && maxHeight / minHeight > 2.0)
                {
                    // pivot and candidate letters cannot belong to the same word 
                    // if one letter is more than twice the size of the other.
                    return false;
                }

                // check for colour difference
                var pivotRgb = pivot.Color.ToRGBValues();
                var candidateRgb = candidate.Color.ToRGBValues();
                if (!pivotRgb.Equals(candidateRgb))
                {
                    // pivot and candidate letters cannot belong to the same word 
                    // if they don't have the same colour.
                    return false;
                }

                return true;
            }
        };

        var words = wordExtractor.GetWords(letters);
        List<string> palavaras = new();
        foreach (var word in words)
        {
            palavaras.Add(word.ToString());
        }
        retorno.Add(palavaras);
        // 1. Extract words
    };


    return retorno;

}