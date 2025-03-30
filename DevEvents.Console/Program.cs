// See https://aka.ms/new-console-template for more information

using System.IO.Compression;
using System.Text;

var path = "example.txt";
var text = "Hello from FileStream";

// Write to File
using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
{
    var buffer = Encoding.UTF8.GetBytes(text);
    fs.Write(buffer, 0, buffer.Length);
}

// Read from file
using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
{
    var buffer = new byte[fs.Length];
    fs.Read(buffer, 0, buffer.Length);
    var result = Encoding.UTF8.GetString(buffer);
    Console.WriteLine(result);
}

text = "Hello from MemoryStream";

// Write
using (MemoryStream ms = new MemoryStream())
{
    var buffer = Encoding.UTF8.GetBytes(text);

    ms.Write(buffer, 0, buffer.Length);

    // Read
    ms.Position = 0;
    var readBuffer = new byte[ms.Length];
    ms.Read(readBuffer, 0, readBuffer.Length);

    var result = Encoding.UTF8.GetString(readBuffer);
    Console.WriteLine(result);
}

using (var reader = new StreamReader("example.txt"))
{
    string? line;

    while((line = reader.ReadLine()) != null)
    {
        Console.WriteLine(line);
    }
}

using (var writer = new StreamWriter("log.txt"))
{
    writer.WriteLine("Log Entry: " + DateTime.Now);
}

// Compress
using (var fs = File.OpenRead("example.txt"))
{
    using (var compressedFile = File.Create("example.gz"))
    {
        using (var gzip = new GZipStream(compressedFile, CompressionMode.Compress))
        {
            await fs.CopyToAsync(gzip);
        }
    }
}

// Decompress
using (var fs = File.OpenRead("example.gz"))
{
    using (var decompressedFile = File.Create("decompressed.txt"))
    {
        using (var gzip = new GZipStream(fs, CompressionMode.Decompress))
        {
            await gzip.CopyToAsync(decompressedFile);
        }
    }
}


Console.ReadLine();