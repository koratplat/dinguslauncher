﻿#region Usings
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
#endregion

#region .NET Extentions

//This code was brought to you by:
//https://stackoverflow.com/questions/1926264/color-different-parts-of-a-richtextbox-string
//https://stackoverflow.com/questions/262280/how-can-i-know-if-a-process-is-running
//https://stackoverflow.com/questions/444798/case-insensitive-containsstring
//https://stackoverflow.com/questions/6084940/how-do-i-search-a-multi-dimensional-array
//https://www.dotnetperls.com/between-before-after
//https://stackoverflow.com/questions/12422619/can-i-disable-the-close-button-of-a-form-using-c
//https://stackoverflow.com/questions/9031537/really-simple-encryption-with-c-sharp-and-symmetricalgorithm

public static class NETExt
{
    #region Rich Text Box Extensions
    public static void AppendText(this RichTextBox box, string text, Color color)
    {
        box.SelectionStart = box.TextLength;
        box.SelectionLength = 0;

        box.SelectionColor = color;
        box.AppendText(text);
        box.SelectionColor = box.ForeColor;
    }
    #endregion

    #region Process Extensions
    public static bool IsRunning(this Process process)
    {
        try
        {
            Process.GetProcessById(process.Id);
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }
        return true;
    }
    #endregion

    #region String Extensions
    public static bool Contains(this string source, string toCheck, StringComparison comp)
    {
        if (source == null)
            return false;
        return source.IndexOf(toCheck, comp) >= 0;
    }
    #endregion

    #region Array Helper
    public static object FindInDimensions(this object[,] target,
          object searchTerm)
    {
        object result = null;
        var rowLowerLimit = target.GetLowerBound(0);
        var rowUpperLimit = target.GetUpperBound(0);

        var colLowerLimit = target.GetLowerBound(1);
        var colUpperLimit = target.GetUpperBound(1);

        for (int row = rowLowerLimit; row < rowUpperLimit; row++)
        {
            for (int col = colLowerLimit; col < colUpperLimit; col++)
            {
                // you could do the search here...
            }
        }

        return result;
    }
    #endregion

    #region Substring Extensions
    /// <summary>
    /// Get string value between [first] a and [last] b.
    /// </summary>
    public static string Between(this string value, string a, string b)
    {
        int posA = value.IndexOf(a);
        int posB = value.LastIndexOf(b);
        if (posA == -1)
        {
            return "";
        }
        if (posB == -1)
        {
            return "";
        }
        int adjustedPosA = posA + a.Length;
        if (adjustedPosA >= posB)
        {
            return "";
        }
        return value.Substring(adjustedPosA, posB - adjustedPosA);
    }

    /// <summary>
    /// Get string value after [first] a.
    /// </summary>
    public static string Before(this string value, string a)
    {
        int posA = value.IndexOf(a);
        if (posA == -1)
        {
            return "";
        }
        return value.Substring(0, posA);
    }

    /// <summary>
    /// Get string value after [last] a.
    /// </summary>
    public static string After(this string value, string a)
    {
        int posA = value.LastIndexOf(a);
        if (posA == -1)
        {
            return "";
        }
        int adjustedPosA = posA + a.Length;
        if (adjustedPosA >= value.Length)
        {
            return "";
        }
        return value.Substring(adjustedPosA);
    }
    #endregion

    #region String Utilities
    private static byte[] key = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
    private static byte[] iv = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };

    public static string Crypt(this string text)
    {
        SymmetricAlgorithm algorithm = DES.Create();
        ICryptoTransform transform = algorithm.CreateEncryptor(key, iv);
        byte[] inputbuffer = Encoding.Unicode.GetBytes(text);
        byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
        return Convert.ToBase64String(outputBuffer);
    }

    public static string Decrypt(this string text)
    {
        SymmetricAlgorithm algorithm = DES.Create();
        ICryptoTransform transform = algorithm.CreateDecryptor(key, iv);
        byte[] inputbuffer = Convert.FromBase64String(text);
        byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
        return Encoding.Unicode.GetString(outputBuffer);
    }
    #endregion

    #region Exception Helpers
    //https://github.com/AlexMelw/EasySharp/blob/master/NHelpers/ExceptionsDealing/Extensions/ExceptionExtensions.cs
    /// <summary>
    ///     Gets the entire stack trace consisting of exception's footprints (File, Method, LineNumber)
    /// </summary>
    /// <param name="exception">Source <see cref="Exception" /></param>
    /// <returns>
    ///     <see cref="string" /> that represents the entire stack trace consisting of exception's footprints (File,
    ///     Method, LineNumber)
    /// </returns>
    public static string GetExceptionFootprints(this Exception exception)
    {
        StackTrace stackTrace = new StackTrace(exception, true);
        StackFrame[] frames = stackTrace.GetFrames();

        if (ReferenceEquals(frames, null))
        {
            return string.Empty;
        }

        var traceStringBuilder = new StringBuilder();

        for (var i = 0; i < frames.Length; i++)
        {
            StackFrame frame = frames[i];

            if (frame.GetFileLineNumber() < 1)
                continue;

            traceStringBuilder.AppendLine($"File: {frame.GetFileName()}");
            traceStringBuilder.AppendLine($"Method: {frame.GetMethod().Name}");
            traceStringBuilder.AppendLine($"LineNumber: {frame.GetFileLineNumber()}");

            if (i == frames.Length - 1)
                break;

            traceStringBuilder.AppendLine(" ---> ");
        }

        string stackTraceFootprints = traceStringBuilder.ToString();

        if (string.IsNullOrWhiteSpace(stackTraceFootprints))
            return "NO DETECTED FOOTPRINTS";

        return stackTraceFootprints;
    }
    #endregion

    #region DirectoryInfo Extensions
    public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dir, params string[] extensions)
    {
        if (extensions == null)
            throw new ArgumentNullException("extensions");
        IEnumerable<FileInfo> files = dir.EnumerateFiles();
        return files.Where(f => extensions.Contains(f.Extension));
    }
    #endregion
}
#endregion