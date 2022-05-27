/**
* Project: Pixel Walker
*
* Description: 
* GptApiKey is a class that saves a key to a file then encrypt it,
* then if anyone wants to access the program, they need to 
* decrypt the key file and then they can get the pass key.
*
* Author: Pixel Walker -
* Maynard, Gregory
* Shubhajeet, Baral
* Do, Khuong
* Nguyen, Thuong
*
* Date: 05-26-2022
*/
using System;
using System.IO;
using System.Security.AccessControl;

/** the main ApiKey-Handler class that contains:
* A method to save the key to file, encrypt it;
* A method to decrypt the file, get the key;
* A file path property.
*/
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

    // Get the pass key from the encrypted file.
    public string GetKeyFromFile()
    {
        string LockedKey;
        try
        {
            // Decrypt the file
            File.Decrypt(FilePath);
            
            // Open stream Reader
            using (StreamReader r = new StreamReader(FilePath))
            {
                // store the key as the string to output
                LockedKey = r.ReadLine();
            }
            return LockedKey;
        }
        catch (Exception)
        {
            throw;
        }
    }

    // constructor of GptApiKey class with the file path
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
        catch (Exception)
        {
            throw;
        }
    }
}
