using Microsoft.VisualBasic;

namespace Alpha.Utilities.Extensions;

public static class PasswordHashExtensions
{
    public static string Hash(string val)
    {
        const int num = 30;
        int num2 = num - val.Length;
        if (num2 > 1)
        {
            for (int m = 1; m <= num2; m++)
            {
                val = string.Format("{0}{1}", val, Strings.Chr(0x15));
            }
        }

        double num3 = 1.0;
        for (int i = 1; i <= num; i++)
        {
            int num5 = num + (Strings.Asc(Strings.Mid(val, i, 1)) * i);
            num3 *= num5;
        }

        val = num3.ToString("G15");
        string str = string.Empty;
        for (int j = 1; j <= val.Length; j++)
        {
            str = string.Format("{0}{1}", str, HashCode(Strings.Mid(val, j, 3)));
        }

        string str2 = string.Empty;
        for (int k = 20; k <= (str.Length - 0x12); k += 2)
        {
            str2 = string.Format("{0}{1}", str2, Strings.Mid(str, k, 1));
        }

        return str2;
    }

    public static string RandomNumberString(int length)
    {
        const string chars = "0123456789";
        string rndStr = string.Empty;
        Random rnd = new Random();

        for (int i = 0; i < length; ++i)
        {
            rndStr += chars[rnd.Next() % chars.Length];
        }

        return rndStr;
    }

    private static string HashCode(string words)
    {
        string number = string.Empty;
        for (int i = 1; i <= words.Length; i++)
        {
            number = number + Strings.Asc(Strings.Mid(words, i, 1));
        }

        return Conversion.Hex(number);
    }
}