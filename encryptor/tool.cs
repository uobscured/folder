using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class AESTool
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("\nModes: (E)ncrypt string, (D)ecrypt string, (EF) encrypt file, (DF) decrypt file, (Q)uit");
            string mode = Console.ReadLine().Trim().ToUpper();
            if (mode == "Q") break;

            Console.Write("Enter AES key: ");
            string keyInput = Console.ReadLine();
            Console.Write("Enter IV (16 chars): ");
            string ivInput = Console.ReadLine();
            if (ivInput.Length != 16) { Console.WriteLine("IV must be 16 chars."); continue; }

            byte[] key = DeriveKey(keyInput);
            byte[] iv = Encoding.UTF8.GetBytes(ivInput);

            try
            {
                if (mode == "E")
                {
                    Console.Write("Enter string to encrypt: ");
                    string plain = Console.ReadLine();
                    string enc = EncryptString(plain, key, iv);
                    Console.WriteLine("Encrypted (Base64):\n" + enc);
                }
                else if (mode == "D")
                {
                    Console.Write("Enter Base64 string to decrypt: ");
                    string enc = Console.ReadLine();
                    string dec = DecryptString(enc, key, iv);
                    Console.WriteLine("Decrypted string:\n" + dec);

                    // Print one-liner decryptor snippet
                    Console.WriteLine("\nEquivalent one-line decryptor snippet:");
                    Console.WriteLine(
@"using System;using System.IO;using System.Text;using System.Security.Cryptography;
class P{static void Main(){var aes=Aes.Create();aes.Key=SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(""" + keyInput + @"""));aes.IV=Encoding.UTF8.GetBytes(""" + ivInput + @""");string enc=""" + enc + @""";Console.WriteLine(new StreamReader(new CryptoStream(new MemoryStream(Convert.FromBase64String(enc)),aes.CreateDecryptor(),CryptoStreamMode.Read)).ReadToEnd());}}"
                    );
                }
                else if (mode == "EF")
                {
                    Console.Write("Input file path: ");
                    string inPath = Console.ReadLine();
                    Console.Write("Output file path: ");
                    string outPath = Console.ReadLine();
                    byte[] data = File.ReadAllBytes(inPath);
                    string b64 = Convert.ToBase64String(EncryptBytes(data, key, iv));
                    File.WriteAllText(outPath, b64);
                    Console.WriteLine("File encrypted to Base64 at: " + outPath);
                }
                else if (mode == "DF")
                {
                    Console.Write("Input file path (Base64 ciphertext): ");
                    string inPath = Console.ReadLine();
                    Console.Write("Output file path (decrypted): ");
                    string outPath = Console.ReadLine();
                    string b64 = File.ReadAllText(inPath);
                    byte[] decBytes = DecryptBytes(b64, key, iv);
                    File.WriteAllBytes(outPath, decBytes);
                    Console.WriteLine("File decrypted to: " + outPath);

                    // Print one-liner decryptor snippet for file content
                    Console.WriteLine("\nEquivalent one-line decryptor snippet:");
                    Console.WriteLine(
@"using System;using System.IO;using System.Text;using System.Security.Cryptography;
class P{static void Main(){var aes=Aes.Create();aes.Key=SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(""" + keyInput + @"""));aes.IV=Encoding.UTF8.GetBytes(""" + ivInput + @""");string enc=File.ReadAllText(""" + inPath + @""");Console.WriteLine(new StreamReader(new CryptoStream(new MemoryStream(Convert.FromBase64String(enc)),aes.CreateDecryptor(),CryptoStreamMode.Read)).ReadToEnd());}}"
                    );
                }
                else Console.WriteLine("Invalid mode.");
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }
    }

    static byte[] DeriveKey(string keyInput)
    {
        using (SHA256 sha = SHA256.Create())
            return sha.ComputeHash(Encoding.UTF8.GetBytes(keyInput));
    }

    static string EncryptString(string plainText, byte[] key, byte[] iv)
    {
        byte[] cipher = EncryptBytes(Encoding.UTF8.GetBytes(plainText), key, iv);
        return Convert.ToBase64String(cipher);
    }

    static string DecryptString(string base64Input, byte[] key, byte[] iv)
    {
        byte[] plain = DecryptBytes(base64Input, key, iv);
        return Encoding.UTF8.GetString(plain);
    }

    static byte[] EncryptBytes(byte[] data, byte[] key, byte[] iv)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = key; aes.IV = iv; aes.Mode = CipherMode.CBC; aes.Padding = PaddingMode.PKCS7;
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            { cs.Write(data, 0, data.Length); cs.FlushFinalBlock(); return ms.ToArray(); }
        }
    }

    static byte[] DecryptBytes(string base64Input, byte[] key, byte[] iv)
    {
        byte[] cipherBytes = Convert.FromBase64String(base64Input);
        using (var aes = Aes.Create())
        {
            aes.Key = key; aes.IV = iv; aes.Mode = CipherMode.CBC; aes.Padding = PaddingMode.PKCS7;
            using (var ms = new MemoryStream(cipherBytes))
            using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
            using (var outMs = new MemoryStream())
            { cs.CopyTo(outMs); return outMs.ToArray(); }
        }
    }
}
