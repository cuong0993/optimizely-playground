using System.Linq;
using System.Text.RegularExpressions;

namespace AlloyDemo.Business.ExtensionMethods
{
    public static class EpiserverFindExtensionMethods
    {
        // Elasticsearch uses the following stop words by default
        // https://www.elastic.co/guide/en/elasticsearch/guide/current/stopwords.html
        public static string elasticsearchStopWords =
            "a|an|and|are|as|at|be|but|by|for|if|in|into|is|it|no|not|of|on|or|such|that|the|their|then|there|these|they|this|to|was|will|with";

        // add any organisation-specific stop words e.g. Royal Mint might ignore coin and money
        public static string extraStopWords = "coin|money";

        public static string stopWords = elasticsearchStopWords + "|" + extraStopWords;

        public static string RemoveStopWords(this string input)
        {
            var stopWordsRegex = new Regex("^(" + stopWords + ")$");

            var words = input.Split(' ');

            var remainingWords = words.Where(
                word => !stopWordsRegex.IsMatch(word.Trim()));

            var result = string.Join(" ", remainingWords);

            return result;
        }
    }
}