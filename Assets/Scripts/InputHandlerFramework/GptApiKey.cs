
using System;
using System.IO;
using System.Security.AccessControl;

// the main ApiKey-Handler class
class GptApiKey
{
    public string FilePath { get; }

    // Save key to a file and lock it.
    public void SaveKeyToFile(string key)
    {
        try
        {
            // file may already be encrypted - decrypt it if yes
            FileAttributes fiAttributes = File.GetAttributes(FilePath);

            if ((fiAttributes & FileAttributes.Encrypted) == FileAttributes.Encrypted)
            {
                //File is Encrypted. Decrypt the file
                File.Decrypt(FilePath);
            }
            // open stream writer and save the key
            using (StreamWriter w = new StreamWriter(FilePath))
            {
                w.WriteLine(key);
                Console.WriteLine("Key Saved In " + FilePath);

                // Encrypt the file.
                File.Encrypt(FilePath);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    // Decrypt a file.
    public string GetKeyFromFile()
    {
        string LockedKey;
        try
        {
            // The built-in decryption method of System
            File.Decrypt(FilePath);

            using (StreamReader r = new StreamReader(FilePath))
            {
                LockedKey = r.ReadLine();
            }

            return LockedKey;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public GptApiKey(string filePath)
    {
        FilePath = filePath;
    }
}

// a class to test out the file encryption / decryption 
class GptApiKeyTestDriver
{
    public static void Main()
    {
        try
        {
            // create a new class instance
            GptApiKey host = new GptApiKey("C:/Users/Tn/Documents/ApiKeyHandler/bin/debug/test.txt");
            host.SaveKeyToFile("IloveCoding427");

            string example = host.GetKeyFromFile();
            Console.WriteLine(example);

            Console.WriteLine("Done");
        }
        // Catch any thrown error, most likely the UnAuthorizedAccess exception error
        catch (Exception)
        {
            throw;
        }
    }
}