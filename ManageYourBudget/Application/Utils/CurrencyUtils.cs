namespace Application.Utils
{
    public class CurrencyUtils
    {
        public static string FormatCurrencyDisplay(string currencyCode)
        {
            if (currencyCode == "uah")
            {
                return "₴";
            }
            else if (currencyCode == "usd")
            {
                return "$";
            }
            else
            {
                throw new Exception("Wrong currency");
            }
        }
    }
}
